using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ThemeSnake : MonoBehaviour
{
    [SerializeField] private Camera bgColor;
    [SerializeField] private Image backButton;
    [SerializeField] private TextMeshProUGUI scoreAndRecordText;
    
    [SerializeField] private Image settingsBg;
    [SerializeField] private Image settingsBackButton;
    [SerializeField] private TextMeshProUGUI settingsThemeText;
    [SerializeField] private TextMeshProUGUI settingsLanguageText;
    
    [SerializeField] private Image settingsSoundButton;
    [SerializeField] private Image settingsMusicButton;
    [SerializeField] private Image settingsVibrationButton;

    private Themes _theme;
    private Color _colorLight;
    private Color _colorGrey;
    private Color _colorDark;

    private void Start()
    {
        _colorLight = ColorUtility.TryParseHtmlString("#D4D4D8", out Color color) ? color : Color.white;
        _colorGrey = ColorUtility.TryParseHtmlString("#454244", out Color color1) ? color1 : Color.gray;
        _colorDark = ColorUtility.TryParseHtmlString("#212022", out Color color2) ? color2 : Color.black;
        
        SetTheme(GameHelper.Theme);
        GameHelper.OnThemeChanged += ApplyTheme;
        GameHelper.GameType = MiniGameType.Snake;
    }
    
    private void ApplyTheme(Themes newTheme)
    {
        Console.WriteLine($"Theme changed to {newTheme.ToString()}");
        // Здесь можно добавить код смены оформления игры
        SetTheme(newTheme);
    }
    
    public void OnDestroy()
    {
        GameHelper.OnThemeChanged -= ApplyTheme;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.L))
        {
            SetTheme(Themes.Light);
        }
        if (Input.GetKey(KeyCode.N))
        {
            SetTheme(Themes.Night);
        }
        
        if (Input.GetKey(KeyCode.C))
        {
            GameHelper.AdjustBoardSize(bgColor);
        }
    }

    public void SetTheme(Themes theme)
    {
        switch (theme)
        {
            case Themes.Light: SetLight(); break;
            case Themes.Night: SetDark(); break;
        }
    }
    
    public void SetLight()
    {
        bgColor.backgroundColor = Color.white;
        backButton.color = _colorGrey;
        
        scoreAndRecordText.color = _colorDark;

        settingsBg.color = Color.white;
        settingsBackButton.color = _colorGrey;
        settingsSoundButton.color = _colorGrey;
        settingsMusicButton.color = _colorGrey;
        settingsVibrationButton.color = _colorGrey;
        settingsThemeText.color = _colorDark;
        settingsLanguageText.color = _colorDark;
    } 
    
    public void SetDark()
    {
        bgColor.backgroundColor = _colorDark;
        backButton.color = _colorLight;
        
        scoreAndRecordText.color = _colorLight;

        settingsBg.color = _colorDark;
        settingsBackButton.color = _colorLight;
        settingsSoundButton.color = _colorLight;
        settingsMusicButton.color = _colorLight;
        settingsVibrationButton.color = _colorLight;
        settingsThemeText.color = _colorLight;
        settingsLanguageText.color = _colorLight;
    }
}