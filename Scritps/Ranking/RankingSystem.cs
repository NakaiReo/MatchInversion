using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingSystem : MonoBehaviour
{
    public PlayFabLogin login;

    [SerializeField] Transform rankingContentParent;
    [SerializeField] Transform resultContentParent;
    [SerializeField] GameObject rankingContent;
    [SerializeField] int getRankingTopLength;
    [SerializeField] int getRankingAroundLength;

    LeaderBoardType currentLeaderBoardType;

    public enum LeaderBoardType
	{
        RankingList,
        Result,
	}

    public delegate void ResultCallBack(string name);

    //=================================================================================
    //���[�U�[�l�[��
    //=================================================================================

    string displayName;
    public string DisplayName
	{
		get
		{
            if (displayName == "") return "������";

            return displayName;
		}
		set
		{
            displayName = value;
		}
	}

    /// <summary>
    /// ���[�U�����X�V����
    /// </summary>
    public void UpdateUserName(string playerName)
    {
        //���[�U�����w�肵�āAUpdateUserTitleDisplayNameRequest�̃C���X�^���X�𐶐�
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = playerName
        };

        //���[�U���̍X�V
        Debug.Log($"���[�U���̍X�V�J�n");
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateUserNameSuccess, OnUpdateUserNameFailure);
    }

    public void GetUserName(ResultCallBack callBack)
    {
        PlayFabClientAPI.GetPlayerProfile(
         new GetPlayerProfileRequest
        {
            PlayFabId = PlayFabSettings.staticPlayer.PlayFabId,
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                ShowDisplayName = true
            }
        },
        result => {
            displayName = result.PlayerProfile.DisplayName;
            Debug.Log($"DisplayName: {displayName}");
            callBack(displayName);
        },
        error => {
            displayName = "����������";
            Debug.Log($"Error: DisplayName: {displayName}");
            callBack(displayName);
        }
    );
    }

    //���[�U���̍X�V����
    private void OnUpdateUserNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        //result.DisplayName�ɍX�V������̃��[�U���������Ă�
        Debug.Log($"���[�U���̍X�V���������܂��� : {result.DisplayName}");
    }

    //���[�U���̍X�V���s
    private void OnUpdateUserNameFailure(PlayFabError error)
    {
        Debug.LogError($"���[�U���̍X�V�Ɏ��s���܂���\n{error.GenerateErrorReport()}");
    }

    //=================================================================================
    //�X�R�A
    //=================================================================================

    /// <summary>
    /// �X�R�A(���v���)���X�V����
    /// </summary>
    public void UpdatePlayerStatistics(GameDirector.GameMode gameMode, int score)
    {
        //UpdatePlayerStatisticsRequest�̃C���X�^���X�𐶐�
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>{
        new StatisticUpdate{
          StatisticName = GetRankName(gameMode),   //�����L���O��(���v���)
          Value = score, //�X�R�A(int)
        }
      }
        };

        //���[�U���̍X�V
        Debug.Log($"�X�R�A(���v���)�̍X�V�J�n");
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnUpdatePlayerStatisticsSuccess, OnUpdatePlayerStatisticsFailure);
    }

    public void GetPlayerScore(GameDirector.GameMode gameMode)
	{

	}

    //�X�R�A(���v���)�̍X�V����
    private void OnUpdatePlayerStatisticsSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log($"�X�R�A(���v���)�̍X�V���������܂���");
    }

    //�X�R�A(���v���)�̍X�V���s
    private void OnUpdatePlayerStatisticsFailure(PlayFabError error)
    {
        Debug.LogError($"�X�R�A(���v���)�X�V�Ɏ��s���܂���\n{error.GenerateErrorReport()}");
    }

    //=================================================================================
    //�����L���O�擾
    //=================================================================================
   
    /// <summary>
    /// �����L���O(���[�_�[�{�[�h)���擾
    /// </summary>
    public void GetLeaderboardTop(LeaderBoardType leaderBoardType, GameDirector.GameMode gameMode)
    {
        currentLeaderBoardType = leaderBoardType;

        InitLeaderBoard(leaderBoardType, getRankingTopLength);

        //GetLeaderboardRequest�̃C���X�^���X�𐶐�
        var request = new GetLeaderboardRequest
        {
            StatisticName = GetRankName(gameMode), //�����L���O��(���v���)
            StartPosition = 0,                     //���ʈȍ~�̃����L���O���擾���邩
            MaxResultsCount = getRankingTopLength  //�����L���O�f�[�^�������擾���邩(�ő�100)
        };

        //�����L���O(���[�_�[�{�[�h)���擾
        Debug.Log($"�����L���O(���[�_�[�{�[�h" + gameMode + ")�̎擾�J�n");
        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnGetLeaderboardFailure);
    }

    /// <summary>
    /// �����L���O(���[�_�[�{�[�h)���擾
    /// </summary>
    public void GetLeaderboardAroundPlayer(LeaderBoardType leaderBoardType, GameDirector.GameMode gameMode)
    {
        currentLeaderBoardType = leaderBoardType;

        InitLeaderBoard(leaderBoardType, getRankingAroundLength);

        //GetLeaderboardRequest�̃C���X�^���X�𐶐�
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = GetRankName(gameMode),       //�����L���O��(���v���)
            MaxResultsCount = getRankingAroundLength     //�����L���O�f�[�^�������擾���邩(�ő�100)
        };

        //�����L���O(���[�_�[�{�[�h)���擾
        Debug.Log($"�����L���O(���[�_�[�{�[�h" + gameMode + ")�̎擾�J�n");
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnGetLeaderboardAroundPlayerSuccess, OnGetLeaderboardAroundPlayerFailure);
    }

    //�����L���O(���[�_�[�{�[�h)�̎擾����
    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        Debug.Log($"�����L���O(���[�_�[�{�[�h)�̎擾�ɐ������܂���");

        List<RankingContentData> rankingContentDatas = new List<RankingContentData>();

        //result.Leaderboard�Ɋe���ʂ̏��(PlayerLeaderboardEntry)�������Ă���
        foreach (var entry in result.Leaderboard)
        {
            int rank = entry.Position + 1;
            string playerName = entry.DisplayName;
            int score = entry.StatValue;

            rankingContentDatas.Add(new RankingContentData(rank, playerName, score));
        }

        SetLeaderBoard(rankingContentDatas);
    }

    private void OnGetLeaderboardAroundPlayerSuccess(GetLeaderboardAroundPlayerResult result)
    {
        Debug.Log($"�����̏��ʎ��ӂ̃����L���O(���[�_�[�{�[�h)�̎擾�ɐ������܂���");

        List<RankingContentData> rankingContentDatas = new List<RankingContentData>();

        //result.Leaderboard�Ɋe���ʂ̏��(PlayerLeaderboardEntry)�������Ă���
        foreach (var entry in result.Leaderboard)
        {
            int rank = entry.Position + 1;
            string playerName = entry.DisplayName;
            int score = entry.StatValue;

            rankingContentDatas.Add(new RankingContentData(rank, playerName, score));
        }

        SetLeaderBoard(rankingContentDatas);
    }

    //�����L���O(���[�_�[�{�[�h)�̎擾���s
    private void OnGetLeaderboardFailure(PlayFabError error)
    {
        Debug.LogError($"�����L���O(���[�_�[�{�[�h)�̎擾�Ɏ��s���܂���\n{error.GenerateErrorReport()}");
    }

    //�����̏��ʎ��ӂ̃����L���O(���[�_�[�{�[�h)�̎擾���s
    private void OnGetLeaderboardAroundPlayerFailure(PlayFabError error)
    {
        Debug.LogError($"�����̏��ʎ��ӂ̃����L���O(���[�_�[�{�[�h)�̎擾�Ɏ��s���܂���\n{error.GenerateErrorReport()}");
    }

    private void InitLeaderBoard(LeaderBoardType leaderBoardType, int contentSize)
    {
        Transform parent = GetLeaderBoardParent(leaderBoardType);

        if (parent.childCount > 0)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        for(int i = 0; i < contentSize; i++)
        {
            GameObject ins = Instantiate(rankingContent);
            ins.transform.SetParent(parent, false);

            ins.GetComponent<RankingContentGUI>().InitGUI();
        }
    }

    private void SetLeaderBoard(List<RankingContentData> datas)
    {
        Debug.Log(currentLeaderBoardType + ", " + datas.Count);

        Transform parent = GetLeaderBoardParent(currentLeaderBoardType);

        for(int i = 0; i < datas.Count; i++)
        {
            RankingContentGUI gui = parent.GetChild(i).GetComponent<RankingContentGUI>();

            gui.SetGUI(datas[i]);
        }
    }

    Transform GetLeaderBoardParent(LeaderBoardType leaderBoardType) => leaderBoardType switch
    {
        LeaderBoardType.RankingList => rankingContentParent,
        LeaderBoardType.Result => resultContentParent,
        _ => null,
    };

    //=================================================================================
    //���ʋ@�\
    //=================================================================================

    //�Q�[�����[�h���Ƃ̃����L���O�̖��O���擾
    string GetRankName(GameDirector.GameMode gameMode) => gameMode switch
    {
        GameDirector.GameMode.Normal => "NormalModeRanking",
        GameDirector.GameMode.Limited => "LimitedModeRanking",
        _ => "�����ȃQ�[���R�[�h"
    };
}
