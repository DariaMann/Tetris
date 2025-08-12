using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchZone: MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject secretPanel;
    [SerializeField] private TMP_InputField inputField;

    private const string Password = "BABUSKA2025";
    private const string Password_2 = "TEST2025";
    
    private float holdTime = 20f;
    private float holdTimer;
    private bool isHolding = false;

    private void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdTime)
            {
                isHolding = false;
                ShowSecretPanel();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        holdTimer = 0f;
        isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
    }

    public void ShowSecretPanel()
    {
        secretPanel.SetActive(true);
        inputField.text = "";
        inputField.ActivateInputField();
    }
    
    public void HideSecretPanel()
    {
        secretPanel.SetActive(false);
    }
    
    public void OnSubmit()
    {
        if (inputField.text == Password)
        {
            Debug.Log("Правильный код! Выполняем действие.");
            HideSecretPanel();

            AnalyticsManager.Instance.LogEvent(AnalyticType.no_ads_secret_code.ToString());
            GameHelper.SetHaveAds(!GameHelper.HaveAds);
        }
        else if (inputField.text == Password_2)
        {
            Debug.Log("Правильный код 2! Выполняем действие.");
            HideSecretPanel();

            GameHelper.SetShowTest(!GameHelper.ShowTest);
        }
        else
        {
            Debug.Log("Неправильный код!");
            inputField.text = "";
        }
    }
}