using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeSettings : MonoBehaviour
{
    [SerializeField] private Image settingsBg;
    [SerializeField] private Image settingsBackButton;
    [SerializeField] private TextMeshProUGUI settingsThemeText;
    [SerializeField] private TextMeshProUGUI settingsLanguageText;
    
    [SerializeField] private Image settingsRatingButton;
    [SerializeField] private Image settingsSoundButton;
    [SerializeField] private Image settingsMusicButton;
    [SerializeField] private Image settingsVibrationButton;
    
    [SerializeField] private Image settingsLightButton;
    [SerializeField] private Image settingsDarkButton;
    [SerializeField] private TextMeshProUGUI settingsLightText;
    [SerializeField] private TextMeshProUGUI settingsDarkText;
    [SerializeField] private Image settingsLightSelection;
    [SerializeField] private Image settingsDarkSelection;
    
    [SerializeField] private Image settingsEnglishButton;
    [SerializeField] private Image settingsRussianButton;
    [SerializeField] private TextMeshProUGUI settingsEnglishText;
    [SerializeField] private TextMeshProUGUI settingsRussianText;
    [SerializeField] private Image settingsEnglishSelection;
    [SerializeField] private Image settingsRussianSelection;

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
        settingsBg.color = Color.white;
        settingsBackButton.color = _colorGrey;
        settingsRatingButton.color = _colorGrey;
        settingsSoundButton.color = _colorGrey;
        settingsMusicButton.color = _colorGrey;
        settingsVibrationButton.color = _colorGrey;
        settingsThemeText.color = _colorDark;
        settingsLanguageText.color = _colorDark;
        
        settingsLightButton.color = _colorGrey;
        settingsDarkButton.color = _colorGrey;
        settingsLightText.color = _colorLight;
        settingsDarkText.color = _colorLight;
        settingsLightSelection.color = _colorLight;
        settingsDarkSelection.color = _colorLight;
        
        settingsEnglishButton.color = _colorGrey;
        settingsRussianButton.color = _colorGrey;
        settingsEnglishText.color = _colorLight;
        settingsRussianText.color = _colorLight;
        settingsEnglishSelection.color = _colorLight;
        settingsRussianSelection.color = _colorLight;
    } 
    
    public void SetDark()
    {
        settingsBg.color = _colorDark;
        settingsBackButton.color = _colorLight;
        settingsRatingButton.color = _colorLight;
        settingsSoundButton.color = _colorLight;
        settingsMusicButton.color = _colorLight;
        settingsVibrationButton.color = _colorLight;
        settingsThemeText.color = _colorLight;
        settingsLanguageText.color = _colorLight;
        
        settingsLightButton.color = _colorLight;
        settingsDarkButton.color = _colorLight;
        settingsLightText.color = _colorDark;
        settingsDarkText.color = _colorDark;
        settingsLightSelection.color = _colorDark;
        settingsDarkSelection.color = _colorDark;
        
        settingsEnglishButton.color = _colorLight;
        settingsRussianButton.color = _colorLight;
        settingsEnglishText.color = _colorDark;
        settingsRussianText.color = _colorDark;
        settingsEnglishSelection.color = _colorDark;
        settingsRussianSelection.color = _colorDark;
    }
}