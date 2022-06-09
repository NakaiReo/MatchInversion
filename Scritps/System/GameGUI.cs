using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// GameGUIの基本クラス
/// </summary>
public abstract class GameGUIContent
{
	public CanvasGroup canvasGroup;

	public void SetActive(bool enable)
	{
		if (enable) EnableAction();
		else        DisableAction();

		CanvasActive(canvasGroup, enable);
	}

	public void CanvasActive(CanvasGroup cg, bool enable)
	{
		float fadeTime = enable ? 0.5f : 0.1f;

		cg.DOFade(enable ? 1.0f : 0.0f, fadeTime);
		cg.blocksRaycasts = enable;
		cg.interactable = enable;
	}

	public abstract void EnableAction();
	public abstract void DisableAction();
}

[System.Serializable]
public class TitleContent : GameGUIContent
{
	public CanvasGroup mainTitleCanvasGroup;
	public CanvasGroup startGameCanvasGroup;
	public CanvasGroup rankingCanvasGroup;

	[Space]
	public TMP_InputField inputPlayerName;

	public enum TitleScene
	{
		MainTitle,
		StartGame,
		Ranking,
	}

	public override void DisableAction()
	{

	}

	public override void EnableAction()
	{
		SettingMenu.ins.useButtons = false;
	}

	/// <summary>
	/// タイトルのシーン切り替え
	/// </summary>
	/// <param name="titleScene"></param>
	public void TitleSceneChange(TitleScene titleScene)
	{
		CanvasActive(mainTitleCanvasGroup, titleScene == TitleScene.MainTitle);
		CanvasActive(startGameCanvasGroup, titleScene == TitleScene.StartGame);
		CanvasActive(rankingCanvasGroup,   titleScene == TitleScene.Ranking);

		if (titleScene == TitleScene.MainTitle) AnimationGUI(mainTitleCanvasGroup.transform);
		if (titleScene == TitleScene.StartGame) AnimationGUI(startGameCanvasGroup.transform);
		if (titleScene == TitleScene.Ranking)   AnimationGUI(rankingCanvasGroup.transform);
	}

	/// <summary>
	/// シーン切り替えのアニメーション
	/// </summary>
	public void AnimationGUI(Transform tf)
	{
		int childCount = tf.childCount;

		for(int i = 0; i < childCount; i++)
		{
			Transform ctf = tf.GetChild(i);

			ctf.DOScaleX(0, 0.0f);

			DOTween.Sequence()
				.Append(ctf.DOScaleX(1, 0.5f))
				.SetDelay(i * 0.1f);
		}
	}
}

[System.Serializable]
public class GameContent : GameGUIContent
{
	public TMP_Text labelText;
	public TMP_Text matchCountText;
	public TMP_Text limitedText;
	public TMP_Text leftTimeText;
	public Slider leftTimeSlider;
	public CountDownTMP countDownText;
	public TimeOutTMP timeOutText;

	public override void EnableAction()
	{
		SettingMenu.ins.useButtons = true;
	}

	public override void DisableAction()
	{
		
	}

	/// <summary>
	/// 初期化
	/// </summary>
	public void Init(int matchCount, float leftTime, float limitCount)
	{
		SetMatchCountGUI(matchCount);
		SetTimeGUI(leftTime, limitCount);
		countDownText.Init();
		timeOutText.Init();
	}

	/// <summary>
	/// ゲームモードの描画
	/// </summary>
	public void SetLabelText(GameDirector.GameMode gameMode)
    {
		string gamemodeText = gameMode switch
		{
			GameDirector.GameMode.Normal => "Normal Mode",
			GameDirector.GameMode.Limited => "Limited Mode",
			_ => "存在しないモード"
		};

		labelText.text = gamemodeText;
    }

	/// <summary>
	/// 残り試行回数のの描画
	/// </summary>
	public void SetLimitedCountText(int limitedCount)
    {
		limitedText.text = "残り試行回数: " + limitedCount + "回";
    }

	/// <summary>
	/// 揃えた数の描画
	/// </summary>
	public void SetMatchCountGUI(int matchCount)
	{
		matchCountText.text = "揃えた数: " + matchCount.ToString() + "回";
	}

	/// <summary>
	/// 残り時間の描画
	/// </summary>
	public void SetTimeGUI(float leftTime, float limitTime)
	{
		leftTimeText.text = "残り時間: " + leftTime.ToString("F1") + "秒";
		leftTimeText.color = leftTime < 10.0f ? Color.red : Color.white;
		leftTimeSlider.value = leftTime / limitTime;
	}

	/// <summary>
	/// カウントダウンの描画
	/// </summary>
	public void SetCountDownText(int time)
	{
		countDownText.Play("- " + time.ToString() + " -", 1.0f, 0.2f, 0.0f);
	}

	/// <summary>
	/// タイムアウトの描画
	/// </summary>
	public void TimeOutTextPlay(float time)
	{
		timeOutText.Play(time);
	}

	/// <summary>
	/// タイムアウトの非表示
	/// </summary>
	public void TimeOutTextFade(float time)
	{
		timeOutText.Fade(time);
	}
}

[System.Serializable]
public class ResultContent : GameGUIContent
{
	public TMP_Text matchCountText;
	public TMP_Text averageCountText;
	public TMP_Text bestScoreText;
	public CanvasGroup rankingScrollViewCanvasGroup;
	public Transform rankingScrollViewContent;
	public GameObject rankingTextContent;
	public CanvasGroup buttonsCanvasGroup;

	public override void EnableAction()
	{
	}

	public override void DisableAction()
	{
	}
}

public class GameGUI : MonoBehaviour
{
	public static GameGUI ins = null;

    public enum SceneType
	{
		Title,
		Gameplay,
		Result,
	}

	public SceneType currentScene { get; private set; }

	public delegate void CallBackAction(); 

	public TitleContent titleContent;
	public GameContent gameContent;
	public ResultContent resultContent;

	/// <summary>
	/// GUIの切り替え
	/// </summary>
	public void ChangeGUI(SceneType sceneType, CallBackAction callBack = null)
	{
		currentScene = sceneType;

		Debug.Log(sceneType);
		titleContent. SetActive(sceneType == SceneType.Title);
		gameContent.  SetActive(sceneType == SceneType.Gameplay);
		resultContent.SetActive(sceneType == SceneType.Result);

		if (callBack != null) callBack();
	}
}

public static class TMP_Extend
{
	public static void FadeOutText(this TMP_Text tmpText, string str, float time)
	{
		tmpText.color = new Color(0, 0, 0, 0);
		tmpText.text = str;
		tmpText.DOColor(ColorDirector.RESULT_COLOR, time);
	}

	public static void FadeOutCanvasGroup(this CanvasGroup canvasGroup, float time)
	{
		canvasGroup.alpha = 0;
		canvasGroup.DOFade(1.0f, time);
	}
}