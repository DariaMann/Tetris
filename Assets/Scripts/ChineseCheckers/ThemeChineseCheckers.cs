using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeChineseCheckers: Theme
{
    [SerializeField] private Camera bgColor;
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

    private Color _colorBgField;
    
    private Color _colorBgStartPanelLight;
    private Color _colorBgStartPanelDark;
    
    private void Awake()
    {
        GameHelper.GameType = MiniGameType.ChineseCheckers;
        GameplayTimeTracker.Instance.RestartTimer();
        InitializeColor();
        _colorBgField = ColorUtility.TryParseHtmlString("#6C6258", out Color colorBgField ) ? colorBgField  : Color.gray;
        
        _colorBgStartPanelLight = ColorUtility.TryParseHtmlString("#EEE4DA", out Color colorBgStartPanelLight) ? colorBgStartPanelLight : Color.white;
        _colorBgStartPanelDark = ColorUtility.TryParseHtmlString("#000000", out Color colorBgStartPanelDark) ? colorBgStartPanelDark : Color.gray;
    }

    public override void SetLight()
    {
        bgColor.backgroundColor = ColorBgLight;
        bgStartPanel.color = _colorBgStartPanelLight;
        
        foreach (var text in lightText)
        {
            text.color = ColorBgLight;
        }
        
        Color c = bgStartPanel.color;
        c.a = 0.7f; // нужная альфа, например, 30%
        bgStartPanel.color  = c;
        
        undoButton.image.color = ColorBgDark;
        
        ColorBlock colors = undoButton.colors;

        // Изменяем только альфу отключенного цвета
        Color disabled = colors.disabledColor;
        disabled.a = 0.2f; // нужная альфа, например, 30%
        colors.disabledColor = disabled;

        undoButton.colors = colors;
        
        field.color = ColorMiddleLight;
        
        foreach (var bg in eduBackgoundsFields)
        {
            bg.color = ColorMiddleLight;
        }

        eduPage1Image.sprite = eduPage1Light;
        
        speedTextButton.color = ColorBgLight;
        
        foreach (var bg in backgounds)
        {
            bg.color = ColorBgLight;
        }
        foreach (var person in checkersManager.Players)
        {
            person.SetTheme(ColorBgLight, ColorBgDark, ColorBgLight);
        }
    } 
    
    public override void SetDark()
    {
        bgColor.backgroundColor = ColorBgDark;
        bgStartPanel.color = _colorBgStartPanelDark;
        
        foreach (var text in lightText)
        {
            text.color = ColorBgDark;
        }
        
        Color c = bgStartPanel.color;
        c.a = 0.5f;
        bgStartPanel.color  = c;
        undoButton.image.color = ColorMiddleLight;
        
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
        
        speedTextButton.color = ColorBgDark;
        
        foreach (var bg in backgounds)
        {
            bg.color = ColorBgDark;
        }

        foreach (var person in checkersManager.Players)
        {
            person.SetTheme(ColorBgDark, ColorBgLight,ColorBgDark);
        }
    }
}