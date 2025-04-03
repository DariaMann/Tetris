using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeChineseCheckers: MonoBehaviour
{
    [SerializeField] private Camera bgColor;
    [SerializeField] private Image backButton;
    [SerializeField] private Image speedButton;
    [SerializeField] private Image hintButton;
    [SerializeField] private Image undoButton;
    [SerializeField] private Image playButton;
    [SerializeField] private Image playIconButton;
    [SerializeField] private SpriteRenderer field;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private HexMap hexMap;
    [SerializeField] private CheckersManager checkersManager;
    
    [SerializeField] private TextMeshProUGUI speedTextButton;
    
    [SerializeField] private Image warningBg;
    [SerializeField] private TextMeshProUGUI warningText;
    
    [SerializeField] private Image settingsBg;
    [SerializeField] private Image settingsBackButton;
    [SerializeField] private TextMeshProUGUI settingsThemeText;
    [SerializeField] private TextMeshProUGUI settingsLanguageText;
    
    [SerializeField] private Image settingsSoundButton;
    [SerializeField] private Image settingsMusicButton;
    [SerializeField] private Image settingsVibrationButton;
    
    [SerializeField] private Color colorLight;
    [SerializeField] private Color colorSoGrey;
    [SerializeField] private Color colorGrey;
    [SerializeField] private Color colorDark;

    private Themes _theme;
    private Color _colorLight;
    private Color _colorSoGrey;
    private Color _colorGrey;
    private Color _colorDark;
    private Color _colorTileLight;
    private Color _colorTileDark;

    private void Awake()
    {
        _colorLight = ColorUtility.TryParseHtmlString("#D4D4D8", out Color color) ? color : Color.white;
        _colorSoGrey = ColorUtility.TryParseHtmlString("#939395", out Color color3) ? color3 : Color.gray;
        _colorGrey = ColorUtility.TryParseHtmlString("#454244", out Color color1) ? color1 : Color.gray;
        _colorDark = ColorUtility.TryParseHtmlString("#212022", out Color color2) ? color2 : Color.black;
        _colorTileLight = ColorUtility.TryParseHtmlString("#F2F2F3", out Color color4) ? color4 : Color.white;
        _colorTileDark = ColorUtility.TryParseHtmlString("#606060", out Color color5) ? color5 : Color.black;
        
        SetTheme(GameHelper.Theme);
        GameHelper.OnThemeChanged += ApplyTheme;
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
        bgColor.backgroundColor = Color.white;
        backButton.color = _colorDark;
        speedButton.color = _colorDark;
        hintButton.color = _colorDark;
        undoButton.color = _colorDark;
        playButton.color = _colorDark;
        playIconButton.color = _colorLight;
        field.color = _colorLight;
        
        speedTextButton.color = Color.white;
        
        scoreText.color = _colorDark;
        
        warningBg.color = _colorLight;
        warningText.color = _colorDark;

        foreach (var tile in hexMap.Tiles)
        {
            tile.SetTheme(_colorTileLight, _colorDark);
        }
        
        foreach (var person in checkersManager.Players)
        {
            person.SetTheme(Color.white);
        }
        
        settingsBg.color = Color.white;
        settingsBackButton.color = _colorGrey;
        settingsSoundButton.color = _colorGrey;
        settingsMusicButton.color = _colorGrey;
        settingsVibrationButton.color = _colorGrey;
        settingsThemeText.color = _colorDark;
        settingsLanguageText.color = _colorDark;
    } 
    
    public void SetDark()
    {
        bgColor.backgroundColor = _colorDark;
        backButton.color = _colorLight;
        speedButton.color = _colorLight;
        hintButton.color = _colorLight;
        undoButton.color = _colorLight;
        playButton.color = _colorLight;
        playIconButton.color = _colorDark;
        field.color = _colorGrey;
        
        speedTextButton.color = _colorDark;
        
        scoreText.color = _colorLight;
        
        warningBg.color = _colorGrey;
        warningText.color = _colorLight;

        foreach (var tile in hexMap.Tiles)
        {
            tile.SetTheme(_colorTileDark, Color.white);
        }
        
        foreach (var person in checkersManager.Players)
        {
            person.SetTheme(_colorDark);
        }
        
        settingsBg.color = _colorDark;
        settingsBackButton.color = _colorLight;
        settingsSoundButton.color = _colorLight;
        settingsMusicButton.color = _colorLight;
        settingsVibrationButton.color = _colorLight;
        settingsThemeText.color = _colorLight;
        settingsLanguageText.color = _colorLight;
    }
}