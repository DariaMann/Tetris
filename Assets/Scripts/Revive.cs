using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Revive: MonoBehaviour
{
    [SerializeField] private GameObject backgroudPanel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject linesPanel;
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private GameObject adImage;
    [SerializeField] private GameObject adText;
    [SerializeField] private GameObject noAdText;
    
    [SerializeField] private TextMeshProUGUI countdownText; // Привяжи сюда свой TMP текст
    [SerializeField] private float scaleDuration = 0.4f;    // Время анимации увеличения/уменьшения
    [SerializeField] private float pauseBetween = 0.2f;     // Пауза между цифрами
    
    private Coroutine countdownCoroutine;
    public bool IsShowTimer { get; set; }
    
    void OnApplicationPause(bool pause)
    {
        if (!pause && IsShowTimer && GameHelper.IsShowRevive)
        {
            RestartTimer();
        }
    }

    private void RestartTimer()
    {
        StopTimer();
        StartTimer();
    }
    
    private void StartTimer()
    {
        countdownCoroutine = StartCoroutine(CountdownRoutine());
    }
    
    private void StopTimer()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownText.text = "";
            countdownText.transform.localScale = Vector3.zero;
            countdownCoroutine = null;
        }
    }
    
    private IEnumerator CountdownRoutine()
    {
        for (int i = 3; i >= 0; i--)
        {
            countdownText.text = i.ToString();
            countdownText.transform.localScale = Vector3.zero;
            
            if (AudioManager.Instance != null)
            {
                if (i == 0)
                {
                    AudioManager.Instance.PlayTimerFinishSound();
                }
                else
                {
                    AudioManager.Instance.PlayTimerSound();
                }
            }

            // Увеличить
            yield return countdownText.transform.DOScale(1.5f, scaleDuration).SetEase(Ease.OutBack).WaitForCompletion();

            // Уменьшить
            yield return countdownText.transform.DOScale(0f, scaleDuration).SetEase(Ease.InBack).WaitForCompletion();

            // Немного подождать перед следующей цифрой
            yield return new WaitForSeconds(pauseBetween);
        }

        countdownText.text = "";
        // Здесь можешь запустить игру, включить спавн или что нужно
        Debug.Log("Start!");
        ShowTimerRevivePanel(false);
    }
    
    public void ShowMainRevivePanel(bool isShow)
    {
        if (isShow)
        {
            backgroudPanel.SetActive(true);
            mainPanel.SetActive(true);
            GameHelper.IsShowRevive = true;
            AppodealManager.Instance.HideBottomBanner();
            adImage.SetActive(GameHelper.HaveAds);
            adText.SetActive(GameHelper.HaveAds);
            noAdText.SetActive(!GameHelper.HaveAds);
        }
        else
        {
            backgroudPanel.SetActive(false);
            mainPanel.SetActive(false);
            GameHelper.IsShowRevive = false;
            AppodealManager.Instance.ShowBottomBanner();
        }
    }
    
    public void ShowLinesRevivePanel(bool isShow)
    {
        if (isShow)
        {
            mainPanel.SetActive(false);
            linesPanel.SetActive(true);
        }
        else
        {
            backgroudPanel.SetActive(false);
            linesPanel.SetActive(false);
            GameHelper.IsShowRevive = false;
            AppodealManager.Instance.ShowBottomBanner();
        }
    }
    
    public void ShowTimerRevivePanel(bool isShow)
    {
        if (isShow)
        {
            mainPanel.SetActive(false);
            timerPanel.SetActive(true);
            IsShowTimer = true;
            StartTimer();
        }
        else
        {
            backgroudPanel.SetActive(false);
            timerPanel.SetActive(false);
            GameHelper.IsGameOver = false;
            GameHelper.IsShowRevive = false;
            IsShowTimer = false;
            AppodealManager.Instance.ShowBottomBanner();
        }
    }

    public void OnBallClick(int type)
    {
        ShowLinesRevivePanel(false);
        GameManagerLines98.Instance.DeleteBallsByColor(type);
    }
}