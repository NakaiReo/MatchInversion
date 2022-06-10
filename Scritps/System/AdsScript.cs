using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

/// <summary>
/// UnityAds�̕\��
/// </summary>
public class AdsScript : MonoBehaviour
{
    public string gameId = "";
    public string bannerId = "Banner";
    public string interstitialID = "Interstitial";
    public bool testMode = true;

    void Start()
    {
        //�L���̏�����
        Advertisement.Initialize(gameId, testMode);
        StartCoroutine(showBanner());
    }

    IEnumerator showBanner()
    {
        //���������I���܂ő҂�
        while (!Advertisement.isInitialized)
        {
            yield return new WaitForSeconds(0.5f); // 0.5�b��ɍL���\��
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER); //�o�i�[���㕔�����ɃZ�b�g
        Advertisement.Banner.Show(bannerId); //�o�i�[�\��
    }

    /// <summary>
    /// Movie�L����\��
    /// </summary>
    public void ShowAdsMovie()
    {
        // �������Ɏ��s���Ă�����A�Đ����Ȃ�
        if (!Advertisement.isInitialized) return;

        if (Advertisement.IsReady(interstitialID))
        {
            // �L���\����̃I�v�V�����ݒ�
            var options = new ShowOptions
            {
                resultCallback = (result) =>
                {
                    switch (result)
                    {
                        case ShowResult.Finished: // �Ō�܂Ő���ɍĐ�
                            Debug.Log("The Ads was successfully shown.");
                            break;
                        case ShowResult.Skipped: // �r���ŃX�L�b�v���ꂽ
                            Debug.Log("The Ads was skipped.");
                            break;
                        case ShowResult.Failed: // �Đ��Ɏ��s����
                            Debug.Log("The Ads failed to be shown.");
                            break;
                    }
                }
            };

            Advertisement.Show(interstitialID, options);
        }
    }
}
