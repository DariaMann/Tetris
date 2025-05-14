using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemePause: MonoBehaviour
{
    [SerializeField] private List<Image> bgButtons;
    
    private Themes _theme;
    private Color _colorLight;
    private Color _colorDark;
    
    private void Start()
    {
        _colorLight = ColorUtility.TryParseHtmlString("#BBADA0", out Color color6) ? color6 : Color.white;
        _colorDark = ColorUtility.TryParseHtmlString("#7E6B59", out Color color8) ? color8 : Color.black;
        
        SetTheme(GameHelper.Theme);
        GameHelper.OnThemeChanged += ApplyTheme;
    }
    
    private void ApplyTheme(Themes newTheme)
    {
        Console.WriteLine($"Theme changed to {newTheme.ToString()}");
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
        foreach (var button in bgButtons)
        {
            button.color = _colorLight;
        }
    } 
    
    public void SetDark()
    {
        foreach (var button in bgButtons)
        {
            button.color = _colorDark;
        }
    }
}