using System;
using Assets.SimpleLocalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private RectTransform layoutGroupParent;
    [SerializeField] private ScrollRect scrollRect;

    [SerializeField] private GameObject snakeSettings;
    [SerializeField] private Toggle manyFoodToggle;
    [SerializeField] private Toggle moveThroughWallsToggle;
    [SerializeField] private Toggle accelerationToggle;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private Slider speed;
    [SerializeField] private TextMeshProUGUI textSlider;
    [SerializeField] private Image backgroundSnakeSlider;
    [SerializeField] private Image fillSnakeSlider;
    
    [SerializeField] private GameObject tetrisSettings;
    [SerializeField] private Toggle accelerationTetrisToggle;
    [SerializeField] private TextMeshProUGUI speedTetrisText;
    [SerializeField] private Slider speedTetris;
    [SerializeField] private TextMeshProUGUI textTetrisSlider;
    [SerializeField] private Image backgroundTetrisSlider;
    [SerializeField] private Image fillTetrisSlider;

    [SerializeField] private TextMeshProUGUI versionText;

    private void Start()
    {
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
            manyFoodToggle.onValueChanged.AddListener(PlayClickSound);
            moveThroughWallsToggle.onValueChanged.AddListener(OnMoveThroughWallsToggle);
            moveThroughWallsToggle.onValueChanged.AddListener(PlayClickSound);
            accelerationToggle.onValueChanged.AddListener(OnAccelerationToggle);
            accelerationToggle.onValueChanged.AddListener(PlayClickSound);
            speed.onValueChanged.AddListener(OnSpeedSlider);
            speed.onValueChanged.AddListener(PlayClickSound);
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
            accelerationTetrisToggle.onValueChanged.AddListener(PlayClickSound);
            speedTetris.onValueChanged.AddListener(OnSpeedTetrisSlider);
            speedTetris.onValueChanged.AddListener(PlayClickSound);
        }
        else
        {
            snakeSettings.SetActive(false);
            tetrisSettings.SetActive(false);
        }
        
        RefreshUI();

        SetVersionText();
    }

    private string GetVersionText()
     {
        return $"Version: {VersionInfo.Version}({VersionInfo.Build})";
     }

    private void SetVersionText()
    {
        string finalStr = "";
        string id = GameHelper.GetPlayerID();
        if (id != "")
        {
            finalStr = $"Id: {id}, ";
        }
        finalStr += GetVersionText();
        versionText.text = finalStr;
    }
    
    private void PlayClickSound(bool change)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClickSound();
        }
    }   
    
    private void PlayClickSound(float change)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClickSound();
        }
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
        MyJsonHelper.SaveSnakeSettings(GameHelper.SnakeSettings);
    }
    
    public void OnMoveThroughWallsToggle(bool change)
    {
        GameHelper.SnakeSettings.MoveThroughWalls = change;
        MyJsonHelper.SaveSnakeSettings(GameHelper.SnakeSettings);
    }
    
    public void OnAccelerationToggle(bool change)
    {
        GameHelper.SnakeSettings.Acceleration = change;
        MyJsonHelper.SaveSnakeSettings(GameHelper.SnakeSettings);
        ShowSnakeSpeedParameters(!change);
    }
    
    public void OnSpeedSlider(Single change)
    {
        int speedType = (int) change;
        GameHelper.SnakeSettings.Speed = GameHelper.GetSpeedByTypeSnake(speedType);
        textSlider.text = speedType.ToString();
        MyJsonHelper.SaveSnakeSettings(GameHelper.SnakeSettings);
    }

    public void OnRateClick()
    {
        Debug.Log("Rate click");
#if UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.combysoft.combygames");
#elif UNITY_IOS
        Application.OpenURL("https://apps.apple.com/app/id6744244657");
#else
        Debug.Log("Оценка доступна только на мобильных платформах");
#endif
    }
    
    public void OnPrivacyPolicyClick()
    {
        Debug.Log("Privacy Policy click");
        Application.OpenURL("https://docs.google.com/document/d/1V2GoxaUvbsuK4PkvKpNJnmrc1WDLZUpQ9zEb1yob6is/edit?usp=sharing");
    }
    
    public void OnSupportClick()
    {
        string email = "combysoft@gmail.com";
        string subject = Uri.EscapeDataString("Bug Report from CombyGames Game");
        
        // Собираем полезную информацию
        string body = Uri.EscapeDataString(
            $"Please describe the issue:\n\n" +
            $"---\n" +
            $"Device: {SystemInfo.deviceModel}\n" +
            $"OS: {SystemInfo.operatingSystem}\n" +
            $"Game {GetVersionText()}\n" +
            $"Time: {System.DateTime.Now}"
        );
    
        string mailto = $"mailto:{email}?subject={subject}&body={body}";
        
        Debug.Log(mailto);
    
        Application.OpenURL(mailto);
    }
    
    public void OnRemoveAdsClick()
    {
        GameHelper.SetHaveAds(!GameHelper.HaveAds);
    }

    public void OnRestoreInAppClick()
    {
        Debug.Log("RestoreInApp");
    }
    
    public void ShowSnakeSpeedParameters(bool isShow)
    {
        if (isShow)
        {
            Color c = speedText.color;
            c.a = 1f; // нет прозрачности
            speedText.color = c;
            speed.interactable = true;
            
            Color c1 = textSlider.color;
            c1.a = 1f; // нет прозрачности
            textSlider.color = c1;
            
            Color c2 = backgroundSnakeSlider.color;
            c2.a = 1f; // нет прозрачности
            backgroundSnakeSlider.color = c2;
            
            Color c3 = fillSnakeSlider.color;
            c3.a = 1f; // нет прозрачности
            fillSnakeSlider.color = c3;
        }
        else
        {
            Color c = speedText.color;
            c.a = 0.5f; // 30% прозрачности
            speedText.color = c;
            speed.interactable = false;
            
            Color c1 = textSlider.color;
            c1.a = 0.5f; // нет прозрачности
            textSlider.color = c1;
            
            Color c2 = backgroundSnakeSlider.color;
            c2.a = 0.5f; // нет прозрачности
            backgroundSnakeSlider.color = c2;
            
            Color c3 = fillSnakeSlider.color;
            c3.a = 0.5f; // нет прозрачности
            fillSnakeSlider.color = c3;
        }
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
        MyJsonHelper.SaveTetrisSettings(GameHelper.TetrisSettings);
        ShowTetrisSpeedParameters(!change);
    }
    
    public void OnSpeedTetrisSlider(Single change)
    {
        int speedType = (int) change;
        GameHelper.TetrisSettings.Speed = GameHelper.GetSpeedByTypeTetris(speedType);
        textTetrisSlider.text = speedType.ToString();
        MyJsonHelper.SaveTetrisSettings(GameHelper.TetrisSettings);
    }
    
    public void ShowTetrisSpeedParameters(bool isShow)
    {
        if (isShow)
        {
            Color c = speedTetrisText.color;
            c.a = 1f; // нет прозрачности
            speedTetrisText.color = c;
            speedTetris.interactable = true;
            
            Color c1 = textTetrisSlider.color;
            c1.a = 1f; // нет прозрачности
            textTetrisSlider.color = c1;
            
            Color c2 = backgroundTetrisSlider.color;
            c2.a = 1f; // нет прозрачности
            backgroundTetrisSlider.color = c2;
            
            Color c3 = fillTetrisSlider.color;
            c3.a = 1f; // нет прозрачности
            fillTetrisSlider.color = c3;
        }
        else
        {
            Color c = speedTetrisText.color;
            c.a = 0.5f; // 30% прозрачности
            speedTetrisText.color = c;
            speedTetris.interactable = false;
            
            Color c1 = textTetrisSlider.color;
            c1.a = 0.5f; // нет прозрачности
            textTetrisSlider.color = c1;
            
            Color c2 = backgroundTetrisSlider.color;
            c2.a = 0.5f; // нет прозрачности
            backgroundTetrisSlider.color = c2;
            
            Color c3 = fillTetrisSlider.color;
            c3.a = 0.5f; // нет прозрачности
            fillTetrisSlider.color = c3;
        }
    }
    
    public void RefreshUI()
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroupParent);
        scrollRect.verticalNormalizedPosition = 1f; // Обновление скролла
    }
}