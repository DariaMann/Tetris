using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameOver : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> confetti = new List<ParticleSystem>();
    
    [SerializeField] private CanvasGroup background;
    [SerializeField] private RectTransform headerPanel;
    
    [SerializeField] private CanvasGroup scorePanelGroup;
    [SerializeField] private RectTransform scorePanel;
    [SerializeField] private CanvasGroup ratingPanelGroup;
    [SerializeField] private RectTransform ratingPanel;   
    [SerializeField] private CanvasGroup maximumPanelGroup;
    [SerializeField] private RectTransform maximumPanel;
    
    [SerializeField] private CanvasGroup buttonsGroup; // "домой" и "заново"

    private bool _isMaximumEnable = false;
    private bool _gameOverAnimationCompleted = false;
    private Sequence _gameOverSequence;
    private bool _isWin = false;
    private ParticleSystem _chosenConfetti;
    
    public bool IsGameOver { get; set; }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            InterruptGameOverAnimation();
        }
        else
        {
            ResumeParticles();
        }
    }

    private void OnApplicationQuit()
    {
        InterruptGameOverAnimation();
    }
    
    private void ResumeParticles()
    {
        if (_chosenConfetti != null && _chosenConfetti.isPlaying == false && _chosenConfetti.loop)
        {
            _chosenConfetti.Play();
            Debug.Log("Confetti resumed");
        }
    }
    
    public void InterruptGameOverAnimation()
    {
        if (!_gameOverAnimationCompleted)
        {
            if (_gameOverSequence != null && _gameOverSequence.IsActive())
            {
                _gameOverSequence.Kill();
            }
            
            FastShowPanel(_isWin);
        }
    }

    public void SentAnalytic(SaveScores saveScores)
    {
        AnalyticsManager.Instance.LogEvent(AnalyticType.game_over.ToString(), saveScores.CurrentScore);
    }
    
    public void ShowGameOverPanel(bool isShow, SaveScores saveScores = null, bool isWin = false)
    {
        if (isShow)
        {
            if (saveScores != null)
            {
                SentAnalytic(saveScores);
            }
            AppodealManager.Instance.HideBottomBanner();
        }
        else
        {
            AppodealManager.Instance.ShowBottomBanner();
        }
        
        InterruptGameOverAnimation();
        
        _gameOverAnimationCompleted = false;
        _chosenConfetti = null;
        _isWin = isWin;
        _isMaximumEnable = GameHelper.GameType == MiniGameType.G2048;
        maximumPanel.gameObject.SetActive(_isMaximumEnable);
        if (isShow)
        {
            foreach (var conf in confetti)
            {
                conf.gameObject.SetActive(false);
            }
            gameObject.SetActive(true);
            IsGameOver = true;
            GameHelper.IsGameOver = IsGameOver;
            StartCoroutine(GameOverFallbackTimer(2f, isWin)); // Через 2 секунды проверим, дошло ли всё до конца
            PlayGameOverAnimation(isWin);
        }
        else
        {
            foreach (var conf in confetti)
            {
                conf.gameObject.SetActive(false);
            }
            gameObject.SetActive(false);
            IsGameOver = false;
            GameHelper.IsGameOver = IsGameOver;
        }
    }

    private void PlayGameOverAnimation(bool isWin)
    {
        try
        {
            if (_gameOverSequence != null && _gameOverSequence.IsActive())
            {
                _gameOverSequence.Kill();
            }
            _gameOverSequence = DOTween.Sequence();

            // 1. Задний фон появляется
            background.alpha = 0;
            _gameOverSequence.Append(background.DOFade(1, 0.3f));

            // 2. Панель заголовка спускается сверху с прыжком
            Vector2 finalPos = new Vector2(headerPanel.anchoredPosition.x, -156.7322f);
            float screenHeight = ((RectTransform) headerPanel.parent).rect.height;
            headerPanel.anchoredPosition = new Vector2(finalPos.x, screenHeight + 200);
            _gameOverSequence.Append(headerPanel.DOAnchorPos(finalPos, 0.5f).SetEase(Ease.OutBack));

            // 3. Конфетти (только при победе)
            _gameOverSequence.AppendCallback(() =>
            {
                if (isWin)
                {
                    AudioManager.Instance.PlayWinsSound();
                    int randomIndex = Random.Range(0, confetti.Count);
                    _chosenConfetti = confetti[randomIndex];
                    _chosenConfetti.gameObject.SetActive(true);
                }
            });

            // 4. Панель с очками — fade in + пульс
            scorePanelGroup.alpha = 0;
            scorePanel.localScale = Vector3.one;
            _gameOverSequence.Append(scorePanelGroup.DOFade(1, 0.3f));
            _gameOverSequence.Join(scorePanel.DOPunchScale(Vector3.one * 0.2f, 0.4f, 1, 0.5f));

            // 5. Панель с рейтингом
            ratingPanelGroup.alpha = 0;
            ratingPanel.localScale = Vector3.one;
            _gameOverSequence.Append(ratingPanelGroup.DOFade(1, 0.3f));
            _gameOverSequence.Join(ratingPanel.DOPunchScale(Vector3.one * 0.2f, 0.4f, 1, 0.5f));

            // 6. Панель максимума, если включена
            if (_isMaximumEnable)
            {
                maximumPanelGroup.alpha = 0;
                maximumPanel.localScale = Vector3.one;
                _gameOverSequence.Append(maximumPanelGroup.DOFade(1, 0.3f));
                _gameOverSequence.Join(maximumPanel.DOPunchScale(Vector3.one * 0.2f, 0.4f, 1, 0.5f));
            }

            // 7. Кнопки — плавно появляются
            buttonsGroup.interactable = false;
            buttonsGroup.alpha = 0;
            _gameOverSequence.Append(buttonsGroup.DOFade(1, 0.3f));
            
            _gameOverSequence.AppendCallback(() => { buttonsGroup.interactable = true; });
            
            _gameOverSequence.OnKill(() => {
                if (!_gameOverAnimationCompleted)
                {
                    Debug.Log("Твин был прерван. Выполняем аварийно.");
                    FastShowPanel(isWin);
                }
            });

            // ✅ Страховка: что-то в конце обязательно выполняется
            _gameOverSequence.OnComplete(() =>
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

    private void FastShowPanel(bool isWin)
    {
        // Резервный план: быстро показать всё вручную
        background.alpha = 1;
        headerPanel.anchoredPosition = new Vector2(headerPanel.anchoredPosition.x, -156.7322f);
        scorePanelGroup.alpha = 1;
        ratingPanelGroup.alpha = 1;
        buttonsGroup.alpha = 1;
        buttonsGroup.interactable = true;
        
        scorePanel.localScale = Vector3.one;
        ratingPanel.localScale = Vector3.one;

        if (_isMaximumEnable)
        {
            maximumPanelGroup.alpha = 1;
            maximumPanel.localScale = Vector3.one;
        }

        // Активируем нужное после сбоя
        if (isWin)
        {
            if (_chosenConfetti == null)
            {
                int randomIndex = Random.Range(0, confetti.Count);
                _chosenConfetti = confetti[randomIndex];
                _chosenConfetti.gameObject.SetActive(true);
            }
            else
            {
                _chosenConfetti.gameObject.SetActive(true);
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