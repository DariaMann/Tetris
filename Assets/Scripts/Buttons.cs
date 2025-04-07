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
    [SerializeField] private int scene;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsPanel;
    
    [SerializeField] private Image settingsEnglishButton;
    [SerializeField] private Image settingsRussianButton;
    [SerializeField] private TextMeshProUGUI settingsEnglishText;
    [SerializeField] private TextMeshProUGUI settingsRussianText;
    
    [SerializeField] private Image settingsLightButton;
    [SerializeField] private Image settingsDarkButton;
    [SerializeField] private TextMeshProUGUI settingsLightText;
    [SerializeField] private TextMeshProUGUI settingsDarkText;
    
    [SerializeField] private Image settingsSoundButton;
    [SerializeField] private Image settingsMusicButton;
    [SerializeField] private Image settingsVibrationButton;

    [SerializeField] private Sprite settingsSoundOn;
    [SerializeField] private Sprite settingsSoundOff;
    [SerializeField] private Sprite settingsMusicOn;
    [SerializeField] private Sprite settingsMusicOff;
    [SerializeField] private Sprite settingsVibrationOn;
    [SerializeField] private Sprite settingsVibrationOff;

    private Color _colorLight;
    private Color _colorGrey;


    private void Awake()
    { 
        _colorLight = ColorUtility.TryParseHtmlString("#D4D4D8", out Color color) ? color : Color.white; 
        _colorGrey = ColorUtility.TryParseHtmlString("#454244", out Color color1) ? color1 : Color.gray;
    }

    void Start () {
        isPaused = false;

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
        if (scene == 2 || scene == 3 || scene == 4 || scene == 5)
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
    }
    
    public void OnDestroy()
    {
        GameHelper.OnSoundChanged -= ApplySound;
        GameHelper.OnMusicChanged -= ApplyMusic;
        GameHelper.OnVibrationChanged -= ApplyVibration;
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
            settingsEnglishButton.color = _colorLight;
            settingsRussianButton.color = _colorGrey;
            
            settingsEnglishText.color = _colorGrey;
            settingsRussianText.color = _colorLight;
        }
        else
        {
            settingsEnglishButton.color = _colorGrey;
            settingsRussianButton.color = _colorLight;
            
            settingsEnglishText.color = _colorLight;
            settingsRussianText.color = _colorGrey;
        }
    } 
    
    private void SetThemeSelect(Themes theme)
    {
        if (theme == Themes.Light)
        {
            settingsLightButton.color = _colorLight;
            settingsDarkButton.color = _colorGrey;
            
            settingsLightText.color = _colorGrey;
            settingsDarkText.color = _colorLight;
        }
        else
        {
            settingsLightButton.color = _colorGrey;
            settingsDarkButton.color = _colorLight;
            
            settingsLightText.color = _colorLight;
            settingsDarkText.color = _colorGrey;
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
        SetLocalization(language);
        SetLanguageSelect(languageId);
    }
    
    public void SetLocalization(string localization)
    {
        LocalizationManager.Language = localization;
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
    }
    
}
