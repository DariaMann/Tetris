using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeSnake : Theme
{
    [SerializeField] private Camera bgColor;
    [SerializeField] private Image eduBgColor;
    [SerializeField] private Image finger;
    [SerializeField] private Sprite fingerLight;
    [SerializeField] private Sprite fingerDark;
    [SerializeField] private SpriteRenderer grid;
    [SerializeField] private List<Image> eduGrid = new List<Image>();
    [SerializeField] private List<Image> buttons = new List<Image>();
    [SerializeField] private List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
    [SerializeField] private List<TextMeshProUGUI> buttonTexts = new List<TextMeshProUGUI>();
    [SerializeField] private List<SpriteRenderer> walls = new List<SpriteRenderer>();
    [SerializeField] private List<Image> eduWalls = new List<Image>();
    
    private Color _colorDark;
    private Color _colorGridLight;
    private Color _colorGridDark;
    
    private Color _colorBgDark;
    
    private Color _colorWallDark;
    
    private void Awake()
    {
        GameHelper.GameType = MiniGameType.Snake;
        InitializeColor();
        _colorDark = ColorUtility.TryParseHtmlString("#9A8C7F", out Color color8) ? color8 : Color.black;
        _colorGridLight = ColorUtility.TryParseHtmlString("#E7DCD0", out Color color3) ? color3 : Color.white;
        _colorGridDark = ColorUtility.TryParseHtmlString("#303030", out Color color4) ? color4 : Color.black;
        
        _colorBgDark = ColorUtility.TryParseHtmlString("#212022", out Color colorBgDark) ? colorBgDark : Color.black;
        
        _colorWallDark = ColorUtility.TryParseHtmlString("#454244", out Color colorWallDark) ? colorWallDark : Color.black;
    }
    
    public override void SetLight()
    {
        bgColor.backgroundColor = ColorBgLight;
        eduBgColor.color = ColorBgLight;
        finger.sprite = fingerLight;
        foreach (var button in buttons)
        {
            button.color = _colorDark;
        }  
        foreach (var text in texts)
        {
            text.color = _colorDark;
        }
        foreach (var text in buttonTexts)
        {
            text.color = ColorBgLight;
        }
        grid.color = _colorGridLight;
        foreach (var cell in eduGrid)
        {
            cell.color = _colorGridLight;
        }
        foreach (var wall in walls)
        {
            wall.color = _colorDark;
        }  
        foreach (var wall in eduWalls)
        {
            wall.color = _colorDark;
        }
    } 
    
    public override void SetDark()
    {
        bgColor.backgroundColor = _colorBgDark;
        eduBgColor.color = _colorBgDark;
        finger.sprite = fingerDark;
        foreach (var button in buttons)
        {
            button.color = ColorBgLight;
        }
        foreach (var text in texts)
        {
            text.color = ColorBgLight;
        }
        foreach (var text in buttonTexts)
        {
            text.color = _colorBgDark;
        }
        grid.color = _colorGridDark;
        foreach (var cell in eduGrid)
        {
            cell.color = _colorGridDark;
        }
        foreach (var wall in walls)
        {
            wall.color = _colorWallDark;
        }   
        foreach (var wall in eduWalls)
        {
            wall.color = _colorWallDark;
        }
    }
}