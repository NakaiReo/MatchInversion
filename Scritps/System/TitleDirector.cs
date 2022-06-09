using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TitleDirector : MonoBehaviour
{
    [SerializeField] GameDirector gameDirector;   //ゲームの進行を管理するクラス
    [SerializeField] RankingSystem rankingSystem; //ランキングを管理するクラス
    [SerializeField] GameGUI gameGUI;             //GUIを管理するクラス

    const int MAX_NAME_LENGTH = 10;

    // Use this for initialization
    IEnumerator Start()
    {
        //PlayFabにログイン
        Fade.ins.FadeIn(0);
        rankingSystem.login.Login();
        yield return new WaitForSeconds(2.0f);

        //名前の取得
        rankingSystem.GetUserName((s) => GetPlayName(s));
        Fade.ins.FadeOut(1.0f);
        gameGUI.ChangeGUI(GameGUI.SceneType.Title); //現在のGUIをTitleに変更
        gameGUI.titleContent.TitleSceneChange(TitleContent.TitleScene.MainTitle); //現在のSceneをMainTitleに
    }

    /// <summary>
    /// ボタンの処理: ゲームモードの選択
    /// </summary>
    public void OnGameStart()
    {
        Debug.Log("OnPushGameStart!");
        SoundDirector.ins.SE.Play("Select");
        gameGUI.titleContent.TitleSceneChange(TitleContent.TitleScene.StartGame);
    }

    /// <summary>
    /// ボタンの処理: 設定を開く
    /// </summary>
    public void OnSetting()
    {
        SoundDirector.ins.SE.Play("Select");
        SettingMenu.ins.OnMenuEnable();
    }

    /// <summary>
    /// ボタンの処理: ランキングの表示
    /// </summary>
    public void OnRanking()
    {
        SoundDirector.ins.SE.Play("Select");
        gameGUI.titleContent.TitleSceneChange(TitleContent.TitleScene.Ranking);
    }


    /// <summary>
    /// ボタンの処理: ゲームの終了
    /// </summary>
    public void OnShutdown()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    /// <summary>
    /// ボタンの処理: NormalModeでゲームスタート
    /// </summary>
    public void OnNormalModeStart()
    {
        SoundDirector.ins.SE.Play("Select");
        UpdatePlayerName();
        gameDirector.StartGame(GameDirector.GameMode.Normal);
    }

    /// <summary>
    /// ボタンの処理: LimitedModeでスタート
    /// </summary>
    public void OnLimitedModeStart()
    {
        SoundDirector.ins.SE.Play("Select");
        UpdatePlayerName();
        gameDirector.StartGame(GameDirector.GameMode.Limited);
    }

    /// <summary>
    /// ボタンの処理: 戻る
    /// </summary>
    public void OnBack()
    {
        SoundDirector.ins.SE.Play("Cancel");
        gameGUI.titleContent.TitleSceneChange(TitleContent.TitleScene.MainTitle);
    }

    /// <summary>
    /// プレイヤー名入力に名前を入れる
    /// </summary>
    /// <param name="name"></param>
    public void GetPlayName(string name)
    {
        gameGUI.titleContent.inputPlayerName.text = name;
    }

    /// <summary>
    /// 名前を更新する
    /// </summary>
    public void UpdatePlayerName()
    {
        rankingSystem.UpdateUserName(gameGUI.titleContent.inputPlayerName.text);
    }
}