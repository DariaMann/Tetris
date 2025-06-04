using System;
using System.Collections.Generic;
using UnityEngine;
using AppodealStack.Monetization.Api;
using AppodealStack.Monetization.Common;

public class AppodealManager : MonoBehaviour, IAppodealInitializationListener
{
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

        Appodeal.SetLogLevel(AppodealLogLevel.Verbose); // Включить подробные логи
        //Appodeal.SetTesting(true); // Включить тестовый режим
        
        // Типы рекламы, которые используем
        int adTypes = AppodealAdType.Interstitial | AppodealAdType.Banner | AppodealAdType.RewardedVideo;
        
        AppodealCallbacks.Sdk.OnInitialized += OnInitializationFinished;
        // Инициализация Appodeal
        Appodeal.Initialize(appKey, adTypes);

        // Подписываемся на события
        InterstitialCallbacksSubscribe();
        RewardedVideoCallbacksSubscribe();
        BannerCallbacksSubscribe();

        // todo: Включаем тестовый режим (выключи перед релизом)
        Appodeal.SetTesting(true);

        // Загрузить баннер
        ShowBottomBanner();
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
        Appodeal.Show(AppodealShowStyle.BannerBottom);
    }
    
    public void HideBottomBanner()
    {
        Appodeal.Hide(AppodealAdType.Banner);
    }

    // Метод для показа межстраничной рекламы (например, после уровня)
    public void ShowInterstitial()
    {
        if (Appodeal.IsLoaded(AppodealAdType.Interstitial))
        {
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
        if (Appodeal.IsLoaded(AppodealAdType.RewardedVideo))
        {
            Appodeal.Show(AppodealShowStyle.RewardedVideo);
        }
        else
        {
            Debug.Log("Вознаградительная реклама не загружена");
        }
    }
    
    public void OnInitializationFinished(List<string> errors)
    {
        if (errors != null && errors.Count > 0)
        {
            Debug.LogError("Appodeal инициализация завершилась с ошибками: " + string.Join(", ", errors));
        }
        else
        {
            Debug.Log("Appodeal успешно инициализирован!");
        }
    }
    
    public bool IsInterstitialReady() => Appodeal.IsLoaded(AppodealAdType.Interstitial);
    public bool IsRewardedVideoReady()
    {
#if UNITY_EDITOR
        return true;
#endif
      return  Appodeal.IsLoaded(AppodealAdType.RewardedVideo);
    } 
    
    // --- Реализация интерфейса IAppodealInitializationListener ---
    
    public void OnInitializationFinished(object sender, SdkInitializedEventArgs e) {}
    
    #region InterstitialAd Callbacks

// Called when interstitial was loaded (precache flag shows if the loaded ad is precache)
    private void OnInterstitialLoaded(object sender, AdLoadedEventArgs e)
    {
        Debug.Log("Interstitial loaded");
    }

// Called when interstitial failed to load
    private void OnInterstitialFailedToLoad(object sender, EventArgs e)
    {
        Debug.Log("Interstitial failed to load");
    }

// Called when interstitial was loaded, but cannot be shown (internal network errors, placement settings, etc.)
    private void OnInterstitialShowFailed(object sender, EventArgs e)
    {
        Debug.Log("Interstitial show failed");
    }

// Called when interstitial is shown
    private void OnInterstitialShown(object sender, EventArgs e)
    {
        Debug.Log("Interstitial shown");
    }

// Called when interstitial is closed
    private void OnInterstitialClosed(object sender, EventArgs e)
    {
        Debug.Log("Interstitial closed");
        OnInterstitialFinished?.Invoke();
    }

// Called when interstitial is clicked
    private void OnInterstitialClicked(object sender, EventArgs e)
    {
        Debug.Log("Interstitial clicked");
    }

// Called when interstitial is expired and can not be shown
    private void OnInterstitialExpired(object sender, EventArgs e)
    {
        Debug.Log("Interstitial expired");
    }

    #endregion

    #region RewardedVideoAd Callbacks

//Called when rewarded video was loaded (precache flag shows if the loaded ad is precache).
    private void OnRewardedVideoLoaded(object sender, AdLoadedEventArgs e)
    {
        Debug.Log($"[APDUnity] [Callback] OnRewardedVideoLoaded(bool isPrecache:{e.IsPrecache})");
        OnRewardedVideoLoadedAction?.Invoke();
    }

// Called when rewarded video failed to load
    private void OnRewardedVideoFailedToLoad(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnRewardedVideoFailedToLoad()");
    }

// Called when rewarded video was loaded, but cannot be shown (internal network errors, placement settings, etc.)
    private void OnRewardedVideoShowFailed(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnRewardedVideoShowFailed()");
    }

// Called when rewarded video is shown
    private void OnRewardedVideoShown(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnRewardedVideoShown()");
    }

// Called when rewarded video is closed
    private void OnRewardedVideoClosed(object sender, RewardedVideoClosedEventArgs e)
    {
        Debug.Log($"[APDUnity] [Callback] OnRewardedVideoClosed(bool finished:{e.Finished})");
    }

// Called when rewarded video is viewed until the end
    private void OnRewardedVideoFinished(object sender, RewardedVideoFinishedEventArgs e)
    {
        Debug.Log($"[APDUnity] [Callback] OnRewardedVideoFinished(double amount:{e.Amount}, string name:{e.Currency})");
        OnRewardedVideoFinishedAction?.Invoke();
    }

// Called when rewarded video is clicked
    private void OnRewardedVideoClicked(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnRewardedVideoClicked()");
    }

//Called when rewarded video is expired and can not be shown
    private void OnRewardedVideoExpired(object sender, EventArgs e)
    {
        Debug.Log("[APDUnity] [Callback] OnRewardedVideoExpired()");
    }

    #endregion
    
    #region BannerAd Callbacks

// Called when a banner is loaded (height arg shows banner's height, precache arg shows if the loaded ad is precache
    private void OnBannerLoaded(object sender, BannerLoadedEventArgs e)
    {
        Debug.Log("Banner loaded");
    }

// Called when banner failed to load
    private void OnBannerFailedToLoad(object sender, EventArgs e)
    {
        Debug.Log("Banner failed to load");
    }

// Called when banner failed to show
    private void OnBannerShowFailed(object sender, EventArgs e)
    {
        Debug.Log("Banner show failed");
    }

// Called when banner is shown
    private void OnBannerShown(object sender, EventArgs e)
    {
        Debug.Log("Banner shown");
    }

    // Called when banner is clicked
    private void OnBannerClicked(object sender, EventArgs e)
    {
        Debug.Log("Banner clicked");
    }

// Called when banner is expired and can not be shown
    private void OnBannerExpired(object sender, EventArgs e)
    {
        Debug.Log("Banner expired");
    }

    #endregion
    
}
