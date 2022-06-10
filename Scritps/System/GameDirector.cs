using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameDirector : MonoBehaviour
{
	[SerializeField] GameGUI gameGUI; //GUI���X�V����N���X
	[SerializeField] RankingSystem rankingSystem; //�����L���O���Ǘ�����N���X
	[SerializeField] AdsScript adsScript; //�L���̃N���X
	[SerializeField] Transform tableParent;  //�Ֆʂ̐eTransform
	[SerializeField] GameObject matchEffect; //�Ֆʂ����������̃G�t�F�N�g

	[Space]
	[SerializeField] float limitTime; float leftTime; //�c�莞��
	[SerializeField] int maxLimitedCount; int currentLimitedCount; //�c�莎�s��
	[SerializeField] int gridSize; //�Ֆʂ̃T�C�Y

	/// <summary>
	/// �Q�[�����[�h
	/// </summary>
	public enum GameMode
    {
		Normal,
		Limited,
    }

	public static int GameModeLength = 2; //�Q�[�����[�h�̐�

	public GameMode useGameMode { get; private set; } //���ݎg�p����Ă���Q�[�����[�h

	int sumFlipCount; //���s��
	int matchCount;   //������ꂽ��
	float countDownTime; int backCountDownTime; //�J�E���g�_�E���p�̕ϐ�

	int gridLength { get { return gridSize * gridSize; } } //�Ֆʂ̑���
	bool[] tableData; //���݂̔Ֆʂ̏��
	Image[] buttonImages; //�Ֆʂ�Image

	public enum State
	{
		Init,      //������
		CountDown, //�J�E���g�_�E��
		GamePlay,  //�Q�[����
		TimeOut,   //���Ԑ؂�
		Result,    //���ʂ̕\��
	}
	public State currentState; //�Q�[���̐i�s���

	private void Awake()
	{
		//�Ֆʂ̏�����
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
		//���݂̃Q�[���̐i�s��Ԃɂ��Update
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
	/// �J�E���g�_�E���p��Update
	/// </summary>
	void UpdateCountDown()
	{
		//�J�E���g�_�E����0�ɂȂ�����Q�[�����J�n
		if(countDownTime <= 0)
		{
			StartGamePlay();
			return;
		}

		//1�b���ƂɃe�L�X�g�̍X�V
		{
			int ceilTime = Mathf.CeilToInt(countDownTime);

			//�c�莞�Ԃ�3�b�ȏ�Ȃ�\�����Ȃ�
			if (countDownTime >= 3.0f)
			{
				gameGUI.gameContent.countDownText.Init();
			}

			//1�b���ƂɃe�L�X�g��ς���
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
	/// �Q�[�����p��Update
	/// </summary>
	void UpdateGamePlay()
	{
		//�c�莞�Ԃ�0�ȉ��Ȃ�Q�[�����I������
		if (leftTime <= 0)
		{
			StartCoroutine(TimeOut());
		}

		//�c�莞�Ԃ�5�b�ȉ��̎��A1�b���Ƃɉ���炷
		int ceilTime = Mathf.CeilToInt(countDownTime);
		if (ceilTime < backCountDownTime && ceilTime <= 5.0f)
		{
			backCountDownTime = ceilTime;
			SoundDirector.ins.SE.Play("LowCountDown");
		}

		//�c�莞�Ԃ̕`��
		gameGUI.gameContent.SetTimeGUI(leftTime, limitTime);
		leftTime -= Time.deltaTime;
	}

	/// <summary>
	/// �Q�[���̊J�n
	/// </summary>
	void StartGamePlay()
    {
		currentState = State.GamePlay;
		backCountDownTime = int.MaxValue;
		SoundDirector.ins.SE.Play("GameStart");
		CreateTable();
	}

	/// <summary>
	/// �������Ԃ��������̏���
	/// </summary>
	/// <returns></returns>
	IEnumerator TimeOut()
	{
		currentState = State.TimeOut;

		SettingMenu.ins.OnCloseMenu(); //�ݒ�����
		SoundDirector.ins.SE.Play("TimeOut"); //����炷
		rankingSystem.UpdatePlayerStatistics(useGameMode, matchCount); //�X�R�A�̓o�^
		gameGUI.gameContent.TimeOutTextPlay(1.5f); //�^�C���A�E�g�̃e�L�X�g��`��
		yield return new WaitForSeconds(2.0f); 

		rankingSystem.GetLeaderboardAroundPlayer(RankingSystem.LeaderBoardType.Result, useGameMode); //�����L���O�̎擾
		gameGUI.gameContent.TimeOutTextFade(1.0f); //�^�C���A�E�g�̃e�L�X�g���\��
		yield return new WaitForSeconds(1.0f);

		adsScript.ShowAdsMovie();
		yield return new WaitForSeconds(1.0f);

		//���ʂ�\��
		StartCoroutine(Result());
		yield break;
	}

	/// <summary>
	/// ���ʉ�ʂ̕`��
	/// </summary>
	/// <returns></returns>
	IEnumerator Result()
	{
		//GUI��Result�ɐ؂�ւ���
		gameGUI.ChangeGUI(GameGUI.SceneType.Result);

		SoundDirector.ins.SE.Play("Result");

		float averageCount = (matchCount == 0 || sumFlipCount == 0) ? 0.0f : (float)sumFlipCount / matchCount; //���ώ��s��
		gameGUI.resultContent.matchCountText.text = "��������: " + matchCount + "��";        //���������̕`��
		gameGUI.resultContent.averageCountText.text ="���ώ��s��: " + averageCount + "��"; //���ώ��s�񐔂̕`��

		gameGUI.resultContent.canvasGroup.transform.DOScaleX(0.0f, 0.0f);
		gameGUI.resultContent.canvasGroup.transform.DOScaleX(1.0f, 0.75f).SetEase(Ease.InSine);
		yield return new WaitForSeconds(1.0f);
		yield break;
	}

	/// <summary>
	/// �Q�[�����ǂݍ��܂ꂽ���̏���
	/// </summary>
	/// <param name="gameMode"></param>
	public void StartGame(GameMode gameMode)
	{
		currentState = State.Init;

		//���݂�GUI��Gameplay�ɐ؂�ւ���
		gameGUI.ChangeGUI(GameGUI.SceneType.Gameplay);

		//���ꂼ�ꏉ����
		Init(gameMode);

		currentState = State.CountDown;
	}


	/// <summary>
	/// �Ֆʂ̏�����
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
	/// �Ֆʂ����ׂĐ^������
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
	/// �Ֆʂ̐���
	/// </summary>
	void CreateTable()
	{
		tableData = new bool[gridLength];

		int blackColorCount = Random.Range(1, gridLength - 2); //��������邩

		for(int i = 0; i < gridLength; i++)
		{
			tableData[i] = i <= blackColorCount; 
		}

		tableData = BoolShuffle(tableData); //�Ֆʂ̃V���b�t��

		currentLimitedCount = maxLimitedCount; //���s�񐔂̃��Z�b�g

		DrawTable(0.1f);
	}

	/// <summary>
	/// �Ֆʂ̕`��
	/// </summary>
	/// <param name="flipTime"></param>
	public void DrawTable(float flipTime)
	{
		//���ꂼ���Image�ɐF�����蓖�Ă�
		for(int i = 0; i < gridLength; i++)
		{
			bool data = tableData[i];
			Color color = data ? Color.black : Color.white;

			if (flipTime > 0.0f) buttonImages[i].DOColor(color, flipTime);
			else buttonImages[i].color = color;
		}

		//�Q�[�����[�h��LimitedMode�Ȃ�̂��莎�s�񐔂�`��
		if (useGameMode == GameMode.Limited) gameGUI.gameContent.SetLimitedCountText(currentLimitedCount);
		else gameGUI.gameContent.limitedText.text = "";
	}

	/// <summary>
	/// �Ֆʂ������ꂽ��
	/// </summary>
	public void PushButton(int index)
	{
		//�Q�[���v���C���ȊO�͉����Ȃ��悤��
		if (currentState != State.GamePlay) return;

		Vector2Int pos = IndexToVector2Int(index); //2�����̈ʒu
		currentLimitedCount--; //�̂��莎�s�񐔂̌���

		SoundDirector.ins.SE.Play("Flip");

		//���g�̔��]
		Flip(index);

		//4�����̔��]
		for (int dx = -1, dy = 0, i = 0; i < 4; dx += dy, dy = dx - dy, dx = dy - dx, ++i)
		{
			int px = pos.x + dx;
			int py = pos.y + dy;

			if (px < 0 || px >= gridSize) continue;
			if (py < 0 || py >= gridSize) continue;

			Flip(Vector2IntToIndex(px, py));
		}

		//�Ֆʂ̕`��
		DrawTable(0.1f);

		//�Ֆʂ���������
		if (CheckMatchTable(true))
		{
			//���_���Ֆʂ̃��Z�b�g
			matchCount++;
			CreateTable();

			InstanceMatchEffect();

			SoundDirector.ins.SE.Play("Match");
			gameGUI.gameContent.SetMatchCountGUI(matchCount);
		}

		//�Q�[�����[�h��LimitedMode�Ŏ��s�񐔂�0�ɂȂ�����Ֆʃ��Z�b�g
        else if(useGameMode == GameMode.Limited && currentLimitedCount <= 0)
        {
			SoundDirector.ins.SE.Play("Cancel");
			CreateTable();
        }
	}

	/// <summary>
	/// �Ֆʂ̔��]
	/// </summary>
	void Flip(int index)
	{
		tableData[index] = !tableData[index];
	}

	/// <summary>
	/// �Ֆʂ���������
	/// </summary>
	bool CheckMatchTable(bool useWhite)
	{
		int blackCount = tableData.Where(n => n == true).Count();

		return blackCount >= gridLength || (useWhite && blackCount <= 0);
	}

	/// <summary>
	/// �{�^���̏���: �ݒ���J��
	/// </summary>
	public void OnSetting()
    {
		if (currentState != State.GamePlay) return;
		SoundDirector.ins.SE.Play("Select");
		SettingMenu.ins.OnMenuEnable();
    }

	/// <summary>
	/// �{�^���̏���: ���X�^�[�g
	/// </summary>
	public void OnRetry()
	{
		SoundDirector.ins.SE.Play("Select");
		SettingMenu.ins.OnCloseMenu();
		StartGame(useGameMode);
	}

	/// <summary>
	/// �{�^���̏���: �^�C�g���ɖ߂�
	/// </summary>
	public void OnGotoTitle()
    {
		gameGUI.ChangeGUI(GameGUI.SceneType.Title);
		SoundDirector.ins.SE.Play("Select");
		SettingMenu.ins.OnCloseMenu();
		gameGUI.titleContent.TitleSceneChange(TitleContent.TitleScene.MainTitle);
    }

	/// <summary>
	/// �����G�t�F�N�g�̐���
	/// </summary>
	void InstanceMatchEffect()
	{
		GameObject ins = Instantiate(matchEffect);
		ins.transform.position = Vector3.zero;

		ins.GetComponent<ParticleSystem>().Play();
	}

	/// <summary>
	/// bool�̔z��̃V���b�t��
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
	/// Index��Vector2Int�ɕϊ�
	/// </summary>
	Vector2Int IndexToVector2Int(int index)
	{
		return new Vector2Int(index % gridSize, index / gridSize);
	}

	/// <summary>
	/// Vector2Int��Index�ɕϊ�
	/// </summary>
	int Vector2IntToIndex(Vector2Int vector)
	{
		return Vector2IntToIndex(vector.x, vector.y);
	}

	/// <summary>
	/// 2�������W��Index�ɕϊ�
	/// </summary>
	int Vector2IntToIndex(int x, int y)
	{
		return x + y * gridSize;
	}
}
