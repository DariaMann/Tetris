using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeSettings : MonoBehaviour
{
    [SerializeField] private RectTransform layoutGroupParent;
    [SerializeField] private ScrollRect scrollRect;
    
    [SerializeField] private Image settingsBg;
    [SerializeField] private Image settingsBackBg;
    
    [SerializeField] private List<Image> lightImages = new List<Image>();
    [SerializeField] private List<TextMeshProUGUI> lightTexts = new List<TextMeshProUGUI>();
    
    [SerializeField] private List<Image> middleImages = new List<Image>();
    [SerializeField] private List<TextMeshProUGUI> middleTexts = new List<TextMeshProUGUI>();

    [SerializeField] private GameObject snakeSettings;
    [SerializeField] private Toggle manyFoodToggle;
    [SerializeField] private Toggle moveThroughWallsToggle;
    [SerializeField] private Toggle accelerationToggle;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private Slider speed;
    [SerializeField] private TextMeshProUGUI textSlider;
    
    [SerializeField] private GameObject tetrisSettings;
    [SerializeField] private Toggle accelerationTetrisToggle;
    [SerializeField] private TextMeshProUGUI speedTetrisText;
    [SerializeField] private Slider speedTetris;
    [SerializeField] private TextMeshProUGUI textTetrisSlider;

    private Themes _theme;
    private Color _colorLight;
    private Color _colorDark;
    
    private Color _colorBgLight;
    private Color _colorBgDark;

    private void Start()
    {
//        _colorLight = ColorUtility.TryParseHtmlString("#D4D4D8", out Color color) ? color : Color.white;
//        _colorGrey = ColorUtility.TryParseHtmlString("#454244", out Color color1) ? color1 : Color.gray;
//        _colorDark = ColorUtility.TryParseHtmlString("#212022", out Color color2) ? color2 : Color.black;

//        _colorBgLight = Color.white;
//        _colorBgDark = ColorUtility.TryParseHtmlString("#212022", out Color colorBgDark1) ? colorBgDark1 : Color.black;

        _colorLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color color6) ? color6 : Color.white;
        _colorDark = ColorUtility.TryParseHtmlString("#9A8C7F", out Color color8) ? color8 : Color.black;
        
        _colorBgLight = ColorUtility.TryParseHtmlString("#FAF8EF", out Color colorBgLight) ? colorBgLight : Color.white;
        _colorBgDark = ColorUtility.TryParseHtmlString("#2C2926", out Color colorBgDark) ? colorBgDark : Color.black;
        
        
        SetTheme(GameHelper.Theme);
        GameHelper.OnThemeChanged += ApplyTheme;
        
        if (GameHelper.GameType == MiniGameType.Snake)
        {
            tetrisSettings.SetActive(false);
            snakeSettings.SetActive(true);
            // Сначала отключаем обработчики
            manyFoodToggle.onValueChanged.RemoveListener(OnManyFoodsToggle);
            moveThroughWallsToggle.onValueChanged.RemoveListener(OnMoveThroughWallsToggle);
            accelerationToggle.onValueChanged.RemoveListener(OnAccelerationToggle);
            speed.onValueChanged.RemoveListener(OnSpeedSlider);

            // Устанавливаем значения
            SetSnakeSettings();

            // Подключаем обработчики обратно
            manyFoodToggle.onValueChanged.AddListener(OnManyFoodsToggle);
            moveThroughWallsToggle.onValueChanged.AddListener(OnMoveThroughWallsToggle);
            accelerationToggle.onValueChanged.AddListener(OnAccelerationToggle);
            speed.onValueChanged.RemoveListener(OnSpeedSlider);
        }
        else if (GameHelper.GameType == MiniGameType.Tetris)
        {
            snakeSettings.SetActive(false);
            tetrisSettings.SetActive(true);
            // Сначала отключаем обработчики
            accelerationTetrisToggle.onValueChanged.RemoveListener(OnAccelerationTetrisToggle);
            speedTetris.onValueChanged.RemoveListener(OnSpeedTetrisSlider);

            // Устанавливаем значения
            SetTetrisSettings();

            // Подключаем обработчики обратно
            accelerationTetrisToggle.onValueChanged.AddListener(OnAccelerationTetrisToggle);
            speedTetris.onValueChanged.RemoveListener(OnSpeedTetrisSlider);
        }
        else
        {
            snakeSettings.SetActive(false);
            tetrisSettings.SetActive(false);
        }
        
        RefreshUI();
    }
    
    private void ApplyTheme(Themes newTheme)
    {
        Debug.Log($"Theme changed to {newTheme.ToString()}");
        // Здесь можно добавить код смены оформления игры
        SetTheme(newTheme);
    }
    
    public void OnDestroy()
    {
        GameHelper.OnThemeChanged -= ApplyTheme;
    }

    public void SetSnakeSettings()
    {
        manyFoodToggle.isOn = GameHelper.SnakeSettings.ManyFood;
        moveThroughWallsToggle.isOn = GameHelper.SnakeSettings.MoveThroughWalls;
        accelerationToggle.isOn = GameHelper.SnakeSettings.Acceleration;
        int speedType = GameHelper.GetTypeBySpeedSnake(GameHelper.SnakeSettings.Speed);
        speed.value = speedType;
        textSlider.text = speedType.ToString();
        ShowSnakeSpeedParameters(!GameHelper.SnakeSettings.Acceleration);
    }
    
    public void OnManyFoodsToggle(bool change)
    {
        GameHelper.SnakeSettings.ManyFood = change;
        JsonHelper.SaveSnakeSettings(GameHelper.SnakeSettings);
    }
    
    public void OnMoveThroughWallsToggle(bool change)
    {
        GameHelper.SnakeSettings.MoveThroughWalls = change;
        JsonHelper.SaveSnakeSettings(GameHelper.SnakeSettings);
    }
    
    public void OnAccelerationToggle(bool change)
    {
        GameHelper.SnakeSettings.Acceleration = change;
        JsonHelper.SaveSnakeSettings(GameHelper.SnakeSettings);
        ShowSnakeSpeedParameters(!change);
    }
    
    public void OnSpeedSlider(Single change)
    {
        int speedType = (int) change;
        GameHelper.SnakeSettings.Speed = GameHelper.GetSpeedByTypeSnake(speedType);
        textSlider.text = speedType.ToString();
        JsonHelper.SaveSnakeSettings(GameHelper.SnakeSettings);
    }
    
    public void ShowSnakeSpeedParameters(bool isShow)
    {
        speedText.gameObject.SetActive(isShow);
        speed.gameObject.SetActive(isShow);
        RefreshUI();
    }
    
    public void SetTetrisSettings()
    {
        accelerationTetrisToggle.isOn = GameHelper.TetrisSettings.Acceleration;
        int speedType = GameHelper.GetTypeBySpeedTetris(GameHelper.TetrisSettings.Speed);
        speedTetris.value = speedType;
        textTetrisSlider.text = speedType.ToString();
        ShowTetrisSpeedParameters(!GameHelper.TetrisSettings.Acceleration);
    }
    
    public void OnAccelerationTetrisToggle(bool change)
    {
        GameHelper.TetrisSettings.Acceleration = change;
        JsonHelper.SaveTetrisSettings(GameHelper.TetrisSettings);
        ShowTetrisSpeedParameters(!change);
    }
    
    public void OnSpeedTetrisSlider(Single change)
    {
        int speedType = (int) change;
        GameHelper.TetrisSettings.Speed = GameHelper.GetSpeedByTypeTetris(speedType);
        textTetrisSlider.text = speedType.ToString();
        JsonHelper.SaveTetrisSettings(GameHelper.TetrisSettings);
    }
    
    public void ShowTetrisSpeedParameters(bool isShow)
    {
        speedTetrisText.gameObject.SetActive(isShow);
        speedTetris.gameObject.SetActive(isShow);
        RefreshUI();
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
        settingsBg.color = _colorBgLight;
        settingsBackBg.color = _colorBgLight;
//        settingsBackButton.color = _colorGrey;
//        settingsRatingButton.color = _colorGrey;
//        settingsSoundButton.color = _colorGrey;
//        settingsMusicButton.color = _colorGrey;
//        settingsVibrationButton.color = _colorGrey;
//        settingsThemeText.color = _colorDark;
//        settingsLanguageText.color = _colorDark;
//        
//        settingsLightButton.color = _colorGrey;
//        settingsDarkButton.color = _colorGrey;
//        settingsLightText.color = _colorLight;
//        settingsDarkText.color = _colorLight;
//        settingsLightSelection.color = _colorLight;
//        settingsDarkSelection.color = _colorLight;
//        
//        settingsEnglishButton.color = _colorGrey;
//        settingsRussianButton.color = _colorGrey;
//        settingsEnglishText.color = _colorLight;
//        settingsRussianText.color = _colorLight;
//        settingsEnglishSelection.color = _colorLight;
//        settingsRussianSelection.color = _colorLight;

        foreach (var light in lightImages)
        {
            light.color = _colorLight;
        }
        foreach (var light in lightTexts)
        {
            light.color = _colorLight;
        }
        
        foreach (var light in middleImages)
        {
            light.color = _colorDark;
        }
        foreach (var light in middleTexts)
        {
            light.color = _colorDark;
        }   
//        
//        foreach (var light in darkImages)
//        {
//            light.color = _colorGrey;
//        }
//        foreach (var light in darkTexts)
//        {
//            light.color = _colorGrey;
//        }
    } 
    
    public void SetDark()
    {
        settingsBg.color = _colorBgDark;
        settingsBackBg.color = _colorBgDark;
//        settingsBackButton.color = _colorLight;
//        settingsRatingButton.color = _colorLight;
//        settingsSoundButton.color = _colorLight;
//        settingsMusicButton.color = _colorLight;
//        settingsVibrationButton.color = _colorLight;
//        settingsThemeText.color = _colorLight;
//        settingsLanguageText.color = _colorLight;
//        
//        settingsLightButton.color = _colorLight;
//        settingsDarkButton.color = _colorLight;
//        settingsLightText.color = _colorDark;
//        settingsDarkText.color = _colorDark;
//        settingsLightSelection.color = _colorDark;
//        settingsDarkSelection.color = _colorDark;
//        
//        settingsEnglishButton.color = _colorLight;
//        settingsRussianButton.color = _colorLight;
//        settingsEnglishText.color = _colorDark;
//        settingsRussianText.color = _colorDark;
//        settingsEnglishSelection.color = _colorDark;
//        settingsRussianSelection.color = _colorDark;

        foreach (var light in lightImages)
        {
            light.color = _colorBgDark;
        }
        foreach (var light in lightTexts)
        {
            light.color = _colorBgDark;
        }
        
        foreach (var light in middleImages)
        {
            light.color = _colorLight;
        }
        foreach (var light in middleTexts)
        {
            light.color = _colorLight;
        }   
//        
//        foreach (var light in darkImages)
//        {
//            light.color = _colorGrey;
//        }
//        foreach (var light in darkTexts)
//        {
//            light.color = _colorGrey;
//        }
    }
    
    public void RefreshUI()
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroupParent);
        scrollRect.verticalNormalizedPosition = 1f; // Обновление скролла
    }
}