using System.Text;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class PlayFabLogin : MonoBehaviour
{
    //�A�J�E���g���쐬���邩
    private bool shouldCreateAccount;

    //���O�C�����Ɏg��ID
    public string customID { get; private set; }

    public void Login()
    {
        customID = LoadCustomID();
        var request = new LoginWithCustomIDRequest { CustomId = customID, CreateAccount = shouldCreateAccount };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    //���O�C������
    private void OnLoginSuccess(LoginResult result)
    {
        //�A�J�E���g���쐬���悤�Ƃ����̂ɁAID�����Ɏg���Ă��āA�o���Ȃ������ꍇ
        if (shouldCreateAccount && !result.NewlyCreated)
        {
            Debug.LogWarning($"CustomId : {customID} �͊��Ɏg���Ă��܂��B");
            Login();//���O�C�����Ȃ���
            return;
        }

        //�A�J�E���g�쐬����ID��ۑ�
        if (result.NewlyCreated)
        {
            SaveCustomID();
        }
        Debug.Log($"PlayFab�̃��O�C���ɐ���\nPlayFabId : {result.PlayFabId}, CustomId : {customID}\n�A�J�E���g���쐬������ : {result.NewlyCreated}");
    }

    //���O�C�����s
    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError($"PlayFab�̃��O�C���Ɏ��s\n{error.GenerateErrorReport()}");
    }

    //=================================================================================
    //�J�X�^��ID�̎擾
    //=================================================================================

    //ID��ۑ����鎞��KEY
    private static readonly string CUSTOM_ID_SAVE_KEY = "CUSTOM_ID_SAVE_KEY";

    //ID���擾
    private string LoadCustomID()
    {
        //ID���擾
        string id = PlayerPrefs.GetString(CUSTOM_ID_SAVE_KEY);

        //�ۑ�����Ă��Ȃ���ΐV�K����
        shouldCreateAccount = string.IsNullOrEmpty(id);
        return shouldCreateAccount ? GenerateCustomID() : id;
    }

    //ID�̕ۑ�
    private void SaveCustomID()
    {
        PlayerPrefs.SetString(CUSTOM_ID_SAVE_KEY, customID);
    }

    //=================================================================================
    //�J�X�^��ID�̐���
    //=================================================================================

    //ID�𐶐�����
    private string GenerateCustomID()
    {
        System.Guid guid = System.Guid.NewGuid();

        return guid.ToString();
    }
}
