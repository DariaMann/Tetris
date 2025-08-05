using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Buttons : MonoBehaviour {
    
    [SerializeField] private float timing;
    [SerializeField] private bool isPaused;
    [SerializeField] private GameObject educationButton;
    [SerializeField] private GameObject ratingCenterButton;
    [SerializeField] private GameObject ratingLeftButton;
    [SerializeField] private GameObject connectServicesCenterButton;
    [SerializeField] private GameObject connectServicesLeftButton;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private RectTransform settingsPanel;
    
    [SerializeField] private GameObject settingsEnglishSelection;
    [SerializeField] private GameObject settingsRussianSelection;
    
    [SerializeField] private GameObject settingsLightSelection;
    [SerializeField] private GameObject settingsDarkSelection;
    [SerializeField] private GameObject settingsAutoSelection;
    
    [SerializeField] private Image settingsSoundButton;
    [SerializeField] private Image settingsMusicButton;
    [SerializeField] private Image settingsVibrationButton;

    [SerializeField] private Sprite settingsSoundOn;
    [SerializeField] private Sprite settingsSoundOff;
    [SerializeField] private Sprite settingsMusicOn;
    [SerializeField] private Sprite settingsMusicOff;
    [SerializeField] private Sprite settingsVibrationOn;
    [SerializeField] private Sprite settingsVibrationOff;
    
    [SerializeField] private TextMeshProUGUI textAdsButton;
    
    [SerializeField] private Image connectServicesCenterImage;
    [SerializeField] private Image connectServicesLeftImage;
    [SerializeField] private Sprite connectServicesGameCenterIcon;
    [SerializeField] private Sprite connectServicesGooglePlayIcon;

    private Coroutine _themeCoroutine;
    private bool _isDelayStopPause;

    void Start () 
    {
        Themes theme = GameHelper.GetTheme();
        ApplyTheme(theme);
        
        int languageId = GameHelper.GetLanguage();
        ApplyLanguage(languageId);
        
        GameHelper.OnAutentificateChanged += ApplyAutentificate;
        
        GameHelper.GetSound();
        ApplySound(GameHelper.Sound);
        GameHelper.OnSoundChanged += ApplySound;
        
        GameHelper.GetMusic();
        ApplyMusic(GameHelper.Music);
        GameHelper.OnMusicChanged += ApplyMusic;
        
        GameHelper.GetVibration();
        ApplyVibration(GameHelper.Vibration);
        GameHelper.OnVibrationChanged += ApplyVibration;
        
        GameHelper.GetHaveAds();
        ApplyHaveAds(GameHelper.HaveAds);
        GameHelper.OnHaveAdsChanged += ApplyHaveAds;
        
        AudioManager.Instance.ToggleMusic(GameHelper.Music);
        AudioManager.Instance.ToggleSound(GameHelper.Sound);

        if (GameHelper.GameType == MiniGameType.None)
        {
            educationButton.SetActive(false);
//            ratingLeftButton.SetActive(false);
//            ratingCenterButton.SetActive(true);
        }
        else
        {
            educationButton.SetActive(true);
//            ratingLeftButton.SetActive(true);
//            ratingCenterButton.SetActive(false);
        }

        SetConnectServicesImage();
        ApplyAutentificate(GameHelper.IsAutentificate);
    }
    
    void OnApplicationPause(bool pause)
    {
        if (pause && GameHelper.GameType != MiniGameType.None &&
            GameHelper.GameType != MiniGameType.G2048 && GameHelper.GameType != MiniGameType.Lines98 && GameHelper.GameType != MiniGameType.Blocks)
        {
            if (!GameHelper.IsGameOver && !GameHelper.IsEdication && !GameHelper.IsShowRevive)
            {
                OnPauseClick();
            }
        }
    }

    void Update()
    {
        Time.timeScale = timing;
        if (Input.GetKeyDown(KeyCode.Escape) && isPaused == false)
        {
            SetPause(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            SetPause(false);
        }

        if (isPaused)
        {
            GameHelper.IsPause = true;
            timing = 0;
            pauseMenu.SetActive(true);
        }
        else if (isPaused == false)
        {
//                GameHelper.IsPause = false;
            timing = 1;
            pauseMenu.SetActive(false);
            if (GameHelper.IsPause && !_isDelayStopPause)
            {
                _isDelayStopPause = true;
                StartCoroutine(DelayStopPause());
            }
        }

        if (_themeCoroutine == null && GameHelper.Theme == Themes.Auto)
        {
            _themeCoroutine = StartCoroutine(MonitorTheme());
        }

        if (_themeCoroutine != null && GameHelper.Theme != Themes.Auto)
        {
            StopCoroutine(_themeCoroutine);
            _themeCoroutine = null;
        }

        if (Input.GetKey(KeyCode.L))
        {
            OnLightThemeClick();
        }

        if (Input.GetKey(KeyCode.N))
        {
            OnDarkThemeClick();
        }
    }

    private void SetConnectServicesImage()
    {
#if UNITY_EDITOR
        connectServicesCenterImage.sprite = connectServicesGameCenterIcon;
        connectServicesLeftImage.sprite = connectServicesGameCenterIcon;
#elif UNITY_ANDROID
        connectServicesCenterImage.sprite = connectServicesGooglePlayIcon;
        connectServicesLeftImage.sprite = connectServicesGooglePlayIcon;
#elif UNITY_IOS
        connectServicesCenterImage.sprite = connectServicesGameCenterIcon;
        connectServicesLeftImage.sprite = connectServicesGameCenterIcon;
#endif
    }
    
    public void ApplyAutentificate(bool isAutentificate)
    {
        Debug.Log("Аутентификация, смена кнопки, isAutentificate: " + isAutentificate + ", GameHelper.GameType: " + GameHelper.GameType);
        if (isAutentificate)
        {
            if (GameHelper.GameType == MiniGameType.None)
            {
                ratingCenterButton.SetActive(true);
                ratingLeftButton.SetActive(false);
                connectServicesCenterButton.SetActive(false);
                connectServicesLeftButton.SetActive(false);
            }
            else
            {
                ratingCenterButton.SetActive(false);
                ratingLeftButton.SetActive(true);
                connectServicesCenterButton.SetActive(false);
                connectServicesLeftButton.SetActive(false);
            }
        }
        else
        {
            if (GameHelper.GameType == MiniGameType.None)
            {
                ratingCenterButton.SetActive(false);
                ratingLeftButton.SetActive(false);
                connectServicesCenterButton.SetActive(true);
                connectServicesLeftButton.SetActive(false);
            }
            else
            {
                ratingCenterButton.SetActive(false);
                ratingLeftButton.SetActive(false);
                connectServicesCenterButton.SetActive(false);
                connectServicesLeftButton.SetActive(true);
            }
        }
    }

    public IEnumerator DelayStopPause()
    {
        yield return new WaitForSeconds(0.1f);
        GameHelper.IsPause = false;
        _isDelayStopPause = false;
    }

    public void OnEducationPlayClick()
    {
        OnBackSettingsClick();
        OnChangePauseClick();
        GameHelper.IsPause = true;
        _isDelayStopPause = false;
        SetPause(false);
    }

    private IEnumerator MonitorTheme()
    {
        bool lastDark = ThemeManager.IsSystemDarkTheme();
        while (true)
        {
            yield return new WaitForSeconds(1f);
//            Debug.Log("Check Theme Auto");
            bool currentDark = ThemeManager.IsSystemDarkTheme();
            if (currentDark != lastDark)
            {
                GameHelper.InvokeThemeChange();
                lastDark = currentDark;
            }
        }
    }
    
    public void OnDestroy()
    {
        GameHelper.OnAutentificateChanged -= ApplyAutentificate;
        GameHelper.OnSoundChanged -= ApplySound;
        GameHelper.OnMusicChanged -= ApplyMusic;
        GameHelper.OnVibrationChanged -= ApplyVibration;
        GameHelper.OnHaveAdsChanged -= ApplyHaveAds;
        if (_themeCoroutine != null)
        {
            StopCoroutine(_themeCoroutine);
            _themeCoroutine = null;
        }

        GameHelper.IsPause = false;
    }
    
    public void OnSoundClick()
    {
        GameHelper.SetSound(!GameHelper.Sound);
    }
    
    public void ApplySound(bool sound)
    {
        settingsSoundButton.sprite = sound ? settingsSoundOn : settingsSoundOff;
        AudioManager.Instance.ToggleSound(sound);
    }

    public void ApplyLanguage( int languageId)
    {
        SetLanguageSelect(languageId);
    }

    private void SetLanguageSelect(int languageId)
    {
        if (languageId == 0)
        {
            settingsEnglishSelection.SetActive(true);
            settingsRussianSelection.SetActive(false);
        }
        else
        {
            settingsEnglishSelection.SetActive(false);
            settingsRussianSelection.SetActive(true);
        }
    } 
    
    public void ApplyTheme(Themes theme)
    {
        SetThemeSelect(theme);
    }
    
    private void SetThemeSelect(Themes theme)
    {
        if (theme == Themes.Light)
        {
            settingsLightSelection.SetActive(true);
            settingsDarkSelection.SetActive(false);
            settingsAutoSelection.SetActive(false);
        }
        else if (theme == Themes.Night)
        {
            settingsLightSelection.SetActive(false);
            settingsDarkSelection.SetActive(true);
            settingsAutoSelection.SetActive(false);
        }
        else if (theme == Themes.Auto)
        {
            settingsLightSelection.SetActive(false);
            settingsDarkSelection.SetActive(false);
            settingsAutoSelection.SetActive(true);
        }
    }
    
    public void OnLanguageChanged(int languageId)
    {
        string language = languageId == 0 ? "English" : "Russian";

        if (LocalizationManager.Language == language)
        {
            return;
        }
        
        GameHelper.SetLanguage(languageId);
        SetLanguageSelect(languageId);
    }

    public void OnRatingClick()
    {
        if (GameHelper.GameType == MiniGameType.None)
        {
            GameServicesManager.ShowAchievementsUI();
        }
        else
        {
            GameServicesManager.ShowLeaderboardUI(GameHelper.GameType);
        }
    } 
    
    public void OnMusicClick()
    {
        GameHelper.SetMusic(!GameHelper.Music);
    }
    
    public void ApplyMusic(bool music)
    {
        settingsMusicButton.sprite = music ? settingsMusicOn : settingsMusicOff;
        AudioManager.Instance.ToggleMusic(music);
    }
    
    public void OnVibrationClick()
    {
        bool newVibrationState = !GameHelper.Vibration;
        if (newVibrationState)
        {
            GameHelper.VibrationStart();
        }
        GameHelper.SetVibration(newVibrationState);
    }
    
    public void ApplyVibration(bool vibration)
    {
        settingsVibrationButton.sprite = vibration ? settingsVibrationOn : settingsVibrationOff;
    }
    
    public void ApplyHaveAds(bool stateAds)
    {
        if (stateAds)
        {
            textAdsButton.text = LocalizationManager.Localize("Settings.removeads");
        }
        else
        {
            textAdsButton.text = LocalizationManager.Localize("Settings.addads");
        }
    }
    
    public void OnLines98Click()
    {
        SceneManager.LoadScene("Lines98");
    }
    
    public void OnChineseCheckersClick()
    {
        SceneManager.LoadScene("ChineseCheckers");
    }
    
    public void OnBlocksClick()
    {
        SceneManager.LoadScene("Blocks");
    }  
    
    public void OnTetrisClick()
    {
        SceneManager.LoadScene("Tetris");
    }
    
    public void OnSnakeClick()
    {
        SceneManager.LoadScene("Snake");
    }
    
    public void On2048Click()
    {
        SceneManager.LoadScene("2048");
    }
    
    public void OnSettingsClick()
    {
        settingsPanel.gameObject.SetActive(true);
    }
    public void OnBackSettingsClick()
    {
        settingsPanel.gameObject.SetActive(false);
    }
    
    public void OnResumeClick(bool state)
    {
        SetPause(state);
        AppodealManager.Instance.ShowBottomBanner();
    }
    
    public void SetPause(bool state)
    {
        if (GameHelper.IsShowRevive || GameHelper.IsGameOver)
        {
            state = false;
        }
        isPaused = state;
        if (state)
        {
            GameHelper.IsPause = true;
            GameplayTimeTracker.Instance.PauseTimer();
        }
        else
        {
            GameHelper.IsPause = false;
            GameplayTimeTracker.Instance.ResumeTimer();
        }
    }
    
    public void OnHomeClick()
    {
        GameplayTimeTracker.Instance.PauseTimer();

        AnalyticsManager.Instance.LogEvent(AnalyticType.game_finish.ToString(), (float) GameHelper.GameType);

        AppodealManager.Instance.HideBottomBanner();
        SceneManager.LoadScene("Menu");
    }

    public void OnPauseClick()
    {
        SetPause(true);
        AppodealManager.Instance.HideBottomBanner();
    } 
    
    public void OnConnectServicesClick()
    {
        GameServicesManager.AuthenticateUser();
    }
    
    public void OnChangePauseClick()
    {
        SetPause(!isPaused);
    }
    
    public void OnAutoThemeClick()
    {
        GameHelper.SetTheme(Themes.Auto);
        SetThemeSelect(Themes.Auto);
    }
    
    public void OnLightThemeClick()
    {
        GameHelper.SetTheme(Themes.Light);
        SetThemeSelect(Themes.Light);
    }
    
    public void OnDarkThemeClick()
    {
        GameHelper.SetTheme(Themes.Night);
        SetThemeSelect(Themes.Night);
    }

    public void OnThemeClick(Themes theme)
    {
        GameHelper.SetTheme(theme);
        SetThemeSelect(theme);
    }
    
}
