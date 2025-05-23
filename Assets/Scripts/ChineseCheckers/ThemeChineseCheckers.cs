using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeChineseCheckers: MonoBehaviour
{
    [SerializeField] private Camera bgColor;
    [SerializeField] private Image eduBgColor;
    [SerializeField] private Image bgStartPanel;
    [SerializeField] private Button undoButton;
    [SerializeField] private SpriteRenderer field;
    [SerializeField] private CheckersManager checkersManager;
    [SerializeField] private List<Image> backgounds;
    
    [SerializeField] private List<Image> eduBackgoundsFields;
    [SerializeField] private List<TextMeshProUGUI> lightText;
    
    [SerializeField] private Image eduPage1Image;
    [SerializeField] private Sprite eduPage1Light;
    [SerializeField] private Sprite eduPage1Dark;
    
    [SerializeField] private TextMeshProUGUI speedTextButton;

    private Themes _theme;
    
    private Color _lightColorLight;
    private Color _lightColorDark;
    
    private Color _colorBgBoard;
    private Color _colorBgField;
    
    private Color _colorBgStartPanelLight;
    private Color _colorBgStartPanelDark;
    
    private void Awake()
    {
        GameHelper.GameType = MiniGameType.ChineseCheckers;
    }

    private void Start()
    {
        _lightColorLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color color6) ? color6 : Color.white;
        _lightColorDark = ColorUtility.TryParseHtmlString("#BBADA0", out Color color8) ? color8 : Color.black;
        
        _colorBgBoard = ColorUtility.TryParseHtmlString("#2C2926", out Color colorBgBoard) ? colorBgBoard : Color.gray;
        _colorBgField = ColorUtility.TryParseHtmlString("#6C6258", out Color colorBgField ) ? colorBgField  : Color.gray;
        
        _colorBgStartPanelLight = ColorUtility.TryParseHtmlString("#EEE4DA", out Color colorBgStartPanelLight) ? colorBgStartPanelLight : Color.white;
        _colorBgStartPanelDark = ColorUtility.TryParseHtmlString("#000000", out Color colorBgStartPanelDark) ? colorBgStartPanelDark : Color.gray;
        
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
        eduBgColor.color = _lightColorLight;
        bgStartPanel.color = _colorBgStartPanelLight;
        
        foreach (var text in lightText)
        {
            text.color = _lightColorLight;
        }
        
        Color c = bgStartPanel.color;
        c.a = 0.7f; // нужная альфа, например, 30%
        bgStartPanel.color  = c;
        
        undoButton.image.color = _colorBgBoard;
        
        ColorBlock colors = undoButton.colors;

        // Изменяем только альфу отключенного цвета
        Color disabled = colors.disabledColor;
        disabled.a = 0.2f; // нужная альфа, например, 30%
        colors.disabledColor = disabled;

        undoButton.colors = colors;
        
        field.color = _lightColorDark;
        
        foreach (var bg in eduBackgoundsFields)
        {
            bg.color = _lightColorDark;
        }

        eduPage1Image.sprite = eduPage1Light;
        
        speedTextButton.color = _lightColorLight;
        
        foreach (var bg in backgounds)
        {
            bg.color = _lightColorLight;
        }
        foreach (var person in checkersManager.Players)
        {
            person.SetTheme(_lightColorLight, _colorBgBoard, _lightColorLight);
        }
    } 
    
    public void SetDark()
    {
        bgColor.backgroundColor = _colorBgBoard;
        eduBgColor.color = _colorBgBoard;
        bgStartPanel.color = _colorBgStartPanelDark;
        
        foreach (var text in lightText)
        {
            text.color = _colorBgBoard;
        }
        
        Color c = bgStartPanel.color;
        c.a = 0.5f;
        bgStartPanel.color  = c;
        undoButton.image.color = _lightColorDark;
        
        ColorBlock colors = undoButton.colors;
        
        Color disabled = colors.disabledColor;
        disabled.a = 0.3f;
        colors.disabledColor = disabled;

        undoButton.colors = colors;
        
        field.color = _colorBgField;
        
        foreach (var bg in eduBackgoundsFields)
        {
            bg.color = _colorBgField;
        }
        
        eduPage1Image.sprite = eduPage1Dark;
        
        speedTextButton.color = _colorBgBoard;
        
        foreach (var bg in backgounds)
        {
            bg.color = _colorBgBoard;
        }

        foreach (var person in checkersManager.Players)
        {
            person.SetTheme(_colorBgBoard, _lightColorLight,_colorBgBoard);
        }
    }
}