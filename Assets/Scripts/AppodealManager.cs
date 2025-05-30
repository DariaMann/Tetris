using System.Collections.Generic;
using UnityEngine;
//using AppodealAds.Unity.Api;
//using AppodealAds.Unity.Common;

public class AppodealManager : MonoBehaviour//, IAppodealInitializationListener, IRewardedVideoAdListener, IInterstitialAdListener, IBannerAdListener 
{
    private string androidAppKey = "80283a642a3caa535c385d3fec98d81d537a51ae63abf318";
    private string iosAppKey = "5199cecdd065fb99badcf0b6c48fc33701d5bf296fcd7862";
    
    public static AppodealManager Instance { get; private set; }
    
    public System.Action OnRewardedVideoFinished;
    public System.Action OnRewardedVideoLoaded;
    public System.Action OnInterstitialFinished;
    
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
//
//        // Типы рекламы, которые используем
//        int adTypes = Appodeal.INTERSTITIAL | Appodeal.BANNER | Appodeal.REWARDED_VIDEO;
//
//        // Инициализация Appodeal
//        Appodeal.initialize(appKey, adTypes, this);
//
//        // Подписываемся на события
//        Appodeal.setInterstitialCallbacks(this);
//        Appodeal.setRewardedVideoCallbacks(this);
//        Appodeal.setBannerCallbacks(this);
//
//        // todo: Включаем тестовый режим (выключи перед релизом)
//        Appodeal.setTesting(true);

        // Загрузить баннер
        ShowBottomBanner();
    }

    public void ShowBottomBanner()
    {
        // Загрузить баннер
//        Appodeal.show(Appodeal.BANNER_BOTTOM);
    }
    
    public void HideBottomBanner()
    {
        // Скрыть баннер
//        Appodeal.hide(Appodeal.BANNER);
    }

    // Метод для показа межстраничной рекламы (например, после уровня)
    public void ShowInterstitial()
    {
//        if (Appodeal.isLoaded(Appodeal.INTERSTITIAL))
//        {
//            Appodeal.show(Appodeal.INTERSTITIAL);
//            
//#if UNITY_ANDROID
//            Appodeal.muteVideosIfCallsMuted(true);
//#endif
//        }
//        else
        {
            Debug.Log("Межстраничная реклама не загружена");
        }
    }

    // Метод для показа вознаградительной рекламы
    public void ShowRewardedVideo()
    {
//        if (Appodeal.isLoaded(Appodeal.REWARDED_VIDEO))
//        {
//            Appodeal.show(Appodeal.REWARDED_VIDEO);
//        }
//        else
        {
            Debug.Log("Вознаградительная реклама не загружена");
        }
    }
    
//    public bool IsInterstitialReady() => Appodeal.isLoaded(Appodeal.INTERSTITIAL);
    public bool IsRewardedVideoReady()
    {
//#if UNITY_EDITOR
        return true;
//#endif
//      return  Appodeal.isLoaded(Appodeal.REWARDED_VIDEO);
    } 
    
    // --- Реализация интерфейса IAppodealInitializationListener ---
    
    public void onInitializationFinished(List<string> errors) {}

    #region Interstitial callback handlers
    
    // Вызывается, когда пользователь кликнул по межстраничной рекламе
    public void onInterstitialClicked()
    {
        Debug.Log("Клик по межстраничной рекламе");
    }
    
    // Вызывается, когда межстраничная реклама показана
    public void onInterstitialShown()
    {
        Debug.Log("Межстраничная реклама начата");
    }

// Вызывается, когда межстраничная реклама загружена (флаг precache показывает, является ли реклама предварительно загруженной)
    public void onInterstitialLoaded(bool isPrecache)
    {
        Debug.Log("Межстраничная реклама загружена");
    }
    
    // Вызывается, когда пользователь закрыл межстраничную рекламу
    public void onInterstitialClosed()
    {
        Debug.Log("Межстраничная реклама закрыта");
        OnInterstitialFinished?.Invoke(); // подключи в UI
    }

// Вызывается, когда не удалось загрузить межстраничную рекламу
    public void onInterstitialFailedToLoad()
    {
        Debug.Log("Не удалось загрузить межстраничную рекламу");
    }

// Вызывается, когда реклама была загружена, но не может быть показана (внутренние ошибки сети, настройки размещения и т.д.)
    public void onInterstitialShowFailed()
    {
        Debug.Log("Не удалось показать межстраничную рекламу");
    }

// Вызывается, когда срок действия рекламы истёк, и она не может быть показана
    public void onInterstitialExpired()
    {
        Debug.Log("Срок действия межстраничной рекламы истёк");
    }

    #endregion
    
    #region Rewarded Video callback handlers
    
    public void onRewardedVideoShown()
    {
        Debug.Log("Вознаградительное видео началось");
    }

    public void onRewardedVideoClicked()
    {
        Debug.Log("Пользователь кликнул на вознаградительную рекламу");
    }

    public void onRewardedVideoFinished(double amount, string name)
    {
        Debug.Log("Пользователь досмотрел видео и получил награду!");
        // Здесь можно дать пользователю награду (монеты, бонусы и т.п.)
        OnRewardedVideoFinished?.Invoke(); // подключи в UI
    }

    public void onRewardedVideoClosed(bool finished)
    {
        Debug.Log("Вознаградительное видео закрыто. Было досмотрено: " + finished);
    }

    public void onRewardedVideoLoaded(bool isPrecache)
    {
        Debug.Log("Вознаградительная реклама загружена");
        OnRewardedVideoLoaded?.Invoke(); // подключи в UI
    }
    
    public void onRewardedVideoExpired()
    {
        Debug.Log("Срок действия согласия пользователя на просмотр вознаграждаемого видео истек");
    }

    public void onRewardedVideoFailedToLoad()
    {
        Debug.Log("Не удалось загрузить вознаградительную рекламу");
    }

    public void onRewardedVideoShowFailed()
    {
        Debug.Log("Не удалось показать вознаградительную рекламу");
    }

    #endregion
    
    #region Banner callback handlers

// Called when a banner is loaded (height arg shows banner's height, precache arg shows if the loaded ad is precache
    public void onBannerLoaded(int height, bool precache)
    {
        Debug.Log("Banner loaded");
    }

// Called when banner failed to load
    public void onBannerFailedToLoad()
    {
        Debug.Log("Banner failed to load");
    }

// Called when banner is shown
    public void onBannerShown()
    {
        Debug.Log("Banner shown");
    }

// Called when banner failed to show
    public void onBannerShowFailed()
    {
        Debug.Log("Banner show failed");
    }

// Called when banner is clicked
    public void onBannerClicked()
    {
        Debug.Log("Banner clicked");
    }

// Called when banner is expired and can not be shown
    public void onBannerExpired()
    {
        Debug.Log("Banner expired");
    }

    #endregion
}
