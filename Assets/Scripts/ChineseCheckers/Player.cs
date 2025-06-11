using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private CheckersManager checkersManager;
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private SpriteRenderer finishIcon;
    [SerializeField] private SpriteRenderer personBG;
    [SerializeField] private TextMeshPro winNumber;
    [SerializeField] private int iD = 0;
    [SerializeField] private PlayerState state;
    
    [SerializeField] private Sprite robotSprite;
    [SerializeField] private Sprite noneSprite;
    [SerializeField] private Sprite playerSprite;
    
    [SerializeField] private float scaleMultiplier = 1.2f; // Насколько увеличивается (1.2 = на 20%)
    [SerializeField] private float duration = 0.5f; // Длительность одного цикла
    
    private Tween _pulseTween; // Сохраняем ссылку на анимацию

    public PlayerState State
    {
        get => state;
        private set => state = value;
    }

    public Color Color { get; set; }
    public int WinNumber { get; private set; } = 0;
    public int WinSteps { get; private set; } = 0;
    
    public bool IsFinish { get; private set; }

    public bool IsActive { get; private set; } = true;
    
    public bool IsPlaying { get; set; }

    public List<Chip> Chips { get; private set; } = new List<Chip>();
    
    public List<HexTile> TargetZones { get; private set; } = new List<HexTile>();

    public int ID
    {
        get => iD;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerState newState;
        if (!checkersManager.IsPlaying)
        {
            // Получаем все значения Enum
            PlayerState[] states = (PlayerState[]) Enum.GetValues(typeof(PlayerState));

            // Находим индекс текущего состояния
            int currentIndex = Array.IndexOf(states, State);

            // Вычисляем следующий индекс (сброс к 0, если конец)
            int nextIndex = (currentIndex + 1) % states.Length;

            // Присваиваем новое состояние
            newState = states[nextIndex];
        }

        else
        {
            if (State == PlayerState.None)
            {
                return;
            }
            newState = State == PlayerState.Player ? PlayerState.Robot : PlayerState.Player;
        }

        ChangeState(newState);
        
        AudioManager.Instance.PlayClick4Sound();
    }
    
    public void ChangeState(PlayerState newState, bool firstVisit = false)
    {
        State = newState;
        IsActive = State != PlayerState.None;
        ChangeIcon();
        if (State == PlayerState.Robot && checkersManager.CurrentPlayer == this && !IsPlaying && !firstVisit)
        {
            checkersManager.StartMoveRobot();
        }
        checkersManager.SetSelection(null);
        checkersManager.SavePlayer(this);
    }

    private void ChangeIcon()
    {
        switch (State)
        {
            case PlayerState.Robot: icon.sprite = robotSprite; break;
            case PlayerState.None: icon.sprite = noneSprite; break;
            case PlayerState.Player: icon.sprite = playerSprite; break;
        }
    }

    public void Colored(Color color)
    {
        Color = color;
        icon.color = Color;
//        foreach (var target in TargetZones)
//        {
//            target.gameObject.GetComponent<SpriteRenderer>().color = Color;
//        }
    }

    public void StartPulse()
    {
        if (_pulseTween == null || !_pulseTween.IsActive()) 
        {
            _pulseTween = icon.transform.DOScale(icon.transform.localScale * scaleMultiplier, duration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void StopPulse()
    {
        _pulseTween?.Kill(); // Остановить анимацию
        icon.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f); // Вернуть стандартный размер (или сохранить оригинальный)
    }
    
    public void Finish()
    {
        finishIcon.gameObject.SetActive(true);
        IsFinish = true;
        WinNumber = checkersManager.WinCount;
        WinSteps = checkersManager.Steps;
        winNumber.text = WinNumber.ToString();
        GameHelper.VibrationStart();
        AudioManager.Instance.PlaySuccessLineSound();

        if (State == PlayerState.Player && WinSteps < GameHelper.SaveChineseCheckers.Record)
        {
            GameHelper.SaveChineseCheckers.Record = WinSteps;
            MyJsonHelper.SaveChineseCheckers(GameHelper.SaveChineseCheckers);
            GameServicesManager.ReportScore(WinSteps, MiniGameType.ChineseCheckers);
        }
    }
    
    private void ResetFinish()
    {
        finishIcon.gameObject.SetActive(false);
        IsFinish = false;
        WinNumber = 0;
        WinSteps = 0;
    }

    public void SetCurrentMoveAnchor(bool active)
    {
//        currentMove.SetActive(active);
        if (active)
        {
            StartPulse();
        }
        else
        {
            StopPulse();
        }
    }
    
    public void SetTheme(Color colorBg, Color colorFinishIcon, Color colorWinNumber)
    {
        personBG.color = colorBg;
        finishIcon.color = colorFinishIcon;
        winNumber.color = colorWinNumber;
    }

    public void Reset()
    {
        for (int i = Chips.Count-1; i >= 0; i--)
        {
            Destroy(Chips[i].gameObject);
        }
        Chips.Clear();
        
        ResetFinish();
        IsFinish = false;
        WinNumber = 0;
        WinSteps = 0;
    }

}