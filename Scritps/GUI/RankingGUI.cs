using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class RankingGUI : MonoBehaviour
{
	[SerializeField] RankingSystem rankingSystem; //ランキングの情報を管理するクラス
	[SerializeField] TMP_Text labelText;          //ランキングの名前のテキスト
	[SerializeField] RectTransform rankingPanel;  //ランキングのパネル
	[SerializeField] CanvasGroup rankingContent;  //ランキングのコンテンツ
	[SerializeField] float delay;                 //表示遅延時間

	//現在のページ
	//0.Normal Mode - Top100
	//1.Normal Mode - Player Around
	//2.Limited Mode - Top100
	//3.Limited Mode - Player Around
	int index; 


	/// <summary>
	/// 前のページに戻る
	/// </summary>
	public void LeftNextPage()
	{
		index--;

		if(index < 0)
		{
			index = GameDirector.GameModeLength * 2 - 1;
		}

		DrawRanking();
	}

	/// <summary>
	/// 次のページに進む
	/// </summary>
	public void RightNextPage()
	{
		index++;

		if(index >= GameDirector.GameModeLength * 2)
		{
			index = 0;
		}

		DrawRanking();
	}

	/// <summary>
	/// 初期化
	/// </summary>
	public void Init()
	{
		index = 0;
		rankingPanel.DOScaleX(0, 0);

		DrawRanking();
	}

	/// <summary>
	/// ランキングの処理
	/// </summary>
	public void DrawRanking()
	{
		GameDirector.GameMode gameMode = (GameDirector.GameMode)(index / 2); //表示するゲームモード
		bool isPlayerAround = index % 2 == 1; //プレイヤーの周辺を取得するかどうか

		//ランキングのラベルテキストを更新
		labelText.text = GetGameModeName(gameMode) + " - " + GetRangeString(isPlayerAround);

		if (isPlayerAround)
		{
			//プレイヤーの周辺のランキングを取得
			rankingSystem.GetLeaderboardAroundPlayer(RankingSystem.LeaderBoardType.RankingList ,gameMode);
		}
		else
		{
			//そのゲームモードのTop100を表示
			rankingSystem.GetLeaderboardTop(RankingSystem.LeaderBoardType.RankingList, gameMode);
		}

		//ランキングの描画
		StartCoroutine(DrawRankingGUI());
	}

	/// <summary>
	/// ランキングの描画
	/// </summary>
	IEnumerator DrawRankingGUI()
	{
		Transform tf = rankingPanel.transform;
		int childCount = tf.childCount;

		DOTween.Sequence()
				.Append(tf.DOScaleX(0, 0.25f).SetEase(Ease.InSine))
				.Join(rankingContent.DOFade(1, 0.25f))
				.Append(tf.DOScaleX(1, 0.25f).SetEase(Ease.OutSine))
				.AppendInterval(delay)
				.Append(rankingContent.DOFade(1, 0.5f));

		yield break;
	}


	//ゲームモードをテキストに直す
	string GetGameModeName(GameDirector.GameMode gameMode) => gameMode switch
	{
		GameDirector.GameMode.Normal => "Normal Mode",
		GameDirector.GameMode.Limited => "Limited Mode",
		_ => "Null",
	};

	//ランキングの取得方法をテキストに直す
	string GetRangeString(bool isPlayerAround) => isPlayerAround switch
	{
		false => "Top100",
		true  => "Around",
	};
}