using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Theme2048 : MonoBehaviour
{ 
    [SerializeField] private Camera bgColor;
    [SerializeField] private Image scoreBg;
    [SerializeField] private Image recordBg;
    [SerializeField] private Image bgPanelBg;
    [SerializeField] private Image backButton;
    [SerializeField] private Image undoButton;
    [SerializeField] private TextMeshProUGUI scoreAndRecordText;
    [SerializeField] private TextMeshProUGUI maximumText;
    [SerializeField] private List<Image> cells;
    [SerializeField] private List<TextMeshProUGUI> texts;

    private Themes _theme;
    private Color _lightColorLight;
    private Color _lightColorGrey;
    private Color _lightColorDark;
    
    private Color _colorLight;
    private Color _colorCellsDark;
    
    private Color _colorBgBoard;
    private Color _colorBgDark;

    private void Start()
    {
        _lightColorLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color color) ? color : Color.white;
        _lightColorGrey = ColorUtility.TryParseHtmlString("#CDC1B4", out Color color1) ? color1 : Color.gray;
        _lightColorDark = ColorUtility.TryParseHtmlString("#BBADA0", out Color color2) ? color2 : Color.black;
        
        _colorLight = ColorUtility.TryParseHtmlString("#D4D4D8", out Color color3) ? color3 : Color.white;
        
        _colorCellsDark = ColorUtility.TryParseHtmlString("#B7A693", out Color color6) ? color6 : Color.gray;
        _colorBgBoard = ColorUtility.TryParseHtmlString("#877564", out Color color8) ? color8 : Color.gray;
        _colorBgDark = ColorUtility.TryParseHtmlString("#2C2926", out Color color7) ? color7 : Color.black;
        
        SetTheme(GameHelper.Theme);
        GameHelper.OnThemeChanged += ApplyTheme;
        GameHelper.GameType = MiniGameType.G2048;
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
            case Themes.Light: SetLight(); break;
            case Themes.Night: SetDark(); break;
        }
    }
    
    public void SetLight()
    {
        bgColor.backgroundColor = _lightColorLight;
        backButton.color = _lightColorDark;
        undoButton.color = _lightColorDark;
        recordBg.color = _lightColorDark;
        scoreBg.color = _lightColorDark;
        scoreAndRecordText.color = _lightColorDark;
        maximumText.color = _lightColorDark;
        bgPanelBg.color = _lightColorDark;
        foreach (var cell in cells)
        {
            cell.color = _lightColorGrey;
        }
        foreach (var text in texts)
        {
            text.color = _lightColorLight;
        }
    } 
    
    public void SetDark()
    {
        bgColor.backgroundColor = _colorBgDark;
        backButton.color = _colorLight;
        undoButton.color = _colorLight;
        recordBg.color = _colorBgBoard;
        scoreBg.color = _colorBgBoard;
        scoreAndRecordText.color = _colorLight;
        maximumText.color = _colorLight;
        bgPanelBg.color = _colorBgBoard;
        foreach (var cell in cells)
        {
            cell.color = _colorCellsDark;
        }
        foreach (var text in texts)
        {
            text.color = _colorLight;
        }
    } 
}