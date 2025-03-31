using System.Collections;
using System.Collections.Generic;
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
    
    [SerializeField] private Image settingsSoundButton;
    [SerializeField] private Image settingsMusicButton;
    [SerializeField] private Image settingsVibrationButton;
    
    [SerializeField] private Sprite settingsSoundOn;
    [SerializeField] private Sprite settingsSoundOff;
    [SerializeField] private Sprite settingsMusicOn;
    [SerializeField] private Sprite settingsMusicOff;
    [SerializeField] private Sprite settingsVibrationOn;
    [SerializeField] private Sprite settingsVibrationOff;
    
    void Start () {
        isPaused = false;
        
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
    }
    
    public void OnDarkThemeClick()
    {
        GameHelper.SetTheme(Themes.Night);
    }

    public void OnThemeClick(Themes theme)
    {
        GameHelper.SetTheme(theme);
    }
    
}
