using System;
using UnityEngine;
using AppodealStack.Monetization.Api;
using AppodealStack.Monetization.Common;
using Random = UnityEngine.Random;

public class AppodealManager : MonoBehaviour
{
    private int gamesPlayed = 0;
    private float lastAdTime = 0;
    
    private string androidAppKey = "80283a642a3caa535c385d3fec98d81d537a51ae63abf318";
    private string iosAppKey = "5199cecdd065fb99badcf0b6c48fc33701d5bf296fcd7862";
    
    public static AppodealManager Instance { get; private set; }
    
    public Action OnRewardedVideoFinishedAction;
    public Action OnRewardedVideoLoadedAction;
    public Action OnInterstitialFinished;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Чтобы не уничтожался между сценами
            Initialize();
            OnInterstitialFinished += RestartInterstitialTimer;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        InterstitialCallbacksUnsubscribe();
        RewardedVideoCallbacksUnsubscribe();
        BannerCallbacksUnsubscribe();
        OnInterstitialFinished -= RestartInterstitialTimer;
    }

    public void Initialize()
    {
        // Выбираем ключ в зависимости от платформы
#if UNITY_ANDROID
        string appKey = androidAppKey;
#elif UNITY_IOS
        string appKey = iosAppKey;
#else
        string appKey = "";
#endif

        // todo: Включаем тестовый режим (выключи перед релизом)
//        Appodeal.SetTesting(true);
        Appodeal.SetLogLevel(AppodealLogLevel.Verbose); // Включить подробные логи

        // Типы рекламы, которые используем
        int adTypes = AppodealAdType.Interstitial | AppodealAdType.Banner | AppodealAdType.RewardedVideo;
        
        AppodealCallbacks.Sdk.OnInitialized += OnInitializationFinished;
        // Инициализация Appodeal
        Appodeal.Initialize(appKey, adTypes);

        // Подписываемся на события
        InterstitialCallbacksSubscribe();
        RewardedVideoCallbacksSubscribe();
        BannerCallbacksSubscribe();
    }
    
    public void InterstitialCallbacksSubscribe()
    {
        AppodealCallbacks.Interstitial.OnLoaded += OnInterstitialLoaded;
        AppodealCallbacks.Interstitial.OnFailedToLoad += OnInterstitialFailedToLoad;
        AppodealCallbacks.Interstitial.OnShown += OnInterstitialShown;
        AppodealCallbacks.Interstitial.OnShowFailed += OnInterstitialShowFailed;
        AppodealCallbacks.Interstitial.OnClosed += OnInterstitialClosed;
        AppodealCallbacks.Interstitial.OnClicked += OnInterstitialClicked;
        AppodealCallbacks.Interstitial.OnExpired += OnInterstitialExpired;
    }
    
    public void RewardedVideoCallbacksSubscribe()
    {
        AppodealCallbacks.RewardedVideo.OnLoaded += OnRewardedVideoLoaded;
        AppodealCallbacks.RewardedVideo.OnFailedToLoad += OnRewardedVideoFailedToLoad;
        AppodealCallbacks.RewardedVideo.OnShown += OnRewardedVideoShown;
        AppodealCallbacks.RewardedVideo.OnShowFailed += OnRewardedVideoShowFailed;
        AppodealCallbacks.RewardedVideo.OnClosed += OnRewardedVideoClosed;
        AppodealCallbacks.RewardedVideo.OnFinished += OnRewardedVideoFinished;
        AppodealCallbacks.RewardedVideo.OnClicked += OnRewardedVideoClicked;
        AppodealCallbacks.RewardedVideo.OnExpired += OnRewardedVideoExpired;
    }
    
    public void BannerCallbacksSubscribe()
    {
        AppodealCallbacks.Banner.OnLoaded += OnBannerLoaded;
        AppodealCallbacks.Banner.OnFailedToLoad += OnBannerFailedToLoad;
        AppodealCallbacks.Banner.OnShown += OnBannerShown;
        AppodealCallbacks.Banner.OnShowFailed += OnBannerShowFailed;
        AppodealCallbacks.Banner.OnClicked += OnBannerClicked;
        AppodealCallbacks.Banner.OnExpired += OnBannerExpired;
    }
    
    public void InterstitialCallbacksUnsubscribe()
    {
        AppodealCallbacks.Interstitial.OnLoaded -= OnInterstitialLoaded;
        AppodealCallbacks.Interstitial.OnFailedToLoad -= OnInterstitialFailedToLoad;
        AppodealCallbacks.Interstitial.OnShown -= OnInterstitialShown;
        AppodealCallbacks.Interstitial.OnShowFailed -= OnInterstitialShowFailed;
        AppodealCallbacks.Interstitial.OnClosed -= OnInterstitialClosed;
        AppodealCallbacks.Interstitial.OnClicked -= OnInterstitialClicked;
        AppodealCallbacks.Interstitial.OnExpired -= OnInterstitialExpired;
    }
    
    public void RewardedVideoCallbacksUnsubscribe()
    {
        AppodealCallbacks.RewardedVideo.OnLoaded -= OnRewardedVideoLoaded;
        AppodealCallbacks.RewardedVideo.OnFailedToLoad -= OnRewardedVideoFailedToLoad;
        AppodealCallbacks.RewardedVideo.OnShown -= OnRewardedVideoShown;
        AppodealCallbacks.RewardedVideo.OnShowFailed -= OnRewardedVideoShowFailed;
        AppodealCallbacks.RewardedVideo.OnClosed -= OnRewardedVideoClosed;
        AppodealCallbacks.RewardedVideo.OnFinished -= OnRewardedVideoFinished;
        AppodealCallbacks.RewardedVideo.OnClicked -= OnRewardedVideoClicked;
        AppodealCallbacks.RewardedVideo.OnExpired -= OnRewardedVideoExpired;
    }
    
    public void BannerCallbacksUnsubscribe()
    {
        AppodealCallbacks.Banner.OnLoaded -= OnBannerLoaded;
        AppodealCallbacks.Banner.OnFailedToLoad -= OnBannerFailedToLoad;
        AppodealCallbacks.Banner.OnShown -= OnBannerShown;
        AppodealCallbacks.Banner.OnShowFailed -= OnBannerShowFailed;
        AppodealCallbacks.Banner.OnClicked -= OnBannerClicked;
        AppodealCallbacks.Banner.OnExpired -= OnBannerExpired;
    }

    public void ShowBottomBanner()
    {
        if (!GameHelper.HaveAds)
        {
            return;
        }
        Debug.Log("ShowBottomBanner");
        Appodeal.Show(AppodealShowStyle.BannerBottom);
    }
    
    public void HideBottomBanner()
    {
        Debug.Log("HideBottomBanner");
        Appodeal.Hide(AppodealAdType.Banner);
    }

    public bool IsShowInterstitial()
    {
        gamesPlayed++;
        int randomNumberGame = Random.Range(2, 4);
        float randomMinIntervalSeconds = Random.Range(120f, 150f);
        Debug.Log("Межстраничная реклама: randomNumberGame = " + randomNumberGame + ", gamesPlayed = " + gamesPlayed +
                  ", randomMinIntervalSeconds = " + randomMinIntervalSeconds +
                  ", (Time.time - lastAdTime) = "+(Time.time - lastAdTime));
        if (gamesPlayed >= randomNumberGame && Time.time - lastAdTime > randomMinIntervalSeconds)
        {
            if (IsInterstitialReady())
            {
                return true;
            }
        }
        return false;
    }

    public void RestartInterstitialTimer()
    {
        gamesPlayed = 0;
        lastAdTime = Time.time;
        Debug.Log("Обновление таймера межстраничной рекламы");
    }

    // Метод для показа межстраничной рекламы (например, после уровня)
    public void ShowInterstitial()
    {
        if (!GameHelper.HaveAds)
        {
            return;
        }
        if (Appodeal.IsLoaded(AppodealAdType.Interstitial))
        {
            Debug.Log("Показ межстраничной рекламы");
            Appodeal.Show(AppodealShowStyle.Interstitial);
        }
        else
        {
            Debug.Log("Межстраничная реклама не загружена");
        }
    }

    // Метод для показа вознаградительной рекламы
    public void ShowRewardedVideo()
    {
        if (!GameHelper.HaveAds)
        {
            return;
        }
        if (Appodeal.IsLoaded(AppodealAdType.RewardedVideo))
        {
            Appodeal.Show(AppodealShowStyle.RewardedVideo);
        }
        else
        {
            Debug.Log("Вознаградительная реклама не загружена");
        }
    }

    public bool IsInterstitialReady()
    {
        if (!GameHelper.HaveAds)
        {
            return false;
        }
        return Appodeal.IsLoaded(AppodealAdType.Interstitial);
    } 
    
    public bool IsRewardedVideoReady()
    {
        if (!GameHelper.HaveAds)
        {
            return false;
        }
        return Appodeal.IsLoaded(AppodealAdType.RewardedVideo);
    } 
    
    // --- Реализация интерфейса IAppodealInitializationListener ---
    
    public void OnInitializationFinished(object sender, SdkInitializedEventArgs e) {}
    
    #region InterstitialAd Callbacks

// Called when interstitial was loaded (precache flag shows if the loaded ad is precache)
    private void OnInterstitialLoaded(object sender, AdLoadedEventArgs e)
    {
        Debug.Log("Interstitial loaded");
        AnalyticsManager.Instance.LogEvent(AnalyticType.interstitial_loaded.ToString());
    }

// Called when interstitial failed to load
    private void OnInterstitialFailedToLoad(object sender, EventArgs e)
    {
        Debug.Log("Interstitial failed to load");
        AnalyticsManager.Instance.LogEvent(AnalyticType.interstitial_load_failed.ToString());
    }

// Called when interstitial was loaded, but cannot be shown (internal network errors, placement settings, etc.)
    private void OnInterstitialShowFailed(object sender, EventArgs e)
    {
        Debug.Log("Interstitial show failed");
        AnalyticsManager.Instance.LogEvent(AnalyticType.interstitial_show_failed.ToString());
    }

// Called when interstitial is shown
    private void OnInterstitialShown(object sender, EventArgs e)
    {
        Debug.Log("Interstitial shown");
        AnalyticsManager.Instance.LogEvent(AnalyticType.interstitial_shown.ToString());
    }

// Called when interstitial is closed
    private void OnInterstitialClosed(object sender, EventArgs e)
    {
        Debug.Log("Interstitial closed");
        OnInterstitialFinished?.Invoke();
        AnalyticsManager.Instance.LogEvent(AnalyticType.interstitial_closed.ToString());
    }

// Called when interstitial is clicked
    private void OnInterstitialClicked(object sender, EventArgs e)
    {
        Debug.Log("Interstitial clicked");
        AnalyticsManager.Instance.LogEvent(AnalyticType.interstitial_clicked.ToString());
    }

// Called when interstitial is expired and can not be shown
    private void OnInterstitialExpired(object sender, EventArgs e)
    {
        Debug.Log("Interstitial expired");
        AnalyticsManager.Instance.LogEvent(AnalyticType.interstitial_expired.ToString());
    }

    #endregion

    #region RewardedVideoAd Callbacks

//Called when rewarded video was loaded (precache flag shows if the loaded ad is precache).
    private void OnRewardedVideoLoaded(object sender, AdLoadedEventArgs e)
    {
        Debug.Log($"[APDUnity] [Callback] OnRewardedVideoLoaded(bool isPrecache:{e.IsPrecache})");
        OnRewardedVideoLoadedAction?.Invoke();
        AnalyticsManager.Instance.LogEvent(AnalyticType.rewarded_loaded.ToString());
    }

// Called when rewarded video failed to load
    private void OnRewardedVideoFailedToLoad(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnRewardedVideoFailedToLoad()");
        AnalyticsManager.Instance.LogEvent(AnalyticType.rewarded_load_failed.ToString());
    }

// Called when rewarded video was loaded, but cannot be shown (internal network errors, placement settings, etc.)
    private void OnRewardedVideoShowFailed(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnRewardedVideoShowFailed()");
        AnalyticsManager.Instance.LogEvent(AnalyticType.rewarded_show_failed.ToString());
    }

// Called when rewarded video is shown
    private void OnRewardedVideoShown(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnRewardedVideoShown()");
        AnalyticsManager.Instance.LogEvent(AnalyticType.rewarded_shown.ToString());
    }

// Called when rewarded video is closed
    private void OnRewardedVideoClosed(object sender, RewardedVideoClosedEventArgs e)
    {
        Debug.Log($"[APDUnity] [Callback] OnRewardedVideoClosed(bool finished:{e.Finished})");
        AnalyticsManager.Instance.LogEvent(AnalyticType.rewarded_closed.ToString());
    }

// Called when rewarded video is viewed until the end
    private void OnRewardedVideoFinished(object sender, RewardedVideoFinishedEventArgs e)
    {
        Debug.Log($"[APDUnity] [Callback] OnRewardedVideoFinished(double amount:{e.Amount}, string name:{e.Currency})");
        OnRewardedVideoFinishedAction?.Invoke();
        AnalyticsManager.Instance.LogEvent(AnalyticType.rewarded_finished.ToString());
    }

// Called when rewarded video is clicked
    private void OnRewardedVideoClicked(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnRewardedVideoClicked()");
        AnalyticsManager.Instance.LogEvent(AnalyticType.rewarded_clicked.ToString());
    }

//Called when rewarded video is expired and can not be shown
    private void OnRewardedVideoExpired(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnRewardedVideoExpired()");
        AnalyticsManager.Instance.LogEvent(AnalyticType.rewarded_expired.ToString());
    }

    #endregion
    
    #region BannerAd Callbacks

// Called when a banner is loaded (height arg shows banner's height, precache arg shows if the loaded ad is precache
    private void OnBannerLoaded(object sender, BannerLoadedEventArgs e)
    {
        Debug.Log("Banner loaded");
//        AnalyticsManager.Instance.LogEvent(AnalyticType.banner_loaded.ToString());
    }

// Called when banner failed to load
    private void OnBannerFailedToLoad(object sender, EventArgs e)
    {
        Debug.Log("Banner failed to load");
        AnalyticsManager.Instance.LogEvent(AnalyticType.banner_load_failed.ToString());
    }

// Called when banner failed to show
    private void OnBannerShowFailed(object sender, EventArgs e)
    {
        Debug.Log("Banner show failed");
        AnalyticsManager.Instance.LogEvent(AnalyticType.banner_show_failed.ToString());
    }

// Called when banner is shown
    private void OnBannerShown(object sender, EventArgs e)
    {
        Debug.Log("Banner shown");
//        AnalyticsManager.Instance.LogEvent(AnalyticType.banner_shown.ToString());
    }

    // Called when banner is clicked
    private void OnBannerClicked(object sender, EventArgs e)
    {
        Debug.Log("Banner clicked");
        AnalyticsManager.Instance.LogEvent(AnalyticType.banner_clicked.ToString());
    }

// Called when banner is expired and can not be shown
    private void OnBannerExpired(object sender, EventArgs e)
    {
        Debug.Log("Banner expired");
        AnalyticsManager.Instance.LogEvent(AnalyticType.banner_expired.ToString());
    }

    #endregion
    
}
