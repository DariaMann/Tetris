using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Theme2048 : MonoBehaviour
{ 
    [SerializeField] private Camera bgColor;
    [SerializeField] private Image scoreBg;
    [SerializeField] private Image recordBg;
    [SerializeField] private Image bgPanelBg;
    [SerializeField] private Image backButton;
    [SerializeField] private List<Image> cells;
    [SerializeField] private List<TextMeshProUGUI> texts;
    
    [SerializeField] private Image settingsBg;
    [SerializeField] private Image settingsBackButton;
    [SerializeField] private TextMeshProUGUI settingsThemeTextButton;
    
    [SerializeField] private Image settingsSoundButton;
    [SerializeField] private Image settingsMusicButton;
    [SerializeField] private Image settingsVibrationButton;

    private Themes _theme;
    private Color _lightColorLight;
    private Color _lightColorGrey;
    private Color _lightColorDark;
    
    private Color _colorLight;
    private Color _colorCells;
    private Color _colorGrey;
    private Color _colorDark;

    private void Awake()
    {
        _lightColorLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color color) ? color : Color.white;
        _lightColorGrey = ColorUtility.TryParseHtmlString("#CDC1B4", out Color color1) ? color1 : Color.gray;
        _lightColorDark = ColorUtility.TryParseHtmlString("#BBADA0", out Color color2) ? color2 : Color.black;
        
        _colorLight = ColorUtility.TryParseHtmlString("#D4D4D8", out Color color3) ? color3 : Color.white;
        _colorCells = ColorUtility.TryParseHtmlString("##878787", out Color color6) ? color6 : Color.gray;
        _colorGrey = ColorUtility.TryParseHtmlString("#454244", out Color color4) ? color4 : Color.gray;
        _colorDark = ColorUtility.TryParseHtmlString("#212022", out Color color5) ? color5 : Color.black;
        
        SetTheme(GameHelper.Theme);
        GameHelper.OnThemeChanged += ApplyTheme;
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
        bgColor.backgroundColor = _lightColorLight;
        backButton.color = _lightColorDark;
        recordBg.color = _lightColorDark;
        scoreBg.color = _lightColorDark;
        bgPanelBg.color = _lightColorDark;
        foreach (var cell in cells)
        {
            cell.color = _lightColorGrey;
        }
        foreach (var text in texts)
        {
            text.color = _lightColorLight;
        }
        
        settingsBg.color = Color.white;
        settingsBackButton.color = _colorGrey;
        settingsSoundButton.color = _colorGrey;
        settingsMusicButton.color = _colorGrey;
        settingsVibrationButton.color = _colorGrey;
        settingsThemeTextButton.color = _colorDark;
    } 
    
    public void SetDark()
    {
        bgColor.backgroundColor = _colorDark;
        backButton.color = _colorLight;
        recordBg.color = _colorGrey;
        scoreBg.color = _colorGrey;
        bgPanelBg.color = _colorGrey;
        foreach (var cell in cells)
        {
            cell.color = _colorCells;
        }
        foreach (var text in texts)
        {
            text.color = _colorLight;
        }
        
        settingsBg.color = _colorDark;
        settingsBackButton.color = _colorLight;
        settingsSoundButton.color = _colorLight;
        settingsMusicButton.color = _colorLight;
        settingsVibrationButton.color = _colorLight;
        settingsThemeTextButton.color = _colorLight;
    } 
}