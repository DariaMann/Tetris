using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Theme2048 : Theme
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

    private Color _colorCellLight;
    private Color _colorCellsDark;
    
    private Color _colorBgBoardDark;
    
    private void Awake()
    {
        GameHelper.GameType = MiniGameType.G2048;
        InitializeColor();
        _colorCellLight = ColorUtility.TryParseHtmlString("#CDC1B4", out Color colorCellLight) ? colorCellLight : Color.gray;
        _colorCellsDark = ColorUtility.TryParseHtmlString("#B7A693", out Color colorCellsDark) ? colorCellsDark : Color.gray;

        _colorBgBoardDark = ColorUtility.TryParseHtmlString("#877564", out Color colorBgBoardDark) ? colorBgBoardDark : Color.gray;
    }

    public override void SetLight()
    {
        bgColor.backgroundColor = ColorBgLight;
        eduBgColor.color = ColorBgLight;
        foreach (var text in texts)
        {
            text.color = ColorBgLight;
        }
        
        finger.sprite = lightFingerSprite;
        undoButton.image.color = ColorBgDark;
        
        ColorBlock colors = undoButton.colors;

        // Изменяем только альфу отключенного цвета
        Color disabled = colors.disabledColor;
        disabled.a = 0.2f; // нужная альфа, например, 30%
        colors.disabledColor = disabled;

        undoButton.colors = colors;
        
        eduBgPanelBg.color = ColorMiddleLight;
        bgPanelBg.color = ColorMiddleLight;
        foreach (var cell in cells)
        {
            cell.color = _colorCellLight;
        }
    } 
    
    public override void SetDark()
    {
        bgColor.backgroundColor = ColorBgDark;
        eduBgColor.color = ColorBgDark;
        foreach (var text in texts)
        {
            text.color = ColorBgDark;
        }
        
        finger.sprite = darkFingerSprite;
        undoButton.image.color = ColorMiddleLight;
        
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