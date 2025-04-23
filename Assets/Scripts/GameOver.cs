using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameOver : MonoBehaviour
{
    [SerializeField, CanBeNull] private GameObject maximumParam;
    [SerializeField] private List<GameObject> confetti = new List<GameObject>();
    
    [SerializeField] private CanvasGroup background;
    [SerializeField] private RectTransform headerPanel;
    [SerializeField] private CanvasGroup scorePanelGroup;
    [SerializeField] private RectTransform scorePanel;
    [SerializeField] private CanvasGroup ratingPanelGroup;
    [SerializeField] private RectTransform ratingPanel;   
    [SerializeField, CanBeNull] private CanvasGroup maximumPanelGroup;
    [SerializeField, CanBeNull] private RectTransform maximumPanel;
    [SerializeField] private CanvasGroup buttonsGroup; // "домой" и "заново"

    private bool _isMaximumEnable = false;
    private bool _gameOverAnimationCompleted = false;
    private GameObject _chosenConfetti;
    
    public bool IsGameOver { get; set; }

    private void Start()
    {
        SetData();
    }

    public void SetData()
    {
        if (maximumParam != null)
        {
            _isMaximumEnable = GameHelper.GameType == MiniGameType.G2048;
            maximumParam.SetActive(_isMaximumEnable);
        }
    }
    
    public void ShowGameOverPanel(bool isShow, bool isWin = false)
    {
        _gameOverAnimationCompleted = false;
        _chosenConfetti = null;
        if (isShow)
        {
            foreach (var conf in confetti)
            {
                conf.SetActive(false);
            }
            gameObject.SetActive(true);
            StartCoroutine(GameOverFallbackTimer(2f, isWin)); // Через 2 секунды проверим, дошло ли всё до конца
            PlayGameOverAnimation(isWin);
            IsGameOver = true;
        }
        else
        {
            foreach (var conf in confetti)
            {
                conf.SetActive(false);
            }
            gameObject.SetActive(false);
            IsGameOver = false;
        }
    }


    public void PlayGameOverAnimation(bool isWin)
    {
        try
        {
            Sequence sequence = DOTween.Sequence();

            // 1. Задний фон появляется
            background.alpha = 0;
            sequence.Append(background.DOFade(1, 0.3f));

            // 2. Панель заголовка спускается сверху с прыжком
            Vector2 finalPos = new Vector2(headerPanel.anchoredPosition.x, -156.7322f);
            float screenHeight = ((RectTransform) headerPanel.parent).rect.height;
            headerPanel.anchoredPosition = new Vector2(finalPos.x, screenHeight + 200);
            sequence.Append(headerPanel.DOAnchorPos(finalPos, 0.5f).SetEase(Ease.OutBack));

            // 3. Конфетти (только при победе)
            sequence.AppendCallback(() =>
            {
                if (isWin)
                {
                    int randomIndex = Random.Range(0, confetti.Count);
                    _chosenConfetti = confetti[randomIndex];
                    _chosenConfetti.SetActive(true);
                }
            });

            // 4. Панель с очками — fade in + пульс
            scorePanelGroup.alpha = 0;
            scorePanel.localScale = Vector3.one;
            sequence.Append(scorePanelGroup.DOFade(1, 0.3f));
            sequence.Join(scorePanel.DOPunchScale(Vector3.one * 0.2f, 0.4f, 1, 0.5f));

            // 5. Панель с рейтингом
            ratingPanelGroup.alpha = 0;
            ratingPanel.localScale = Vector3.one;
            sequence.Append(ratingPanelGroup.DOFade(1, 0.3f));
            sequence.Join(ratingPanel.DOPunchScale(Vector3.one * 0.2f, 0.4f, 1, 0.5f));

            // 6. Панель максимума, если включена
            if (_isMaximumEnable && maximumPanelGroup != null && maximumPanel != null)
            {
                maximumPanelGroup.alpha = 0;
                maximumPanel.localScale = Vector3.one;
                sequence.Append(maximumPanelGroup.DOFade(1, 0.3f));
                sequence.Join(maximumPanel.DOPunchScale(Vector3.one * 0.2f, 0.4f, 1, 0.5f));
            }

            // 7. Кнопки — плавно появляются
            buttonsGroup.alpha = 0;
            sequence.Append(buttonsGroup.DOFade(1, 0.3f));
            
            sequence.OnKill(() => {
                if (!_gameOverAnimationCompleted)
                {
                    Debug.Log("Твин был прерван. Выполняем аварийно.");
                    FastShowPanel(isWin);
                }
            });

            // ✅ Страховка: что-то в конце обязательно выполняется
            sequence.OnComplete(() =>
            {
                Debug.Log("Анимация GameOver завершилась безопасно");

                // Можно добавить флаг, что всё завершено:
                _gameOverAnimationCompleted = true;
            });
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Ошибка во время анимации GameOver: " + ex.Message);

            FastShowPanel(isWin);
        }
    }

    public void FastShowPanel(bool isWin)
    {
        // Резервный план: быстро показать всё вручную
        background.alpha = 1;
        headerPanel.anchoredPosition = new Vector2(headerPanel.anchoredPosition.x, -156.7322f);
        scorePanelGroup.alpha = 1;
        ratingPanelGroup.alpha = 1;
        buttonsGroup.alpha = 1;

        if (_isMaximumEnable && maximumPanelGroup != null)
            maximumPanelGroup.alpha = 1;

        // Активируем нужное после сбоя
        if (isWin)
        {
            if (_chosenConfetti == null)
            {
                int randomIndex = Random.Range(0, confetti.Count);
                _chosenConfetti = confetti[randomIndex];
                _chosenConfetti.SetActive(true);
            }
            else
            {
                _chosenConfetti.SetActive(true);
            }
        }
        
        _gameOverAnimationCompleted = true;
    }
    
    private IEnumerator GameOverFallbackTimer(float time, bool isWin)
    {
        yield return new WaitForSeconds(time);

        if (!_gameOverAnimationCompleted)
        {
            Debug.LogWarning("Анимация не завершилась. Выполняем аварийно.");
            FastShowPanel(isWin); // тут всё вручную — альфа = 1, позиции выставить и т.п.
        }
    }
}