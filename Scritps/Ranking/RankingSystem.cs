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
    //ユーザーネーム
    //=================================================================================

    string displayName;
    public string DisplayName
	{
		get
		{
            if (displayName == "") return "名無し";

            return displayName;
		}
		set
		{
            displayName = value;
		}
	}

    /// <summary>
    /// ユーザ名を更新する
    /// </summary>
    public void UpdateUserName(string playerName)
    {
        //ユーザ名を指定して、UpdateUserTitleDisplayNameRequestのインスタンスを生成
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = playerName
        };

        //ユーザ名の更新
        Debug.Log($"ユーザ名の更新開始");
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
            displayName = "名無しさん";
            Debug.Log($"Error: DisplayName: {displayName}");
            callBack(displayName);
        }
    );
    }

    //ユーザ名の更新成功
    private void OnUpdateUserNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        //result.DisplayNameに更新した後のユーザ名が入ってる
        Debug.Log($"ユーザ名の更新が成功しました : {result.DisplayName}");
    }

    //ユーザ名の更新失敗
    private void OnUpdateUserNameFailure(PlayFabError error)
    {
        Debug.LogError($"ユーザ名の更新に失敗しました\n{error.GenerateErrorReport()}");
    }

    //=================================================================================
    //スコア
    //=================================================================================

    /// <summary>
    /// スコア(統計情報)を更新する
    /// </summary>
    public void UpdatePlayerStatistics(GameDirector.GameMode gameMode, int score)
    {
        //UpdatePlayerStatisticsRequestのインスタンスを生成
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>{
        new StatisticUpdate{
          StatisticName = GetRankName(gameMode),   //ランキング名(統計情報名)
          Value = score, //スコア(int)
        }
      }
        };

        //ユーザ名の更新
        Debug.Log($"スコア(統計情報)の更新開始");
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnUpdatePlayerStatisticsSuccess, OnUpdatePlayerStatisticsFailure);
    }

    public void GetPlayerScore(GameDirector.GameMode gameMode)
	{

	}

    //スコア(統計情報)の更新成功
    private void OnUpdatePlayerStatisticsSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log($"スコア(統計情報)の更新が成功しました");
    }

    //スコア(統計情報)の更新失敗
    private void OnUpdatePlayerStatisticsFailure(PlayFabError error)
    {
        Debug.LogError($"スコア(統計情報)更新に失敗しました\n{error.GenerateErrorReport()}");
    }

    //=================================================================================
    //ランキング取得
    //=================================================================================
   
    /// <summary>
    /// ランキング(リーダーボード)を取得
    /// </summary>
    public void GetLeaderboardTop(LeaderBoardType leaderBoardType, GameDirector.GameMode gameMode)
    {
        currentLeaderBoardType = leaderBoardType;

        InitLeaderBoard(leaderBoardType, getRankingTopLength);

        //GetLeaderboardRequestのインスタンスを生成
        var request = new GetLeaderboardRequest
        {
            StatisticName = GetRankName(gameMode), //ランキング名(統計情報名)
            StartPosition = 0,                     //何位以降のランキングを取得するか
            MaxResultsCount = getRankingTopLength  //ランキングデータを何件取得するか(最大100)
        };

        //ランキング(リーダーボード)を取得
        Debug.Log($"ランキング(リーダーボード" + gameMode + ")の取得開始");
        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnGetLeaderboardFailure);
    }

    /// <summary>
    /// ランキング(リーダーボード)を取得
    /// </summary>
    public void GetLeaderboardAroundPlayer(LeaderBoardType leaderBoardType, GameDirector.GameMode gameMode)
    {
        currentLeaderBoardType = leaderBoardType;

        InitLeaderBoard(leaderBoardType, getRankingAroundLength);

        //GetLeaderboardRequestのインスタンスを生成
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = GetRankName(gameMode),       //ランキング名(統計情報名)
            MaxResultsCount = getRankingAroundLength     //ランキングデータを何件取得するか(最大100)
        };

        //ランキング(リーダーボード)を取得
        Debug.Log($"ランキング(リーダーボード" + gameMode + ")の取得開始");
        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnGetLeaderboardAroundPlayerSuccess, OnGetLeaderboardAroundPlayerFailure);
    }

    //ランキング(リーダーボード)の取得成功
    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        Debug.Log($"ランキング(リーダーボード)の取得に成功しました");

        List<RankingContentData> rankingContentDatas = new List<RankingContentData>();

        //result.Leaderboardに各順位の情報(PlayerLeaderboardEntry)が入っている
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
        Debug.Log($"自分の順位周辺のランキング(リーダーボード)の取得に成功しました");

        List<RankingContentData> rankingContentDatas = new List<RankingContentData>();

        //result.Leaderboardに各順位の情報(PlayerLeaderboardEntry)が入っている
        foreach (var entry in result.Leaderboard)
        {
            int rank = entry.Position + 1;
            string playerName = entry.DisplayName;
            int score = entry.StatValue;

            rankingContentDatas.Add(new RankingContentData(rank, playerName, score));
        }

        SetLeaderBoard(rankingContentDatas);
    }

    //ランキング(リーダーボード)の取得失敗
    private void OnGetLeaderboardFailure(PlayFabError error)
    {
        Debug.LogError($"ランキング(リーダーボード)の取得に失敗しました\n{error.GenerateErrorReport()}");
    }

    //自分の順位周辺のランキング(リーダーボード)の取得失敗
    private void OnGetLeaderboardAroundPlayerFailure(PlayFabError error)
    {
        Debug.LogError($"自分の順位周辺のランキング(リーダーボード)の取得に失敗しました\n{error.GenerateErrorReport()}");
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
    //共通機能
    //=================================================================================

    //ゲームモードごとのランキングの名前を取得
    string GetRankName(GameDirector.GameMode gameMode) => gameMode switch
    {
        GameDirector.GameMode.Normal => "NormalModeRanking",
        GameDirector.GameMode.Limited => "LimitedModeRanking",
        _ => "無効なゲームコード"
    };
}
