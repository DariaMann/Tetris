using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeMenu : MonoBehaviour
{
    [SerializeField] private Camera bgColor;
    [SerializeField] private Image settingsImage;
    [SerializeField] private List<Image> buttonsBg;
    [SerializeField] private List<TextMeshProUGUI> textsBg;
    
    [SerializeField] private Image settingsBg;
    [SerializeField] private Image settingsBackButton;
    [SerializeField] private TextMeshProUGUI settingsThemeTextButton;
    
    [SerializeField] private Image settingsSoundButton;
    [SerializeField] private Image settingsMusicButton;
    [SerializeField] private Image settingsVibrationButton;

    private Color _colorLight;
    private Color _colorGrey;
    private Color _colorDark;

    private void Awake()
    {
        _colorLight = ColorUtility.TryParseHtmlString("#D4D4D8", out Color color) ? color : Color.white;
        _colorGrey = ColorUtility.TryParseHtmlString("#454244", out Color color1) ? color1 : Color.gray;
        _colorDark = ColorUtility.TryParseHtmlString("#212022", out Color color2) ? color2 : Color.black;

        GameHelper.SetFirstSettings();
        GameHelper.GetTheme();
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
        bgColor.backgroundColor = Color.white;
        settingsImage.color = _colorDark;
        foreach (var button in buttonsBg)
        {
            button.color = _colorLight;
        }
        foreach (var text in textsBg)
        {
            text.color = _colorDark;
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
        settingsImage.color = _colorLight;
        foreach (var button in buttonsBg)
        {
            button.color = _colorGrey;
        }
        foreach (var text in textsBg)
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