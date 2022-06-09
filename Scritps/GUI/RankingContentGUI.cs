using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// ランキングの要素の構造体
/// </summary>
public struct RankingContentData
{
    public int rank { get; private set; } //順位
    public string playerName { get; private set; } //プレイヤーの名前
    public int score { get; private set; } //スコア

    /// <summary>
    /// 初期化
    /// </summary>
    public RankingContentData(int rank, string playerName, int score)
    {
        this.rank = rank;
        this.playerName = playerName;
        this.score = score;
    }
}


public class RankingContentGUI : MonoBehaviour
{
    [SerializeField] TMP_Text rankText;
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] TMP_Text scoreText;

    /// <summary>
    /// ランキングのコンテンツの描画
    /// </summary>
    public void SetGUI(RankingContentData data)
    {
        rankText.text = string.Format("{0,3}", data.rank) + "位";
        playerNameText.text = data.playerName;
        scoreText.text = data.score + "回";
    }

    /// <summary>
    /// ランキングのコンテンツの描画
    /// </summary>
    public void SetGUI(int rank, string playerName, int score)
    {
        SetGUI(new RankingContentData(rank, playerName, score));
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void InitGUI()
	{
        rankText.text = "";
        playerNameText.text = "";
        scoreText.text = "";
    }
}
