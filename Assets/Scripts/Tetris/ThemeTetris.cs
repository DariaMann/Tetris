using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ThemeTetris : MonoBehaviour
{
    [SerializeField] private Camera bgColor;
    [SerializeField] private Image nextFigureBg;
    [SerializeField] private Image countLinesBg;
    [SerializeField] private Image recordBg;
    [SerializeField] private Image buttonDownLeft;
    [SerializeField] private Image buttonDownUp;
    [SerializeField] private SpriteRenderer gridTile;
    [SerializeField] private Image backButton;
    [SerializeField] private Ghost ghost;
    
    [SerializeField] private Tile ghostLightTile;
    [SerializeField] private Tile ghostNightTile;
    
    [SerializeField] private TextMeshProUGUI nextText;
    [SerializeField] private TextMeshProUGUI scoreNameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI recordNameText;
    [SerializeField] private TextMeshProUGUI recordText;
    
    [SerializeField] private Image nextFigureBgRight;
    [SerializeField] private Image countLinesBgRight;
    [SerializeField] private Image recordBgRight;
    [SerializeField] private Image buttonDownRight;
    [SerializeField] private TextMeshProUGUI nextTextRight;
    [SerializeField] private TextMeshProUGUI scoreNameTextRight;
    [SerializeField] private TextMeshProUGUI scoreTextRight;
    [SerializeField] private TextMeshProUGUI recordNameTextRight;
    [SerializeField] private TextMeshProUGUI recordTextRight;
    
    [SerializeField] private TextMeshProUGUI scoreAndRecordTextTop;
    
    private Themes _theme;
    private Color _colorLight;
    private Color _colorGrey;
    private Color _colorDark;
    
    private Color _colorImageLight;
    private Color _colorBgLight;
    private Color _colorBgDark;
    
    private Color _colorGrid;
    private Color _colorSelectLight;

    private void Start()
    {
        _colorLight = ColorUtility.TryParseHtmlString("#D4D4D8", out Color color) ? color : Color.white;
        _colorGrey = ColorUtility.TryParseHtmlString("#454244", out Color color1) ? color1 : Color.gray;
        _colorDark = ColorUtility.TryParseHtmlString("#212022", out Color color2) ? color2 : Color.black;
        
        _colorImageLight = ColorUtility.TryParseHtmlString("#CDC1B4", out Color color8) ? color8 : Color.black;
        
        _colorBgLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color colorBgLight) ? colorBgLight : Color.white;
        _colorBgDark = ColorUtility.TryParseHtmlString("#212022", out Color colorBgDark) ? colorBgDark : Color.black;
        
        _colorGrid = ColorUtility.TryParseHtmlString("#D1CBC4", out Color colorWallDark) ? colorWallDark : Color.black;
        
        _colorSelectLight = ColorUtility.TryParseHtmlString("#2C2926", out Color colorSelectLight) ? colorSelectLight : Color.black;
        
        SetTheme(GameHelper.Theme);
        GameHelper.OnThemeChanged += ApplyTheme;
        GameHelper.GameType = MiniGameType.Tetris;
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
        bgColor.backgroundColor = _colorBgLight;
        backButton.color = _colorImageLight;
        nextFigureBg.color = _colorImageLight;
        countLinesBg.color = _colorImageLight;
        recordBg.color = _colorImageLight;
        buttonDownUp.color = _colorImageLight;
        buttonDownLeft.color = _colorImageLight;
        buttonDownRight.color = _colorImageLight;
        gridTile.color = _colorGrid;
        ghost.Tile.color = _colorSelectLight;
        
        nextText.color = _colorBgLight;
        scoreNameText.color = _colorBgLight;
        scoreText.color = _colorBgLight;
        recordNameText.color = _colorBgLight;
        recordText.color = _colorBgLight;
        
        nextFigureBgRight.color = _colorImageLight;
        countLinesBgRight.color = _colorImageLight;
        recordBgRight.color = _colorImageLight;
        nextTextRight.color = _colorBgLight;
        scoreNameTextRight.color = _colorBgLight;
        scoreTextRight.color = _colorBgLight;
        recordNameTextRight.color = _colorBgLight;
        recordTextRight.color = _colorBgLight;
        
        scoreAndRecordTextTop.color = _colorImageLight;
    } 
    
    public void SetDark()
    {
        bgColor.backgroundColor = _colorDark;
        backButton.color = _colorLight;
        nextFigureBg.color = _colorGrey;
        countLinesBg.color = _colorGrey;
        buttonDownUp.color = _colorGrey;
        buttonDownLeft.color = _colorGrey;
        buttonDownRight.color = _colorGrey;
        recordBg.color = _colorGrey;
        gridTile.color = _colorGrey;
        ghost.Tile.color = _colorLight;
        
        nextText.color = _colorLight;
        scoreNameText.color = _colorLight;
        scoreText.color = _colorLight;
        recordNameText.color = _colorLight;
        recordText.color = _colorLight;
        
        nextFigureBgRight.color = _colorGrey;
        countLinesBgRight.color = _colorGrey;
        recordBgRight.color = _colorGrey;
        nextTextRight.color = _colorLight;
        scoreNameTextRight.color = _colorLight;
        scoreTextRight.color = _colorLight;
        recordNameTextRight.color = _colorLight;
        recordTextRight.color = _colorLight;
        
        scoreAndRecordTextTop.color = _colorLight;
    }
}