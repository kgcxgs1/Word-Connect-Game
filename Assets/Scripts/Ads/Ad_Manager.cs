using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.Ump.Api;
using GoogleMobileAds.Sample;

public class Ad_Manager : MonoBehaviour
{

    BannerViewController bannerViewControllerScript;
    InterstitialAdController interstitialAdControllerScript;
    RewardedAdController rewardedAdControllerScript;

    private void Awake() 
    {
        bannerViewControllerScript = FindObjectOfType<BannerViewController>();
        interstitialAdControllerScript = FindObjectOfType<InterstitialAdController>();
        rewardedAdControllerScript = FindObjectOfType<RewardedAdController>();

        interstitialAdControllerScript.LoadAd();
        rewardedAdControllerScript.LoadAd();
    }

    private void Start() 
    {
        
    }

    public void BannerAdLoad() {    bannerViewControllerScript.LoadAd();    }

    public void BannerAdShow() {    bannerViewControllerScript.ShowAd();    }

    public void BannerAdHide() {    bannerViewControllerScript.HideAd();    }


    public IEnumerator InterstitialAdLoad()
    {  
        yield return new WaitForSeconds(0.01f);

        // while (true)
        // {
        //     if (interstitialAdControllerScript._interstitialAd == null)
        //     {
        //         Debug.Log("Loading");
        //         interstitialAdControllerScript.LoadAd();
        //     }else
        //     {
        //         Debug.Log("previous loaded");
        //     }

        //     yield return new WaitForSeconds(5f);
        // }   
    }
    public IEnumerator InterstitialAdShow(float delay) {   
        yield return new WaitForSeconds(0.01f);
        yield return new WaitForSeconds(delay);
        interstitialAdControllerScript.ShowAd();    }
    public void InterstitialAdDestroy() {  interstitialAdControllerScript.DestroyAd();    }


    public void RewardedAdLoad() {  rewardedAdControllerScript.LoadAd();    }
    public IEnumerator RewardedAdShow(float delay) {   
        yield return new WaitForSeconds(0.01f);
        yield return new WaitForSeconds(delay);
        rewardedAdControllerScript.ShowAd();    }
    public void RewardedAdDestroy() {  rewardedAdControllerScript.DestroyAd();    }


}
