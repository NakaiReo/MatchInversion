using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameDirector : MonoBehaviour
{
	[SerializeField] GameGUI gameGUI; //GUIを更新するクラス
	[SerializeField] RankingSystem rankingSystem; //ランキングを管理するクラス
	[SerializeField] AdsScript adsScript; //広告のクラス
	[SerializeField] Transform tableParent;  //盤面の親Transform
	[SerializeField] GameObject matchEffect; //盤面が揃った時のエフェクト

	[Space]
	[SerializeField] float limitTime; float leftTime; //残り時間
	[SerializeField] int maxLimitedCount; int currentLimitedCount; //残り試行回数
	[SerializeField] int gridSize; //盤面のサイズ

	/// <summary>
	/// ゲームモード
	/// </summary>
	public enum GameMode
    {
		Normal,
		Limited,
    }

	public static int GameModeLength = 2; //ゲームモードの数

	public GameMode useGameMode { get; private set; } //現在使用されているゲームモード

	int sumFlipCount; //試行回数
	int matchCount;   //揃えられた回数
	float countDownTime; int backCountDownTime; //カウントダウン用の変数

	int gridLength { get { return gridSize * gridSize; } } //盤面の総数
	bool[] tableData; //現在の盤面の状態
	Image[] buttonImages; //盤面のImage

	public enum State
	{
		Init,      //初期化
		CountDown, //カウントダウン
		GamePlay,  //ゲーム中
		TimeOut,   //時間切れ
		Result,    //結果の表示
	}
	public State currentState; //ゲームの進行状態

	private void Awake()
	{
		//盤面の初期化
		buttonImages = new Image[gridLength];

		for(int i = 0; i < gridLength; i++)
		{
			Transform tf = tableParent.GetChild(i);
			buttonImages[i] = tf.GetComponent<Image>();

			int n = i;
			tf.GetComponent<Button>().onClick.AddListener(() => PushButton(n));
		}
	}


	private void Update()
	{
		//現在のゲームの進行状態によるUpdate
		switch (currentState)
		{
			case State.CountDown:
				UpdateCountDown();
				break;
			case State.GamePlay:
				UpdateGamePlay();
				break;
			case State.TimeOut:
				break;
			case State.Result:
				break;
		}
	}

	/// <summary>
	/// カウントダウン用のUpdate
	/// </summary>
	void UpdateCountDown()
	{
		//カウントダウンが0になったらゲームを開始
		if(countDownTime <= 0)
		{
			StartGamePlay();
			return;
		}

		//1秒ごとにテキストの更新
		{
			int ceilTime = Mathf.CeilToInt(countDownTime);

			//残り時間が3秒以上なら表示しない
			if (countDownTime >= 3.0f)
			{
				gameGUI.gameContent.countDownText.Init();
			}

			//1秒ごとにテキストを変える
			else if (ceilTime < backCountDownTime)
			{
				backCountDownTime = ceilTime;
				SoundDirector.ins.SE.Play("CountDown");
				gameGUI.gameContent.SetCountDownText(ceilTime);
			}
		}

		countDownTime -= Time.deltaTime;
	}

	/// <summary>
	/// ゲーム中用のUpdate
	/// </summary>
	void UpdateGamePlay()
	{
		//残り時間が0以下ならゲームを終了する
		if (leftTime <= 0)
		{
			StartCoroutine(TimeOut());
		}

		//残り時間が5秒以下の時、1秒ごとに音を鳴らす
		int ceilTime = Mathf.CeilToInt(countDownTime);
		if (ceilTime < backCountDownTime && ceilTime <= 5.0f)
		{
			backCountDownTime = ceilTime;
			SoundDirector.ins.SE.Play("LowCountDown");
		}

		//残り時間の描画
		gameGUI.gameContent.SetTimeGUI(leftTime, limitTime);
		leftTime -= Time.deltaTime;
	}

	/// <summary>
	/// ゲームの開始
	/// </summary>
	void StartGamePlay()
    {
		currentState = State.GamePlay;
		backCountDownTime = int.MaxValue;
		SoundDirector.ins.SE.Play("GameStart");
		CreateTable();
	}

	/// <summary>
	/// 制限時間が来た時の処理
	/// </summary>
	/// <returns></returns>
	IEnumerator TimeOut()
	{
		currentState = State.TimeOut;

		SettingMenu.ins.OnCloseMenu(); //設定を閉じる
		SoundDirector.ins.SE.Play("TimeOut"); //音を鳴らす
		rankingSystem.UpdatePlayerStatistics(useGameMode, matchCount); //スコアの登録
		gameGUI.gameContent.TimeOutTextPlay(1.5f); //タイムアウトのテキストを描画
		yield return new WaitForSeconds(2.0f); 

		rankingSystem.GetLeaderboardAroundPlayer(RankingSystem.LeaderBoardType.Result, useGameMode); //ランキングの取得
		gameGUI.gameContent.TimeOutTextFade(1.0f); //タイムアウトのテキストを非表示
		yield return new WaitForSeconds(1.0f);

		adsScript.ShowAdsMovie();
		yield return new WaitForSeconds(1.0f);

		//結果を表示
		StartCoroutine(Result());
		yield break;
	}

	/// <summary>
	/// 結果画面の描画
	/// </summary>
	/// <returns></returns>
	IEnumerator Result()
	{
		//GUIをResultに切り替える
		gameGUI.ChangeGUI(GameGUI.SceneType.Result);

		SoundDirector.ins.SE.Play("Result");

		float averageCount = (matchCount == 0 || sumFlipCount == 0) ? 0.0f : (float)sumFlipCount / matchCount; //平均試行回数
		gameGUI.resultContent.matchCountText.text = "揃えた数: " + matchCount + "回";        //揃えた数の描画
		gameGUI.resultContent.averageCountText.text ="平均試行回数: " + averageCount + "回"; //平均試行回数の描画

		gameGUI.resultContent.canvasGroup.transform.DOScaleX(0.0f, 0.0f);
		gameGUI.resultContent.canvasGroup.transform.DOScaleX(1.0f, 0.75f).SetEase(Ease.InSine);
		yield return new WaitForSeconds(1.0f);
		yield break;
	}

	/// <summary>
	/// ゲームが読み込まれた時の処理
	/// </summary>
	/// <param name="gameMode"></param>
	public void StartGame(GameMode gameMode)
	{
		currentState = State.Init;

		//現在のGUIをGameplayに切り替える
		gameGUI.ChangeGUI(GameGUI.SceneType.Gameplay);

		//それぞれ初期化
		Init(gameMode);

		currentState = State.CountDown;
	}


	/// <summary>
	/// 盤面の初期化
	/// </summary>
	void Init(GameMode gameMode)
	{
		useGameMode = gameMode;
		leftTime = limitTime;
		countDownTime = 5.0f;
		backCountDownTime = int.MaxValue;
		currentLimitedCount = maxLimitedCount;

		matchCount = 0;

		gameGUI.gameContent.Init(matchCount, leftTime, limitTime);
		gameGUI.gameContent.SetLabelText(gameMode);
		gameGUI.gameContent.SetLimitedCountText(currentLimitedCount);

		InitTable();
	}

	/// <summary>
	/// 盤面をすべて真っ白に
	/// </summary>
	void InitTable()
    {
		tableData = new bool[gridLength];

		for (int i = 0; i < gridLength; i++)
		{
			tableData[i] = false;
		}

		DrawTable(0.1f);
	}

	/// <summary>
	/// 盤面の生成
	/// </summary>
	void CreateTable()
	{
		tableData = new bool[gridLength];

		int blackColorCount = Random.Range(1, gridLength - 2); //黒を何個作るか

		for(int i = 0; i < gridLength; i++)
		{
			tableData[i] = i <= blackColorCount; 
		}

		tableData = BoolShuffle(tableData); //盤面のシャッフル

		currentLimitedCount = maxLimitedCount; //試行回数のリセット

		DrawTable(0.1f);
	}

	/// <summary>
	/// 盤面の描画
	/// </summary>
	/// <param name="flipTime"></param>
	public void DrawTable(float flipTime)
	{
		//それぞれのImageに色を割り当てる
		for(int i = 0; i < gridLength; i++)
		{
			bool data = tableData[i];
			Color color = data ? Color.black : Color.white;

			if (flipTime > 0.0f) buttonImages[i].DOColor(color, flipTime);
			else buttonImages[i].color = color;
		}

		//ゲームモードがLimitedModeならのこり試行回数を描画
		if (useGameMode == GameMode.Limited) gameGUI.gameContent.SetLimitedCountText(currentLimitedCount);
		else gameGUI.gameContent.limitedText.text = "";
	}

	/// <summary>
	/// 盤面が押されたら
	/// </summary>
	public void PushButton(int index)
	{
		//ゲームプレイ中以外は押せないように
		if (currentState != State.GamePlay) return;

		Vector2Int pos = IndexToVector2Int(index); //2次元の位置
		currentLimitedCount--; //のこり試行回数の減少

		SoundDirector.ins.SE.Play("Flip");

		//自身の反転
		Flip(index);

		//4方向の反転
		for (int dx = -1, dy = 0, i = 0; i < 4; dx += dy, dy = dx - dy, dx = dy - dx, ++i)
		{
			int px = pos.x + dx;
			int py = pos.y + dy;

			if (px < 0 || px >= gridSize) continue;
			if (py < 0 || py >= gridSize) continue;

			Flip(Vector2IntToIndex(px, py));
		}

		//盤面の描画
		DrawTable(0.1f);

		//盤面が揃ったら
		if (CheckMatchTable(true))
		{
			//加点し盤面のリセット
			matchCount++;
			CreateTable();

			InstanceMatchEffect();

			SoundDirector.ins.SE.Play("Match");
			gameGUI.gameContent.SetMatchCountGUI(matchCount);
		}

		//ゲームモードがLimitedModeで試行回数が0になったら盤面リセット
        else if(useGameMode == GameMode.Limited && currentLimitedCount <= 0)
        {
			SoundDirector.ins.SE.Play("Cancel");
			CreateTable();
        }
	}

	/// <summary>
	/// 盤面の反転
	/// </summary>
	void Flip(int index)
	{
		tableData[index] = !tableData[index];
	}

	/// <summary>
	/// 盤面が揃ったか
	/// </summary>
	bool CheckMatchTable(bool useWhite)
	{
		int blackCount = tableData.Where(n => n == true).Count();

		return blackCount >= gridLength || (useWhite && blackCount <= 0);
	}

	/// <summary>
	/// ボタンの処理: 設定を開く
	/// </summary>
	public void OnSetting()
    {
		if (currentState != State.GamePlay) return;
		SoundDirector.ins.SE.Play("Select");
		SettingMenu.ins.OnMenuEnable();
    }

	/// <summary>
	/// ボタンの処理: リスタート
	/// </summary>
	public void OnRetry()
	{
		SoundDirector.ins.SE.Play("Select");
		SettingMenu.ins.OnCloseMenu();
		StartGame(useGameMode);
	}

	/// <summary>
	/// ボタンの処理: タイトルに戻る
	/// </summary>
	public void OnGotoTitle()
    {
		gameGUI.ChangeGUI(GameGUI.SceneType.Title);
		SoundDirector.ins.SE.Play("Select");
		SettingMenu.ins.OnCloseMenu();
		gameGUI.titleContent.TitleSceneChange(TitleContent.TitleScene.MainTitle);
    }

	/// <summary>
	/// 成功エフェクトの生成
	/// </summary>
	void InstanceMatchEffect()
	{
		GameObject ins = Instantiate(matchEffect);
		ins.transform.position = Vector3.zero;

		ins.GetComponent<ParticleSystem>().Play();
	}

	/// <summary>
	/// boolの配列のシャッフル
	/// </summary>
	bool[] BoolShuffle(bool[] data)
	{
		for(int i = 0; i < data.Length; i++)
		{
			int r = Random.Range(i, data.Length - 1);

			bool temp = data[i];
			data[i] = data[r];
			data[r] = temp;
		}

		return data;
	}

	/// <summary>
	/// IndexをVector2Intに変換
	/// </summary>
	Vector2Int IndexToVector2Int(int index)
	{
		return new Vector2Int(index % gridSize, index / gridSize);
	}

	/// <summary>
	/// Vector2IntをIndexに変換
	/// </summary>
	int Vector2IntToIndex(Vector2Int vector)
	{
		return Vector2IntToIndex(vector.x, vector.y);
	}

	/// <summary>
	/// 2次元座標をIndexに変換
	/// </summary>
	int Vector2IntToIndex(int x, int y)
	{
		return x + y * gridSize;
	}
}
