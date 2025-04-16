using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeChineseCheckers: MonoBehaviour
{
    [SerializeField] private Camera bgColor;
    [SerializeField] private Image bgStartPanel;
    [SerializeField] private Image backButton;
    [SerializeField] private Image speedButton;
    [SerializeField] private Image hintButton;
    [SerializeField] private Button undoButton;
    [SerializeField] private Image playButton;
    [SerializeField] private Image playIconButton;
    [SerializeField] private SpriteRenderer field;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private HexMap hexMap;
    [SerializeField] private CheckersManager checkersManager;
    [SerializeField] private List<Image> backgounds;
    
    [SerializeField] private TextMeshProUGUI speedTextButton;
    
    [SerializeField] private Image warningBg;
    [SerializeField] private TextMeshProUGUI warningText;

    private Themes _theme;
    private Color _colorLight;
    private Color _colorSoGrey;
    private Color _colorGrey;
    private Color _colorDark;
    private Color _colorTileLight;
    private Color _colorTileDark;
    
    private Color _lightColorLight;
    private Color _lightColorGrey;
    private Color _lightColorDark;
    
    private Color _colorBgBoard;
    private Color _colorBgField;
    
    private Color _colorBgStartPanelLight;
    private Color _colorBgStartPanelDark;

    private void Start()
    {
        _colorLight = ColorUtility.TryParseHtmlString("#D4D4D8", out Color color) ? color : Color.white;
        _colorSoGrey = ColorUtility.TryParseHtmlString("#939395", out Color color3) ? color3 : Color.gray;
        _colorGrey = ColorUtility.TryParseHtmlString("#454244", out Color color1) ? color1 : Color.gray;
        _colorDark = ColorUtility.TryParseHtmlString("#212022", out Color color2) ? color2 : Color.black;
        _colorTileLight = ColorUtility.TryParseHtmlString("#F2F2F3", out Color color4) ? color4 : Color.white;
        _colorTileDark = ColorUtility.TryParseHtmlString("#606060", out Color color5) ? color5 : Color.black;
        
        _lightColorLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color color6) ? color6 : Color.white;
        _lightColorGrey = ColorUtility.TryParseHtmlString("#CDC1B4", out Color color7) ? color7 : Color.gray;
        _lightColorDark = ColorUtility.TryParseHtmlString("#BBADA0", out Color color8) ? color8 : Color.black;
        
        _colorBgBoard = ColorUtility.TryParseHtmlString("#2C2926", out Color colorBgBoard) ? colorBgBoard : Color.gray;
        _colorBgField = ColorUtility.TryParseHtmlString("#6C6258", out Color colorBgField ) ? colorBgField  : Color.gray;
        
        _colorBgStartPanelLight = ColorUtility.TryParseHtmlString("#EEE4DA", out Color colorBgStartPanelLight) ? colorBgStartPanelLight : Color.white;
        _colorBgStartPanelDark = ColorUtility.TryParseHtmlString("#000000", out Color colorBgStartPanelDark) ? colorBgStartPanelDark : Color.gray;
        
        SetTheme(GameHelper.Theme);
        GameHelper.OnThemeChanged += ApplyTheme;
        GameHelper.GameType = MiniGameType.ChineseCheckers;
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
        bgColor.backgroundColor = _lightColorLight;
        bgStartPanel.color = _colorBgStartPanelLight;
        
        Color c = bgStartPanel.color;
        c.a = 0.7f; // нужная альфа, например, 30%
        bgStartPanel.color  = c;
        
//        backButton.color = _lightColorDark;
//        speedButton.color = _lightColorDark;
//        hintButton.color = _lightColorDark;
        undoButton.image.color = _colorBgBoard;
        
        ColorBlock colors = undoButton.colors;

        // Изменяем только альфу отключенного цвета
        Color disabled = colors.disabledColor;
        disabled.a = 0.2f; // нужная альфа, например, 30%
        colors.disabledColor = disabled;

        undoButton.colors = colors;
        
//        playButton.color = _lightColorDark;
//        playIconButton.color = _lightColorLight;
        field.color = _lightColorDark;
//        
        speedTextButton.color = _lightColorLight;
//        
//        scoreText.color = _lightColorDark;
//        
//        warningBg.color = _lightColorDark;
//        warningText.color = _lightColorLight;
        
        foreach (var bg in backgounds)
        {
            bg.color = _lightColorLight;
        }

//        foreach (var tile in hexMap.Tiles)
//        {
//            tile.SetTheme(_lightColorGrey, Color.white);
//        }
        
        foreach (var person in checkersManager.Players)
        {
            person.SetTheme(_lightColorLight, _colorBgBoard, _lightColorLight);
        }
    } 
    
    public void SetDark()
    {
        bgColor.backgroundColor = _colorBgBoard;
        bgStartPanel.color = _colorBgStartPanelDark;
        
        Color c = bgStartPanel.color;
        c.a = 0.5f; // нужная альфа, например, 30%
        bgStartPanel.color  = c;
        
//        backButton.color = _colorLight;
//        speedButton.color = _colorLight;
//        hintButton.color = _colorLight;
        undoButton.image.color = _lightColorDark;
        
        ColorBlock colors = undoButton.colors;

        // Изменяем только альфу отключенного цвета
        Color disabled = colors.disabledColor;
        disabled.a = 0.3f; // нужная альфа, например, 30%
        colors.disabledColor = disabled;

        undoButton.colors = colors;
        
//        playButton.color = _colorLight;
//        playIconButton.color = _colorDark;
        field.color = _colorBgField;
//        
        speedTextButton.color = _colorBgBoard;
//        
//        scoreText.color = _colorLight;
//        
//        warningBg.color = _colorGrey;
//        warningText.color = _colorLight;
        
        foreach (var bg in backgounds)
        {
            bg.color = _colorBgBoard;
        }

//        foreach (var tile in hexMap.Tiles)
//        {
//            tile.SetTheme(_colorTileDark, Color.white);
//        }
        
        foreach (var person in checkersManager.Players)
        {
            person.SetTheme(_colorBgBoard, _lightColorLight,_colorBgBoard);
        }
    }
}