using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ThemeSnake : MonoBehaviour
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
    
    private Themes _theme;
    private Color _colorLight;
    private Color _colorDark;
    private Color _colorGridLight;
    private Color _colorGridDark;
    
    private Color _colorBgLight;
    private Color _colorBgDark;
    
    private Color _colorWallDark;
    
    private void Awake()
    {
        GameHelper.GameType = MiniGameType.Snake;
    }

    private void Start()
    {
        _colorLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color color6) ? color6 : Color.white;
        _colorDark = ColorUtility.TryParseHtmlString("#9A8C7F", out Color color8) ? color8 : Color.black;
        _colorGridLight = ColorUtility.TryParseHtmlString("#E7DCD0", out Color color3) ? color3 : Color.white;
        _colorGridDark = ColorUtility.TryParseHtmlString("#303030", out Color color4) ? color4 : Color.black;
        
        _colorBgLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color colorBgLight) ? colorBgLight : Color.white;
        _colorBgDark = ColorUtility.TryParseHtmlString("#212022", out Color colorBgDark) ? colorBgDark : Color.black;
        
        _colorWallDark = ColorUtility.TryParseHtmlString("#454244", out Color colorWallDark) ? colorWallDark : Color.black;
        
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
        bgColor.backgroundColor = _colorBgLight;
        eduBgColor.color = _colorBgLight;
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
            text.color = _colorBgLight;
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
    
    public void SetDark()
    {
        bgColor.backgroundColor = _colorBgDark;
        eduBgColor.color = _colorBgDark;
        finger.sprite = fingerDark;
        foreach (var button in buttons)
        {
            button.color = _colorLight;
        }
        foreach (var text in texts)
        {
            text.color = _colorLight;
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