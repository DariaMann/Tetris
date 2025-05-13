using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Theme2048 : MonoBehaviour
{ 
    [SerializeField] private Camera bgColor;
    [SerializeField] private Button undoButton;
    [SerializeField] private Image bgPanelBg;

    [SerializeField] private List<Image> cells;
    [SerializeField] private List<TextMeshProUGUI> texts;
    
    [SerializeField] private Image eduBgPanelBg;
    [SerializeField] private Image eduBgColor;
    [SerializeField] private Image finger;
    [SerializeField] private Sprite lightFingerSprite;
    [SerializeField] private Sprite darkFingerSprite;

    private Themes _theme;
    private Color _colorLight;
    private Color _colorGrey;
    private Color _colorDark;
    
    private Color _colorCellLight;
    private Color _colorCellsDark;
    
    private Color _colorBgBoardDark;
    
    private void Awake()
    {
        GameHelper.GameType = MiniGameType.G2048;
    }

    private void Start()
    {
        _colorCellLight = ColorUtility.TryParseHtmlString("#CDC1B4", out Color colorCellLight) ? colorCellLight : Color.gray;

        _colorCellsDark = ColorUtility.TryParseHtmlString("#B7A693", out Color colorCellsDark) ? colorCellsDark : Color.gray;
        _colorBgBoardDark = ColorUtility.TryParseHtmlString("#877564", out Color colorBgBoardDark) ? colorBgBoardDark : Color.gray;
        
        _colorLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color colorLight) ? colorLight : Color.white;
        _colorGrey = ColorUtility.TryParseHtmlString("#BBADA0", out Color colorGrey) ? colorGrey : Color.gray;
        _colorDark = ColorUtility.TryParseHtmlString("#2C2926", out Color colorDark) ? colorDark : Color.black;
        
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
        bgColor.backgroundColor = _colorLight;
        eduBgColor.color = _colorLight;
        foreach (var text in texts)
        {
            text.color = _colorLight;
        }
        
        finger.sprite = lightFingerSprite;
        undoButton.image.color = _colorDark;
        
        ColorBlock colors = undoButton.colors;

        // Изменяем только альфу отключенного цвета
        Color disabled = colors.disabledColor;
        disabled.a = 0.2f; // нужная альфа, например, 30%
        colors.disabledColor = disabled;

        undoButton.colors = colors;
        
        eduBgPanelBg.color = _colorGrey;
        bgPanelBg.color = _colorGrey;
        foreach (var cell in cells)
        {
            cell.color = _colorCellLight;
        }
    } 
    
    public void SetDark()
    {
        bgColor.backgroundColor = _colorDark;
        eduBgColor.color = _colorDark;
        foreach (var text in texts)
        {
            text.color = _colorDark;
        }
        
        finger.sprite = darkFingerSprite;
        undoButton.image.color = _colorGrey;
        
        ColorBlock colors = undoButton.colors;

        // Изменяем только альфу отключенного цвета
        Color disabled = colors.disabledColor;
        disabled.a = 0.3f; // нужная альфа, например, 30%
        colors.disabledColor = disabled;

        undoButton.colors = colors;
        
        eduBgPanelBg.color = _colorBgBoardDark;
        bgPanelBg.color = _colorBgBoardDark;
        foreach (var cell in cells)
        {
            cell.color = _colorCellsDark;
        }
    } 
}