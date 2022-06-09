using System.Text;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    //アカウントを作成するか
    private bool shouldCreateAccount;

    //ログイン時に使うID
    public string customID { get; private set; }

    public void Login()
    {
        customID = LoadCustomID();
        var request = new LoginWithCustomIDRequest { CustomId = customID, CreateAccount = shouldCreateAccount };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    //ログイン成功
    private void OnLoginSuccess(LoginResult result)
    {
        //アカウントを作成しようとしたのに、IDが既に使われていて、出来なかった場合
        if (shouldCreateAccount && !result.NewlyCreated)
        {
            Debug.LogWarning($"CustomId : {customID} は既に使われています。");
            Login();//ログインしなおし
            return;
        }

        //アカウント作成時にIDを保存
        if (result.NewlyCreated)
        {
            SaveCustomID();
        }
        Debug.Log($"PlayFabのログインに成功\nPlayFabId : {result.PlayFabId}, CustomId : {customID}\nアカウントを作成したか : {result.NewlyCreated}");
    }

    //ログイン失敗
    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError($"PlayFabのログインに失敗\n{error.GenerateErrorReport()}");
    }

    //=================================================================================
    //カスタムIDの取得
    //=================================================================================

    //IDを保存する時のKEY
    private static readonly string CUSTOM_ID_SAVE_KEY = "CUSTOM_ID_SAVE_KEY";

    //IDを取得
    private string LoadCustomID()
    {
        //IDを取得
        string id = PlayerPrefs.GetString(CUSTOM_ID_SAVE_KEY);

        //保存されていなければ新規生成
        shouldCreateAccount = string.IsNullOrEmpty(id);
        return shouldCreateAccount ? GenerateCustomID() : id;
    }

    //IDの保存
    private void SaveCustomID()
    {
        PlayerPrefs.SetString(CUSTOM_ID_SAVE_KEY, customID);
    }

    //=================================================================================
    //カスタムIDの生成
    //=================================================================================

    //IDを生成する
    private string GenerateCustomID()
    {
        System.Guid guid = System.Guid.NewGuid();

        return guid.ToString();
    }
}
