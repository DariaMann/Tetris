using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeLines98: MonoBehaviour
{
    [SerializeField] private Camera bgColor;
    [SerializeField] private Button undoButton;
    [SerializeField] private List<TextMeshProUGUI> lightText;
    
    [SerializeField] private LineBoard lineBoard;
    [SerializeField] private Sprite lightTileSprite;
    [SerializeField] private Sprite darkTileSprite;

    private Themes _theme;
    private Color _colorLight;
    private Color _colorGrey;
    private Color _colorDark;

    private void Awake()
    {
        GameHelper.GameType = MiniGameType.Lines98;
    }

    private void Start()
    {
        _colorLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color color6) ? color6 : Color.white;
        _colorGrey = ColorUtility.TryParseHtmlString("#BBADA0", out Color color7) ? color7 : Color.gray;
        _colorDark = ColorUtility.TryParseHtmlString("#2C2926", out Color color8) ? color8 : Color.black;
        
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
        
        if (Input.GetKey(KeyCode.C))
        {
            GameHelper.AdjustBoardSize(bgColor);
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
        foreach (var text in lightText)
        {
            text.color = _colorLight;
        }
        
        undoButton.image.color = _colorDark;
        
        ColorBlock colors = undoButton.colors;

        // Изменяем только альфу отключенного цвета
        Color disabled = colors.disabledColor;
        disabled.a = 0.2f; // нужная альфа, например, 30%
        colors.disabledColor = disabled;

        undoButton.colors = colors;

        foreach (var tile in lineBoard.Tiles)
        {
            tile.SetTheme(lightTileSprite);
        }
    } 
    
    public void SetDark()
    {
        bgColor.backgroundColor = _colorDark;
        foreach (var text in lightText)
        {
            text.color = _colorDark;
        }
        
        undoButton.image.color = _colorGrey;
        
        ColorBlock colors = undoButton.colors;

        // Изменяем только альфу отключенного цвета
        Color disabled = colors.disabledColor;
        disabled.a = 0.3f; // нужная альфа, например, 30%
        colors.disabledColor = disabled;

        undoButton.colors = colors;
        
        foreach (var tile in lineBoard.Tiles)
        {
            tile.SetTheme(darkTileSprite);
        }
    }
}