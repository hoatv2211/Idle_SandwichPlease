using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppLovinMax;
using System;
using DG.Tweening;

public class AdsAppLovinController : Singleton<AdsAppLovinController>
{
    [SerializeField] AppOpenManager appOpenManager;
    [SerializeField] BannerManager bannerManager;
    [SerializeField] InterstitialsManager interManager;
    [SerializeField] NativeManager nativeManager;
    [SerializeField] RewardManager rewardManager;
    void Start()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += OnAppOpenDisplayedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAdAppOpenLoadedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAdAppOpenLoadFailedEvent;
            MaxSdkCallbacks.AppOpen.OnAdClickedEvent += OnAdAppOpenClickedEvent;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAdAppOpenRevenuePaidEvent;

            InitializeBannerAds();
            InitializeInterstitialAds();
            InitializeRewardedAds();
            RequestOpenApp();

            DOVirtual.DelayedCall(0.5f, ShowAdOpenIfReady);
        };

        MaxSdk.SetSdkKey("mIBpLPUzcny53ZySmB1dBmrslD0LupuSDa7EUwfyJhnWaqa7RpMuKBs_NJX3yQl026hm6dbAql0f8r4V274t6r");
        MaxSdk.InitializeSdk();

        //InitializeMRecAds();
    }

    private void OnAdAppOpenRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        TrackAdRevenue(adInfo);
    }
    #region Ads Open App
    private void OnAdAppOpenClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        FirebaseManager.Instance.LogEvent_ads_open_show();
        AdjustTracking.Instance.Event_ads_open_show();
        //AppsFlyerTracking.Instance.LogEvent_ads_open_show();
    }
    private void OnAdAppOpenLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // AppOpen ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 16 seconds)
        FirebaseManager.Instance.LogEvent_ads_open_failed(errorInfo);
        AdjustTracking.Instance.Event_ads_open_failed();
        //AppsFlyerTracking.Instance.LogEvent_ads_open_failed(errorInfo);
        interManager.retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(4, interManager.retryAttempt));
        Invoke("LoadOpenApp", (float)retryDelay);
    }
    private void OnAdAppOpenLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        FirebaseManager.Instance.LogEvent_ads_open_request_complete();
        //AdjustTracking.Instance.Event_ads_open_complete();
        //AppsFlyerTracking.Instance.LogEvent_ads_open_request_complete();
        interManager.retryAttempt = 0;
    }
    private void OnAppOpenDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        FirebaseManager.Instance.LogEvent_ads_open_complete(adInfo);
         AdjustTracking.Instance.Event_ads_open_complete(adInfo);
        //AppsFlyerTracking.Instance.LogEvent_ads_open_complete(adInfo);
    }
    public void OnAppOpenDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        RequestOpenApp();
    }
    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            ShowAdOpenIfReady();
        }
    }
    public void RequestOpenApp()
    {
        MaxSdk.LoadAppOpenAd(appOpenManager.AppOpenAdUnitId);
        FirebaseManager.Instance.LogEvent_ads_open_request();
        AdjustTracking.Instance.Event_ads_open_request();
        //AppsFlyerTracking.Instance.LogEvent_ads_open_request();
    }
    public void ShowAdOpenIfReady()
    {
        if (Module.isRemoveAds!=0)
        {
            Debug.Log("is Remove Ads");
            return;
        }
        if (MaxSdk.IsAppOpenAdReady(appOpenManager.AppOpenAdUnitId))
        {
            MaxSdk.ShowAppOpenAd(appOpenManager.AppOpenAdUnitId);
            Module.isShowOpenapp = false;
        }
        else
        {
            Debug.Log("Ad AppOpen is not ready");
        }
    }
    #endregion
    #region Ads Banner
    public void InitializeBannerAds()
    {
        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(bannerManager.bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(bannerManager.bannerAdUnitId, Color.clear);

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        FirebaseManager.Instance.LogEvent_ads_banner_request();
        //AppsFlyerTracking.Instance.LogEvent_ads_banner_request();
       
    }

    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) { }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        FirebaseManager.Instance.LogEvent_ads_banner_click();
        AdjustTracking.Instance.Event_ads_banner_click();
        //AppsFlyerTracking.Instance.LogEvent_ads_banner_click();
    }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        TrackAdRevenue(adInfo);
    }

    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    public void ShowBanner()
    {
        if (Module.isTest)
            return;

        MaxSdk.ShowBanner(bannerManager.bannerAdUnitId);
    }
    public void HideBanner()
    {
        MaxSdk.DestroyBanner(bannerManager.bannerAdUnitId);
    }
    #endregion
    #region Ads Inter
    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += Interstitial_OnAdRevenuePaidEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

        // Load the first interstitial
        LoadInterstitial();
    }

    private void Interstitial_OnAdRevenuePaidEvent(string arg1, MaxSdkBase.AdInfo adInfo)
    {
        TrackAdRevenue(adInfo);
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(interManager.adUnitId);
        FirebaseManager.Instance.LogEvent_ads_inter_request();
        AdjustTracking.Instance.Event_ads_inter_request();
        //AppsFlyerTracking.Instance.LogEvent_ads_inter_request();
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'
        // Reset retry attempt
        FirebaseManager.Instance.LogEvent_ads_inter_request_complete();
        AdjustTracking.Instance.Event_ads_inter_complete();
        //AppsFlyerTracking.Instance.LogEvent_ads_inter_request_complete();
        interManager.retryAttempt = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 16 seconds)
        FirebaseManager.Instance.LogEvent_ads_inter_failed(errorInfo);
        AdjustTracking.Instance.Event_ads_inter_failed();
        //AppsFlyerTracking.Instance.LogEvent_ads_inter_failed(errorInfo);
        interManager.retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(4, interManager.retryAttempt));
        Invoke(nameof(LoadInterstitial), (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

        FirebaseManager.Instance.LogEvent_ads_inter_show();
        AdjustTracking.Instance.Event_ads_inter_show();

        FirebaseManager.Instance.LogEvent_Ads_Complete();
        FirebaseManager.Instance.LogEvent_Ads_Inter_Complete();
        //AppsFlyerTracking.Instance.LogEvent_ads_inter_show();
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial();
        FirebaseManager.Instance.LogEvent_ads_inter_show_failed();
       
        //AppsFlyerTracking.Instance.LogEvent_ads_inter_show_failed();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        FirebaseManager.Instance.LogEvent_ads_inter_impression();
        //AppsFlyerTracking.Instance.LogEvent_ads_inter_impression();
    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        LoadInterstitial();
        FirebaseManager.Instance.LogEvent_ads_inter_complete(adInfo);
        //AppsFlyerTracking.Instance.LogEvent_ads_inter_complete(adInfo);
        //Bridge.instance.ResetInterTimer();
    }

    public void ShowInterstitialAd()
    {
        if (Module.isTest)
            return;

        if (Module.isRemoveAds != 0)
            return;
        Debug.LogError("ShowInterstitialAd");
        if (!GameManager.Instance.isCanShowInter())
            return;
        
        if (MaxSdk.IsInterstitialReady(interManager.adUnitId))
        {
            MaxSdk.ShowInterstitial(interManager.adUnitId);

            if (Module.firstInterAds == 0)
            {
                Module.firstInterAds = 1;
                DOVirtual.DelayedCall(1, () => 
                { 
                    UIMainGame.Instance.Show_UIAdsRemove();
                    UIMainGame.Instance.btnRemoveAds.gameObject.SetActive(true);
                });
            }
            
        }
        else
        {
            print("Interstitial ad is not ready yet.");
        }
    }
    #endregion
    #region Ads Native
    public void InitializeMRecAds()
    {
        // MRECs are sized to 300x250 on phones and tablets
        MaxSdk.CreateMRec(nativeManager.mrecAdUnitId, MaxSdkBase.AdViewPosition.BottomCenter);

        MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdLoadFailedEvent;
        MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMRecAdRevenuePaidEvent;
        MaxSdkCallbacks.MRec.OnAdExpandedEvent += OnMRecAdExpandedEvent;
        MaxSdkCallbacks.MRec.OnAdCollapsedEvent += OnMRecAdCollapsedEvent;
    }

    public void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo error) { }

    public void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    public void OnMRecAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    #endregion
    #region AdsReward
    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    public void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(rewardManager.adUnitId);
        FirebaseManager.Instance.LogEvent_ads_reward_request();
        AdjustTracking.Instance.Event_ads_reward_request();
        //AppsFlyerTracking.Instance.LogEvent_ads_reward_request();
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.
        // Reset retry attempt
        FirebaseManager.Instance.LogEvent_ads_reward_request_complete();

        
        //AppsFlyerTracking.Instance.LogEvent_ads_reward_request_complete();
        rewardManager.retryAttempt = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 16 seconds).
        FirebaseManager.Instance.LogEvent_ads_reward_failed();
        AdjustTracking.Instance.Event_ads_reward_failed();
        //AppsFlyerTracking.Instance.LogEvent_ads_reward_failed();
        rewardManager.retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(4, rewardManager.retryAttempt));
        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        FirebaseManager.Instance.LogEvent_ads_reward_show();
        AdjustTracking.Instance.Event_ads_reward_show();
        //AppsFlyerTracking.Instance.LogEvent_ads_reward_show();

        FirebaseManager.Instance.LogEvent_Ads_Complete();
        FirebaseManager.Instance.LogEvent_Ads_Reward_Complete();
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
       
        if (isCanReward)
        {
            isCanReward = false;
            actionRewardCallback?.Invoke();
            FirebaseManager.Instance.LogEvent_ads_reward_complete(adInfo, rewardType,rewardValue);
            AdjustTracking.Instance.Event_ads_reward_complete(adInfo);

            GameManager.Instance.CheckDelayRewardandInter();
        }
            
    }

    bool isCanReward = false;
    private string rewardType   = string.Empty;
    private string rewardValue  = string.Empty;
    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
        print("Rewarded user: " + reward.Amount + " " + reward.Label);
        isCanReward = true;
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
        TrackAdRevenue(adInfo);
    }
    private Action actionRewardCallback = null;
    public bool isRewardedAdReady => MaxSdk.IsRewardedAdReady(rewardManager.adUnitId);
    public void ShowRewardedAd(Action callback,string _rewardType="",string _rewardValue= "")
    {
        rewardType = _rewardType;
        rewardValue = _rewardValue;
        actionRewardCallback = callback;
        if (MaxSdk.IsRewardedAdReady(rewardManager.adUnitId))
        {
            GameManager.Instance.CheckDelayRewardandInter();
            MaxSdk.ShowRewardedAd(rewardManager.adUnitId);
            
        }
        else
        {
            print("Rewarded ad is not ready yet.");
        }
    }
    #endregion
    private void TrackAdRevenue(MaxSdkBase.AdInfo adInfo)
    {
        FirebaseManager.Instance.Event_af_ad_impression(adInfo);
        //FirebaseManager.Instance.Event_af_ad_impression_gb(adInfo);
        AdjustTracking.Instance.Event_ad_impression(adInfo);
        //AppsFlyerTracking.Instance.Event_af_ad_impression(adInfo);
    }
}
[System.Serializable]
public class AppOpenManager
{
#if UNITY_IOS
   public string AppOpenAdUnitId = "YOUR_IOS_AD_UNIT_ID";
#else // UNITY_ANDROID
    public string AppOpenAdUnitId = "YOUR_ANDROID_AD_UNIT_ID";
#endif
    public int retryAttempt;
}
[System.Serializable]
public class BannerManager
{
#if UNITY_IOS
    public string bannerAdUnitId = "YOUR_IOS_BANNER_AD_UNIT_ID"; // Retrieve the ID from your account
#else // UNITY_ANDROID
    public string bannerAdUnitId = "YOUR_ANDROID_BANNER_AD_UNIT_ID"; // Retrieve the ID from your account
#endif
}
[System.Serializable]
public class InterstitialsManager
{
#if UNITY_IOS
    public string adUnitId = "YOUR_IOS_AD_UNIT_ID";
#else // UNITY_ANDROID
    public string adUnitId = "YOUR_ANDROID_AD_UNIT_ID";
#endif
    public int retryAttempt;
}
[System.Serializable]
public class NativeManager
{
#if UNITY_IOS
    public string mrecAdUnitId = "YOUR_IOS_AD_UNIT_ID"; // Retrieve the ID from your account
#else // UNITY_ANDROID
    public string mrecAdUnitId = "YOUR_ANDROID_AD_UNIT_ID"; // Retrieve the ID from your account
#endif
}
[System.Serializable]
public class RewardManager
{
#if UNITY_IOS
    public string adUnitId = "YOUR_IOS_AD_UNIT_ID";
#else // UNITY_ANDROID
    public string adUnitId = "YOUR_ANDROID_AD_UNIT_ID";
#endif
    public int retryAttempt;
}
