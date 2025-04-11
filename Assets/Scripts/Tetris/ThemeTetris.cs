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
    [SerializeField] private TextMeshProUGUI nextTextRight;
    [SerializeField] private TextMeshProUGUI scoreNameTextRight;
    [SerializeField] private TextMeshProUGUI scoreTextRight;
    [SerializeField] private TextMeshProUGUI recordNameTextRight;
    [SerializeField] private TextMeshProUGUI recordTextRight;
    
    [SerializeField] private TextMeshProUGUI scoreAndRecordTextTop;
    
    [SerializeField] private List<TextMeshProUGUI> downButtoms = new List<TextMeshProUGUI>();

    private Themes _theme;
    private Color _colorLight;
    private Color _colorGrey;
    private Color _colorDark;

    private void Start()
    {
        _colorLight = ColorUtility.TryParseHtmlString("#D4D4D8", out Color color) ? color : Color.white;
        _colorGrey = ColorUtility.TryParseHtmlString("#454244", out Color color1) ? color1 : Color.gray;
        _colorDark = ColorUtility.TryParseHtmlString("#212022", out Color color2) ? color2 : Color.black;
        
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
        bgColor.backgroundColor = Color.white;
        backButton.color = _colorGrey;
        nextFigureBg.color = _colorLight;
        countLinesBg.color = _colorLight;
        recordBg.color = _colorLight;
        gridTile.color = _colorLight;
        ghost.Tile = ghostNightTile;
        
        nextText.color = _colorDark;
        scoreNameText.color = _colorDark;
        scoreText.color = _colorDark;
        recordNameText.color = _colorDark;
        recordText.color = _colorDark;
        
        nextFigureBgRight.color = _colorLight;
        countLinesBgRight.color = _colorLight;
        recordBgRight.color = _colorLight;
        nextTextRight.color = _colorDark;
        scoreNameTextRight.color = _colorDark;
        scoreTextRight.color = _colorDark;
        recordNameTextRight.color = _colorDark;
        recordTextRight.color = _colorDark;
        
        scoreAndRecordTextTop.color = _colorDark;

        foreach (var down in downButtoms)
        {
            down.color = _colorGrey;
        }
    } 
    
    public void SetDark()
    {
        bgColor.backgroundColor = _colorDark;
        backButton.color = _colorLight;
        nextFigureBg.color = _colorGrey;
        countLinesBg.color = _colorGrey;
        recordBg.color = _colorGrey;
        gridTile.color = _colorGrey;
        ghost.Tile = ghostLightTile;
        
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
        
        foreach (var down in downButtoms)
        {
            down.color = _colorLight;
        }
    }
}