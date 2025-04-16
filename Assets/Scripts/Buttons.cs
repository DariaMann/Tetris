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
    [SerializeField] private bool exitController;
    [SerializeField] private bool havePausePanel = true;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsPanel;
    
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
    
    private Coroutine _themeCoroutine;

    void Start () {
        isPaused = false;

        Themes theme = GameHelper.GetTheme();
        ApplyTheme(theme);
        
        int languageId = GameHelper.GetLanguage();
        ApplyLanguage(languageId);
        
        GameHelper.GetSound();
        ApplySound(GameHelper.Sound);
        GameHelper.OnSoundChanged += ApplySound;
        
        GameHelper.GetMusic();
        ApplyMusic(GameHelper.Music);
        GameHelper.OnMusicChanged += ApplyMusic;
        
        GameHelper.GetVibration();
        ApplyVibration(GameHelper.Vibration);
        GameHelper.OnVibrationChanged += ApplyVibration;
    }
	
	void Update () {
        if (havePausePanel)
        {
            Time.timeScale = timing;
            if (Input.GetKeyDown(KeyCode.Escape) && isPaused == false)
            {
                isPaused = true;
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && isPaused == true)
            {
                isPaused = false;
            }
            if (isPaused == true)
            {
                timing = 0;
                if (!exitController)
                {
                    pauseMenu.SetActive(true);
                }
                else
                {
                    pauseMenu.SetActive(false);
                }
            }
            else if (isPaused == false)
            {
                timing = 1;
                pauseMenu.SetActive(false);
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
    }
    
    private IEnumerator MonitorTheme()
    {
        bool lastDark = ThemeManager.IsSystemDarkTheme();
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("Check Theme Auto");
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
        GameHelper.OnSoundChanged -= ApplySound;
        GameHelper.OnMusicChanged -= ApplyMusic;
        GameHelper.OnVibrationChanged -= ApplyVibration;
        if (_themeCoroutine != null)
        {
            StopCoroutine(_themeCoroutine);
            _themeCoroutine = null;
        }
    }
    
    public void OnSoundClick()
    {
        GameHelper.SetSound(!GameHelper.Sound);
    }
    
    public void ApplySound(bool sound)
    {
        settingsSoundButton.sprite = sound ? settingsSoundOn : settingsSoundOff;
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
        GameServicesManager.ShowLeaderboardUI(GameHelper.GameType);
    } 
    
    public void OnMusicClick()
    {
        GameHelper.SetMusic(!GameHelper.Music);
    }
    
    public void ApplyMusic(bool music)
    {
        settingsMusicButton.sprite = music ? settingsMusicOn : settingsMusicOff;
    }
    
    public void OnVibrationClick()
    {
        GameHelper.SetVibration(!GameHelper.Vibration);
    }
    
    public void ApplyVibration(bool vibration)
    {
        settingsVibrationButton.sprite = vibration ? settingsVibrationOn : settingsVibrationOff;
    }
    
    public void OnChineseCheckersClick()
    {
        SceneManager.LoadScene("ChineseCheckers");
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
        settingsPanel.SetActive(true);
    }
    public void OnBackSettingsClick()
    {
        settingsPanel.SetActive(false);
    }
    public void OnResumeClick(bool state)
    {
        isPaused = state;
    }
    public void OnHomeClick()
    {
        SceneManager.LoadScene("Menu");

    }

    public void OnPauseClick()
    {
        isPaused = true;
        if (isPaused == true)
        {
            timing = 0;
            if (!exitController)
            {
                pauseMenu.SetActive(true);
            }
            else
            {
                pauseMenu.SetActive(false);
            }
        }
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
