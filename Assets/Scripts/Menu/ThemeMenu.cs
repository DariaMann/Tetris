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

    private Color _colorLight;
//    private Color _colorGrey;
    private Color _colorDark;
    
    private Color _colorBgLight;
    private Color _colorBgDark;
    
    private void Awake()
    {
        GameHelper.GameType = MiniGameType.None;
    }

    private void Start()
    {
//        _colorLight = ColorUtility.TryParseHtmlString("#D4D4D8", out Color color) ? color : Color.white;
//        _colorGrey = ColorUtility.TryParseHtmlString("#454244", out Color color1) ? color1 : Color.gray;
//        _colorDark = ColorUtility.TryParseHtmlString("#212022", out Color color2) ? color2 : Color.black;
        
        _colorLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color color6) ? color6 : Color.white;
//        _colorGrey = ColorUtility.TryParseHtmlString("#CDC1B4", out Color color7) ? color7 : Color.gray;
        _colorDark = ColorUtility.TryParseHtmlString("#9A8C7F", out Color color8) ? color8 : Color.black;
        
        _colorBgLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color colorBgLight) ? colorBgLight : Color.white;
        _colorBgDark = ColorUtility.TryParseHtmlString("#2C2926", out Color colorBgDark) ? colorBgDark : Color.black;

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
            case Themes.Auto: SetAuto(); break;
            case Themes.Light: SetLight(); break;
            case Themes.Night: SetDark(); break;
        }
    }

    public void SetAuto()
    {
        bool isDark = ThemeManager.IsSystemDarkTheme();
        if (isDark)
        {
            SetDark();
        }
        else
        {
            SetLight();
        }
    }
    
    public void SetLight()
    {
        bgColor.backgroundColor = _colorBgLight;
        settingsImage.color = _colorLight;
        foreach (var button in buttonsBg)
        {
            button.color = _colorDark;
        }
        foreach (var text in textsBg)
        {
            text.color = _colorLight;
        }
    } 
    
    public void SetDark()
    {
        bgColor.backgroundColor = _colorBgDark;
        settingsImage.color = _colorBgDark;
        foreach (var button in buttonsBg)
        {
            button.color = _colorLight;
        }
        foreach (var text in textsBg)
        {
            text.color = _colorBgDark;
        }
    }
}