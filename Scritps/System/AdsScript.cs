using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

/// <summary>
/// UnityAdsの表示
/// </summary>
public class AdsScript : MonoBehaviour
{
    public string gameId = "";
    public string bannerId = "Banner";
    public string interstitialID = "Interstitial";
    public bool testMode = true;

    void Start()
    {
        //広告の初期化
        Advertisement.Initialize(gameId, testMode);
        StartCoroutine(showBanner());
    }

    IEnumerator showBanner()
    {
        //初期化が終わるまで待つ
        while (!Advertisement.isInitialized)
        {
            yield return new WaitForSeconds(0.5f); // 0.5秒後に広告表示
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER); //バナーを上部中央にセット
        Advertisement.Banner.Show(bannerId); //バナー表示
    }

    /// <summary>
    /// Movie広告を表示
    /// </summary>
    public void ShowAdsMovie()
    {
        // 初期化に失敗していたら、再生しない
        if (!Advertisement.isInitialized) return;

        if (Advertisement.IsReady(interstitialID))
        {
            // 広告表示後のオプション設定
            var options = new ShowOptions
            {
                resultCallback = (result) =>
                {
                    switch (result)
                    {
                        case ShowResult.Finished: // 最後まで正常に再生
                            Debug.Log("The Ads was successfully shown.");
                            break;
                        case ShowResult.Skipped: // 途中でスキップされた
                            Debug.Log("The Ads was skipped.");
                            break;
                        case ShowResult.Failed: // 再生に失敗した
                            Debug.Log("The Ads failed to be shown.");
                            break;
                    }
                }
            };

            Advertisement.Show(interstitialID, options);
        }
    }
}
