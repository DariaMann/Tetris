using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ThemeSnake : MonoBehaviour
{
    [SerializeField] private Camera bgColor;
    [SerializeField] private Image backButton;
    [SerializeField] private TextMeshProUGUI scoreAndRecordText;
    [SerializeField] private SpriteRenderer grid;
    [SerializeField] private List<SpriteRenderer> walls = new List<SpriteRenderer>();
    
    private Themes _theme;
    private Color _colorLight;
//    private Color _colorGrey;
    private Color _colorDark;
    private Color _colorGridLight;
    private Color _colorGridDark;
    
    private Color _colorBgLight;
    private Color _colorBgDark;
    
    private Color _colorWallDark;

    private void Start()
    {
//        _colorLight = ColorUtility.TryParseHtmlString("#D4D4D8", out Color color) ? color : Color.white;
//        _colorGrey = ColorUtility.TryParseHtmlString("#454244", out Color color1) ? color1 : Color.gray;
//        _colorDark = ColorUtility.TryParseHtmlString("#212022", out Color color2) ? color2 : Color.black;
//        _colorGridLight = ColorUtility.TryParseHtmlString("#ECECEC", out Color color3) ? color3 : Color.white;
//        _colorGridDark = ColorUtility.TryParseHtmlString("#303030", out Color color4) ? color4 : Color.black;
        
        _colorLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color color6) ? color6 : Color.white;
//        _colorGrey = ColorUtility.TryParseHtmlString("#CDC1B4", out Color color7) ? color7 : Color.gray;
        _colorDark = ColorUtility.TryParseHtmlString("#9A8C7F", out Color color8) ? color8 : Color.black;
        _colorGridLight = ColorUtility.TryParseHtmlString("#E7DCD0", out Color color3) ? color3 : Color.white;
        _colorGridDark = ColorUtility.TryParseHtmlString("#303030", out Color color4) ? color4 : Color.black;
        
        _colorBgLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color colorBgLight) ? colorBgLight : Color.white;
        _colorBgDark = ColorUtility.TryParseHtmlString("#212022", out Color colorBgDark) ? colorBgDark : Color.black;
        
        _colorWallDark = ColorUtility.TryParseHtmlString("#454244", out Color colorWallDark) ? colorWallDark : Color.black;
        
        SetTheme(GameHelper.Theme);
        GameHelper.OnThemeChanged += ApplyTheme;
        GameHelper.GameType = MiniGameType.Snake;
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
            case Themes.Light: SetLight(); break;
            case Themes.Night: SetDark(); break;
        }
    }
    
    public void SetLight()
    {
        bgColor.backgroundColor = _colorBgLight;
        backButton.color = _colorDark;
        grid.color = _colorGridLight;
        scoreAndRecordText.color = _colorDark;
        foreach (var wall in walls)
        {
            wall.color = _colorDark;
        }
    } 
    
    public void SetDark()
    {
        bgColor.backgroundColor = _colorBgDark;
        backButton.color = _colorLight;
        grid.color = _colorGridDark;
        scoreAndRecordText.color = _colorLight;
        foreach (var wall in walls)
        {
            wall.color = _colorWallDark;
        }
    }
}