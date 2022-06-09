using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsScript : MonoBehaviour
{
#if UNITY_ANDROID
    const string adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
    const string adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
    const string adUnitId = "unexpected_platform";
#endif

    [SerializeField] bool testMode;

    private BannerView bannerView;

    private void Start()
    {
        MobileAds.Initialize(initStatus => { });

        RequestBanner();
    }

    void RequestBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }

        this.bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
        var request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
    }

    private void OnDestroy()
    {
        // バナー広告を破棄するときは必ずDestoryしないと、メモリリークするようです
        bannerView?.Destroy();
    }
}
