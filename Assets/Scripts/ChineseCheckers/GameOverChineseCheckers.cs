using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameOverChineseCheckers: MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> confetti = new List<ParticleSystem>();
    
    [SerializeField] private CanvasGroup background;
    [SerializeField] private RectTransform headerPanel;
    [SerializeField] private CanvasGroup buttonsGroup; // "домой" и "заново"

    private bool _gameOverAnimationCompleted = false;
    private Sequence _gameOverSequence;
    private bool _isWin = false;
    private ParticleSystem _chosenConfetti;
    private List<PlayerInRating> _playerPanels;
    
    public bool IsGameOver { get; set; }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            InterruptGameOverAnimation(_playerPanels);
        }
        else
        {
            ResumeParticles();
        }
    }

    private void OnApplicationQuit()
    {
        InterruptGameOverAnimation(_playerPanels);
    }
    
    private void ResumeParticles()
    {
        if (_chosenConfetti != null && _chosenConfetti.isPlaying == false && _chosenConfetti.loop)
        {
            _chosenConfetti.Play();
            Debug.Log("Confetti resumed");
        }
    }
    
    private void InterruptGameOverAnimation(List<PlayerInRating> playerPanels)
    {
        if (!_gameOverAnimationCompleted)
        {
            if (_gameOverSequence != null && _gameOverSequence.IsActive())
            {
                _gameOverSequence.Kill();
            }

            FastShowPanel(_isWin, playerPanels);
        }
    }

    public void ShowGameOverPanel(bool isShow, List<PlayerInRating> playerPanels, bool isWin = false)
    {
        _playerPanels = playerPanels;
        InterruptGameOverAnimation(playerPanels);
        
        _gameOverAnimationCompleted = false;
        _chosenConfetti = null;
        _isWin = isWin;
        if (isShow)
        {
            foreach (var conf in confetti)
            {
                conf.gameObject.SetActive(false);
            }
            gameObject.SetActive(true);
            IsGameOver = true;
            GameHelper.IsGameOver = IsGameOver;
            StartCoroutine(GameOverFallbackTimer(4f, isWin, playerPanels));
            PlayGameOverAnimation(isWin, playerPanels);
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
    
    private void PlayGameOverAnimation(bool isWin, List<PlayerInRating> playerPanels)
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
                    int randomIndex = Random.Range(0, confetti.Count);
                    _chosenConfetti = confetti[randomIndex];
                    _chosenConfetti.gameObject.SetActive(true);
                }
            });

            // 4. Панели с игроками — fade in + пульс
            foreach (var player in playerPanels)
            {
                player.CanvasGroup.alpha = 0;
                player.RectTransform.localScale = Vector3.one;
                _gameOverSequence.Append(player.CanvasGroup.DOFade(1, 0.3f));
                _gameOverSequence.Join(player.RectTransform.DOPunchScale(Vector3.one * 0.2f, 0.4f, 1, 0.5f));
            }

            // 5. Кнопки — плавно появляются
            buttonsGroup.interactable = false;
            buttonsGroup.alpha = 0;
            _gameOverSequence.Append(buttonsGroup.DOFade(1, 0.3f));
            
            _gameOverSequence.AppendCallback(() => { buttonsGroup.interactable = true; });
            
            _gameOverSequence.OnKill(() => {
                if (!_gameOverAnimationCompleted)
                {
                    Debug.Log("Твин был прерван. Выполняем аварийно.");
                    FastShowPanel(isWin, playerPanels);
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

            FastShowPanel(isWin, playerPanels);
        }
    }

    private void FastShowPanel(bool isWin, List<PlayerInRating> playerPanels)
    {
        // Резервный план: быстро показать всё вручную
        background.alpha = 1;
        headerPanel.anchoredPosition = new Vector2(headerPanel.anchoredPosition.x, -156.7322f);
        buttonsGroup.alpha = 1;
        buttonsGroup.interactable = true;

        foreach (var player in playerPanels)
        {
            player.CanvasGroup.alpha = 1;
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
    
    private IEnumerator GameOverFallbackTimer(float time, bool isWin, List<PlayerInRating> playerPanels)
    {
        yield return new WaitForSeconds(time);

        if (!_gameOverAnimationCompleted)
        {
            Debug.LogWarning("Анимация не завершилась. Выполняем аварийно.");
            FastShowPanel(isWin, playerPanels); // тут всё вручную — альфа = 1, позиции выставить и т.п.
        }
    }
}