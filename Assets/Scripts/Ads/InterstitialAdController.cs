using System;
using UnityEngine;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Sample
{
    public class InterstitialAdController : MonoBehaviour
    {
        private const string _adUnitId =
#if UNITY_ANDROID
            "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
            "ca-app-pub-3940256099942544/4411468910";
#else
            "unused";
#endif

        private InterstitialAd _interstitialAd;
        private bool _hasAdBeenShown = false;

        public void LoadAd()
        {
            if (_interstitialAd != null)
            {
                DestroyAd();
            }

            Debug.Log("Loading interstitial ad.");

            var adRequest = new AdRequest();
            InterstitialAd.Load(_adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Interstitial ad failed to load with error: " + error);
                    return;
                }

                if (ad == null)
                {
                    Debug.LogError("Unexpected error: Interstitial load event fired with null ad and null error.");
                    return;
                }

                Debug.Log("Interstitial ad loaded with response: " + ad.GetResponseInfo());
                _interstitialAd = ad;

                RegisterEventHandlers(ad);

                // Show the ad immediately if it hasn't been shown yet
                if (!_hasAdBeenShown)
                {
                    Invoke("LoadAd", 1f);
                    _hasAdBeenShown = true;  // Mark that the ad has been shown
                }
            });
        }

        public void ShowAd()
        {
            if (_interstitialAd != null && _interstitialAd.CanShowAd())
            {
                Debug.Log("Showing interstitial ad.");
                _interstitialAd.Show();
            }
            else
            {
                Debug.LogError("Interstitial ad is not ready yet.");
            }
        }

        public void DestroyAd()
        {
            if (_interstitialAd != null)
            {
                Debug.Log("Destroying interstitial ad.");
                _interstitialAd.Destroy();
                _interstitialAd = null;
            }
        }

        private void RegisterEventHandlers(InterstitialAd ad)
        {
            ad.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Interstitial ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
            };

            ad.OnAdImpressionRecorded += () =>
            {
                Debug.Log("Interstitial ad recorded an impression.");
            };

            ad.OnAdClicked += () =>
            {
                Debug.Log("Interstitial ad was clicked.");
            };

            ad.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log("Interstitial ad full screen content opened.");
            };

            ad.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial ad full screen content closed.");
                DestroyAd();

                Invoke("LoadAd", 1f);
            };

            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.LogError("Interstitial ad failed to open full screen content with error: " + error);
            };
        }
    }
}
