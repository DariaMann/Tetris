using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.SimpleLocalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;

public class CheckersManager: MonoBehaviour
{
    [SerializeField] private ThemeChineseCheckers themeChinese;
    [SerializeField] private TextMeshProUGUI stepsText;
    [SerializeField] private HexMap hexMap;
    [SerializeField] private bool showHint;
    [SerializeField] private List<Player> players;

    [SerializeField] private Image hint;
    [SerializeField] private Sprite hintAvailable;
    [SerializeField] private Sprite hintUnavailable;

    [SerializeField] private Button undoButton;
    [SerializeField] private Sprite undoActiveSprite;
    [SerializeField] private Sprite undoDeactiveSprite;
    
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject warningPanel;
    
    [SerializeField] private GameObject finishPanel;
    [SerializeField] private RectTransform playersRatingPanel;
    [SerializeField] private GameObject playerInRatingPrefab;
    
    [SerializeField] private float warningSpeed = 0.5f;
    
    [SerializeField] private TextMeshProUGUI speedButtonText;
    
    private float _waitAIMoveTime = 0.5f;
    private int _speedMode = 0;

    private List<PlayerInRating> _playersInRating = new List<PlayerInRating>();
    private float _showingTime = 2;
    private bool _isShowing;
    private Color[] _playerColors = {
        Color.blue,   // Нижний игрок
        Color.red,    // Верхний игрок
        Color.green,  // Левый верхний
        Color.yellow, // Левый нижний
        Color.magenta,// Правый верхний
        Color.cyan    // Правый нижний
    };
    
    public ThemeChineseCheckers ThemeChinese
    {
        get => themeChinese;
        set => themeChinese = value;
    }
    
    public bool IsPlaying { get; set; } = false;
    
    public Stack<Step> EventSteps { get; set; } = new Stack<Step>();

    public Player CurrentPlayer { get; private set; }
    
    public bool IsPaused { get; private set; } = false;
    
    public int WinCount { get; private set; } = 0;
    
    public int Steps { get; private set; } = -1;
    public int CurrentPlayerIndex { get; private set; } = -1;
    public int FirstPlayerIndex { get; private set; } = -1;
    
    public Chip SelectedChip { get; private set; }

    public bool ShowHint
    {
        get => showHint;
        set => showHint = value;
    }
    
    public List<Player> Players
    {
        get => players;
        set => players = value;
    }

    void Start()
    {
        SetFirstSettings();
        LoadData();
        hexMap.GenerateBoard();
        SetupTargetZones();
        foreach (var player in Players)
        {
            player.Colored(_playerColors[player.ID]);
        }
        OnChangeSpeed();
        ChangeStepCount(0, true);
        
//        FirstStart();

        LoadLastPlay();
        LocalizationManager.LocalizationChanged += Localize;
    }
    
    private void Localize()
    {
        stepsText.text = LocalizationManager.Localize("Сheckers.steps") + ": " + Steps;
    }

    private void LoadLastPlay()
    {
        SaveDataChineseCheckers saveData = JsonHelper.LoadChineseCheckersData();
        if (saveData == null)
        {
            FirstStart();
            return;
        }

        SetPlayers(saveData.SavePlayers);
        IsPlaying = true;
        startPanel.SetActive(!IsPlaying);
        ChangeStepCount(saveData.Steps);
        FirstPlayerIndex = saveData.FirstPlayerIndex;
        
        hexMap.StartSave(saveData.SaveChips);
        SetCurrentPlayer(GetPlayerById(saveData.IdPlayingPlayer));
        StartNextTurn(true);
        
        

        CheckUndoButtonState();
    }
    
    private void SaveLastPlay()
    {
        if (!IsPlaying)
        {
            JsonHelper.SaveChineseCheckersData(null);
            return;
        }
        SaveDataChineseCheckers data = new SaveDataChineseCheckers(CurrentPlayer.ID, FirstPlayerIndex, Steps, Players, hexMap.Chips);
        JsonHelper.SaveChineseCheckersData(data);
    }

    private void SetPlayers(List<SavePlayer> savePlayers)
    {
        foreach (var player in Players)
        {
            SavePlayer savePlayer = savePlayers.Find(pl => pl.Id == player.ID);
            player.ChangeState(savePlayer.State);
            if (!player.IsActive)
            {
                player.gameObject.SetActive(false);
            }
        }
    }

    public void ResetGame()
    {
        StopAllCoroutines();
        Reset();
    }
    
    public void StartGame()
    {
        hexMap.StartFilling();
        SetCurrentPlayer(GetFirstPlayer());
        StartNextTurn(true);
    }
    
    void OnApplicationQuit()
    {
        SaveData();
        SaveLastPlay();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveData();
            SaveLastPlay();
        }
    }
    
    private void OnDestroy()
    {
        // Этот метод будет вызван при выходе из сцены
        SaveLastPlay();
        LocalizationManager.LocalizationChanged -= Localize;
    }
    
    public void LoadData()
    {
        foreach (var player in Players)
        {
            PlayerState state = (PlayerState) PlayerPrefs.GetInt("CCStatePlayer" + player.ID);
            player.ChangeState(state);
        }
        ShowHint = PlayerPrefs.GetInt("CCStateHint") == 1;
        SetHintState(ShowHint);
    }
    
    public void SaveData()
    {
        foreach (var player in Players)
        {
            SavePlayer(player);
        }

        SaveHintState();
    }
    
    private void SaveHintState()
    {
        PlayerPrefs.SetInt("CCStateHint", ShowHint ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SavePlayer(Player player)
    {
        PlayerPrefs.SetInt("CCStatePlayer" + player.ID, (int) player.State);
        PlayerPrefs.Save();
    }

    public void SetFirstSettings()
    {
        foreach (var player in Players)
        {
            if (!PlayerPrefs.HasKey("CCStatePlayer" + player.ID))
            {
                SavePlayer(player);
            }
        }
        if (!PlayerPrefs.HasKey("CCStateHint"))
        {
            SaveHintState();
        }
    }
    
    public Player GetFirstPlayer()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].IsActive)
            {
                FirstPlayerIndex = Players[i].ID;
                return Players[i];
            }
        }
        
        Debug.LogError("Ни одного играющего персонажа!");
        FirstPlayerIndex = -1;
        return null;
    }
    
    public Player GetPlayerById(int idPlayer)
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (Players[i].ID == idPlayer)
            {
                return Players[i];
            }
        }
        
        Debug.LogError("Нет персонажа с таким ID!");
        return null;
    }

    private void CheckUndoButtonState()
    {
        bool isActive = CurrentPlayer != null && CurrentPlayer.State == PlayerState.Player && EventSteps.Count > 0;

        undoButton.interactable = isActive;
        undoButton.image.sprite = isActive ? undoActiveSprite : undoDeactiveSprite;
    }

    public void AddStepEventObject(HexTile fromTile, Chip chip)
    {
        Step eventCode = new Step
        {
            FromTile = fromTile,
            Chip = chip
        };
        EventSteps.Push(eventCode);
    }

    public void OnUndoClick()
    {
        bool nextNotRobot = false;
        bool isFirstStep = true;
        do
        {
            Step step = EventSteps.Pop();
            step.Chip.UndoMove(step.FromTile);
            if (step.Chip.Player.State == PlayerState.Player || EventSteps.Count == 0)
            {
                nextNotRobot = true;
            }
            SetSelection(null);
            SetCurrentPlayer(step.Chip.Player);
            
            if (!isFirstStep && step.Chip.Player.ID == FirstPlayerIndex) // Значит был пройден круг ходов
            {
                ChangeStepCount(Steps - 1);
            }

            isFirstStep = false;
        } while (!nextNotRobot);

        CheckUndoButtonState();
        if (CurrentPlayer.State == PlayerState.Robot)
        {
            StartNextTurn(true);
        }
    }

    public void OnChangeHintStateClick()
    {
        SetHintState(!ShowHint);
        SaveHintState();
    }

    public void OnPlayClick()
    {
        int countPlayingPlayers = 0;
        foreach (var player in Players)
        {
            if (player.IsActive)
            {
                countPlayingPlayers += 1;
            }
        }

        if (countPlayingPlayers < 2)
        {
            ShowWarning();
            return;
        }
        IsPlaying = true;
        startPanel.SetActive(!IsPlaying);
        foreach (var player in Players)
        {
            if (!player.IsActive)
            {
                player.gameObject.SetActive(false);
            }
        }
        
        StartGame();
//        OnRepeatClick();
    }
    
    public void FirstStart()
    {
        IsPlaying = false;
        startPanel.SetActive(!IsPlaying);
        foreach (var player in Players)
        {
            player.gameObject.SetActive(true);
        }

        CheckUndoButtonState();
    }
    
    public void OnAgainClick()
    {
        IsPlaying = false;
        startPanel.SetActive(!IsPlaying);
        foreach (var player in Players)
        {
            player.gameObject.SetActive(true);
        }
        ResetGame();
    }

    public void PausedGame(bool isPause)
    {
        IsPaused = isPause;
    }

    private void SetHintState(bool state)
    {
        ShowHint = state;
        hint.sprite = ShowHint ? hintAvailable : hintUnavailable;

        if (ShowHint)
        {
            if (SelectedChip != null)
            {
                SelectedChip.ShowAvailableWay();
            }
        }
        else
        {
            if (SelectedChip != null)
            {
                SelectedChip.HideAvailableWay();
            }
        }
    }

    public void HideHint()
    {
        StopAllCoroutines();
        _isShowing = false;
        warningPanel.SetActive(false);
    }
    
    public void ShowWarning()
    {
        if (_isShowing)
        {
            HideHint();
        }
        
        _showingTime = 2;
        _isShowing = true;
        warningPanel.SetActive(true);
        ShowHintBottomPanel(warningPanel.GetComponent<RectTransform>(), true);
        StartCoroutine(DelayedAnimationHideCoroutine());
    }

    private IEnumerator DelayedAnimationHideCoroutine()
    {
        yield return new WaitForSecondsRealtime(_showingTime);
        yield return StartCoroutine(HideHintBottomPanelCorouite(warningPanel.GetComponent<RectTransform>(), true));
        HideHint();
    }
    
    public void ShowHintBottomPanel(RectTransform panel, bool isUp = false)
    {
        if (isUp)
        {
            panel.anchoredPosition = new Vector2(0f, 200f);
            panel.DOAnchorPosY(0, warningSpeed);
            return;
        }
        panel.anchoredPosition = new Vector2(0f, -200f);
        panel.DOAnchorPosY(0, warningSpeed);
    }
    
    public IEnumerator HideHintBottomPanelCorouite(RectTransform panel, bool isUp = false)
    {
        Tween myTween;
        if (isUp)
        {
            myTween = panel.DOAnchorPosY(200, warningSpeed);
        }
        else
        {
            myTween = panel.DOAnchorPosY(-200, warningSpeed);
        }
        yield return myTween.WaitForCompletion();
    }
    
    public void OnChangeSpeed()
    {
        switch (_speedMode)
        {
            case 1: _speedMode = 2; break;
            case 2: _speedMode = 3; break;
            case 3: _speedMode = 1; break;
            case 0: _speedMode = 1; break;
        }

        ChangeSpeed(_speedMode);
    }

    private void ChangeSpeed(int speedMode)
    {
        speedButtonText.text = "x" + _speedMode;
        
        switch (speedMode)
        {
            case 1: _waitAIMoveTime = 0.5f; break;
            case 2: _waitAIMoveTime = 0.3f; break;
            case 3: _waitAIMoveTime = 0.1f; break;
        }

        foreach (var chip in hexMap.Chips)
        {
            chip.ChangeSpeed(speedMode);
        }
    }

    public void Reset()
    {
        foreach (var player in Players)
        {
            player.Reset();
        }
        SetCurrentPlayer(null);
        WinCount = 0;
        CurrentPlayerIndex = -1;
        FirstPlayerIndex = -1;
        ChangeStepCount(-1);
        EventSteps.Clear();
        SetSelection(null);
        hexMap.ClearChips();
    }
    
    public void ChangeStepCount(int step, bool isSowFirst = false)
    {
        if (step < 0)
        {
            step = 0;
        }

        if (!isSowFirst)
        {
            Steps = step;
        }
        stepsText.text = LocalizationManager.Localize("Сheckers.steps") + ": " + step;
    }
    
    public bool CheckWin(Player player)
    {
        return player.Chips.All(chip => player.TargetZones.Contains(chip.Tile));
    }

    public void PlayerFinish(Player player)
    {
        WinCount += 1;
        player.Finish();
    }
    
    public void EndGame()
    {
        IsPlaying = false;
        foreach (var player in Players)
        {
            Debug.Log($"Игрок {player.ID} занял место {player.WinNumber} !");
        }
        
        SetSelection(null); 
        SetCurrentPlayer(null);
        
        // Остановить ход
        StopAllCoroutines();

        // Показать сообщение о победе (можно добавить UI)
        ShowFinishPanel();
    }

    private void ShowFinishPanel()
    {
        List<Player> sortedPlayers = new List<Player>(players);
        sortedPlayers.Sort((a, b) => a.WinNumber.CompareTo(b.WinNumber));

        foreach (var player in sortedPlayers)
        {
            if (player.IsActive)
            {
                GameObject playerGameObject = Instantiate(playerInRatingPrefab, playersRatingPanel);
                PlayerInRating playerInRating = playerGameObject.GetComponent<PlayerInRating>();
                playerGameObject.SetActive(true);
                playerInRating.SetData(player);
                _playersInRating.Add(playerInRating);
            }   
        }
        
        finishPanel.SetActive(true);
    }
    
    public void HideFinishPanel()
    {
        for (int i = _playersInRating.Count-1; i >= 0; i--)
        {
            Destroy(_playersInRating[i]);
        }
        _playersInRating.Clear();

        finishPanel.SetActive(false);
    }

    public void SetCurrentPlayer(Player newPlayer)
    {
        if (CurrentPlayer != null && CurrentPlayer != newPlayer)
        {
            CurrentPlayer.SetCurrentMoveAnchor(false); 
        }
        
        if (newPlayer == null)
        {
            CurrentPlayer = newPlayer;
            return;
        }

        CurrentPlayerIndex = newPlayer.ID;
        CurrentPlayer = newPlayer;
        CurrentPlayer.SetCurrentMoveAnchor(true);
        hexMap.SetPriority(CurrentPlayerIndex);
        CheckUndoButtonState();
    }  
    
    public void SetSelection(Chip newChip)
    {
        if (SelectedChip != null && SelectedChip != newChip)
        {
           SelectedChip.SetSelection(false); 
        }

        SelectedChip = newChip;
    }
    
    public List<HexTile> FindWays(Chip chip)
    {
        List<HexTile> availableTiles = new List<HexTile>();

        // 1. Получаем текущую клетку фишки
        HexTile startTile = chip.Tile;

        // 2. Получаем соседние клетки
        List<HexTile> neighbors = GetNeighbors(startTile);

        // 3. Проверяем простые шаги (соседняя клетка свободна)
        foreach (HexTile neighbor in neighbors)
        {
            if (!neighbor.IsOccupied)
            {
                availableTiles.Add(neighbor);
            }
        }

        // 4. Проверяем возможность прыжков
        HashSet<HexTile> visited = new HashSet<HexTile>();
        FindJumpWays(startTile, availableTiles, visited);

        return availableTiles;
    }

    
    private List<HexTile> GetNeighbors(HexTile tile)
    {
        List<HexTile> neighbors = new List<HexTile>();

        // Определяем соседей по координатам
        foreach (HexTile otherTile in hexMap.Tiles)
        {
            float distance = Vector2.Distance(tile.Position, otherTile.Position);
            if (Mathf.Abs(distance - hexMap.HexXOffset) < 0.01f || Mathf.Abs(distance - hexMap.HexYOffset) < 0.01f)
            {
                neighbors.Add(otherTile);
            }
        }

        return neighbors;
    }


    private void FindJumpWays(HexTile tile, List<HexTile> availableTiles, HashSet<HexTile> visited)
    {
        visited.Add(tile);

        foreach (HexTile neighbor in GetNeighbors(tile))
        {
            if (neighbor.IsOccupied)
            {
                // Найти клетку за соседней фишкой
                Vector2 jumpPosition = (neighbor.Position - tile.Position) * 2 + tile.Position;
                HexTile jumpTile = hexMap.Tiles.Find(t => Vector2.Distance(t.Position, jumpPosition) < 0.01f);

                if (jumpTile != null && !jumpTile.IsOccupied && !visited.Contains(jumpTile))
                {
                    availableTiles.Add(jumpTile);
                    FindJumpWays(jumpTile, availableTiles, visited);
                }
            }
        }
    }
    
    public List<HexTile> GetPathToTarget(HexTile startTile, HexTile targetTile)
    {
        if (startTile == targetTile)
            return new List<HexTile>();

        Queue<List<HexTile>> pathsQueue = new Queue<List<HexTile>>();
        HashSet<HexTile> visited = new HashSet<HexTile>();

        pathsQueue.Enqueue(new List<HexTile> { startTile });

        while (pathsQueue.Count > 0)
        {
            List<HexTile> currentPath = pathsQueue.Dequeue();
            HexTile currentTile = currentPath[currentPath.Count - 1]; // Берем последнюю клетку в текущем пути

            if (currentTile == targetTile)
                return currentPath; // Нашли путь до цели

            foreach (HexTile neighbor in GetNeighbors(currentTile))
            {
                if (!neighbor.IsOccupied && !visited.Contains(neighbor))
                {
                    List<HexTile> newPath = new List<HexTile>(currentPath) { neighbor };
                    pathsQueue.Enqueue(newPath);
                    visited.Add(neighbor);
                }
                else if (neighbor.IsOccupied) // Проверяем возможность прыжка
                {
                    HexTile jumpTile = GetJumpTile(currentTile, neighbor);
                    if (jumpTile != null && !jumpTile.IsOccupied && !visited.Contains(jumpTile))
                    {
                        List<HexTile> newPath = new List<HexTile>(currentPath) { jumpTile };
                        pathsQueue.Enqueue(newPath);
                        visited.Add(jumpTile);
                    }
                }
            }
        }

        return new List<HexTile>(); // Если пути нет, возвращаем пустой список
    }

    private HexTile GetJumpTile(HexTile startTile, HexTile occupiedNeighbor)
    {
        // Рассчитываем позицию клетки за соседней фишкой
        Vector2 jumpPosition = (occupiedNeighbor.Position - startTile.Position) * 2 + startTile.Position;

        // Ищем клетку, которая находится на рассчитанной позиции
        HexTile jumpTile = hexMap.Tiles.Find(t => Vector2.Distance(t.Position, jumpPosition) < 0.01f);

        return jumpTile;
    }

    public void SetupTargetZones()
    {
        foreach (HexTile tile in hexMap.Tiles)
        {
            int rowSize = hexMap.RowSizes[tile.Row];
            int targetPlayer = GetOppositeZone(tile.Row, tile.Col, rowSize);
            if (targetPlayer != -1)
            {
                Players[targetPlayer].TargetZones.Add(tile);
            }
        }
    }

    int GetOppositeZone(int row, int col, int rowSize)
    {
        // Нижний игрок (последние 4 ряда)
        if (row >= hexMap.RowSizes.Length - 4) return 3;
        // Верхний игрок (первые 4 ряда)
        if (row < 4) return 0;
        // Левый верхний игрок (левый сектор)
        if (row >= 4 && row < 8 && rowSize - 9 > col) return 5;
        // Левый нижний игрок (левый сектор)
        if (row >= 9 && row < 13 && rowSize - 9 > col) return 4;
        // Правый верхний игрок (правый сектор)
        if (row >= 4 && row < 8 && rowSize - col <= rowSize - 9) return 1;
        // Правый нижний игрок (правый сектор)
        if (row >= 9 && row < 13 && rowSize - col <= rowSize - 9) return 2;

        return -1; // Не является зоной игрока
    }
    
    
    public void StartNextTurn(bool isFirst = false)
    {
        int countPlayer = Players.Count;
        Player nextPlayer;

        do
        {
            if (!isFirst)
            {
                CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
            }
            nextPlayer = Players[CurrentPlayerIndex];

            if (CurrentPlayerIndex == FirstPlayerIndex && !isFirst) // Если начинается новый круг ходов
            {
                ChangeStepCount(Steps + 1);
            }

            countPlayer--;
            isFirst = false;

        } while ((!nextPlayer.IsActive || nextPlayer.IsFinish) && countPlayer > 0);

        if (!nextPlayer.IsActive || nextPlayer.IsFinish)
        {
            Debug.Log("Не осталось играющих игроков!");
            EndGame();
            return;
        }

        SetCurrentPlayer(nextPlayer);

        if (CurrentPlayer.State == PlayerState.Robot)
        {
            StartMoveRobot();
        }
    }

    public void StartMoveRobot()
    {
        StartCoroutine(AIMove(CurrentPlayer));
    }

    private IEnumerator AIMove(Player aiPlayer)
    {
        // Ждем, пока пауза не закончится
        while (IsPaused)
        {
            yield return null;
        }
        
        yield return new WaitForSeconds(_waitAIMoveTime); // Задержка перед ходом

        Chip bestChip = null;
        HexTile bestMove = null;
        int bestScore = int.MinValue;

        foreach (var chip in aiPlayer.Chips)
        {
            List<HexTile> possibleMoves = FindWays(chip);

            foreach (var move in possibleMoves)
            {
                int score = EvaluateMove(aiPlayer, chip, move);

                if (score > bestScore)
                {
                    Debug.Log("Score = "+ score + ", Row = " + chip.Tile.Row + ", Col = " + chip.Tile.Col 
                              + ", ToRow = " + move.Row + ", ToCol = " + move.Col);
                    bestScore = score;
                    bestChip = chip;
                    bestMove = move;
                }
            }
        }

        if (bestChip != null && bestMove != null)
        {
            yield return StartCoroutine(bestChip.MoveCoroutine(bestMove));
        }
        
        // Ждем, пока пауза не закончится
        while (IsPaused)
        {
            yield return null;
        }

        StartNextTurn(); // Передаем ход следующему
    }

    private int EvaluateMove(Player player, Chip chip, HexTile move)
    {
        int score = 0;

        bool isInTargetZone = player.TargetZones.Contains(chip.Tile);
        bool isMovingToTargetZone = player.TargetZones.Contains(move);

        // 1. Если фишка еще не в TargetZones, даем высокий приоритет ходу в нее
        if (!isInTargetZone && isMovingToTargetZone)
        {
            score += 300; // Максимальный приоритет входа
        }

        // 2. Если фишка уже в TargetZones, она должна двигаться внутрь, а не обратно
        if (isInTargetZone)
        {
            if (!isMovingToTargetZone)
            {
                Debug.Log("Выход из зоны. Текущая: "+ chip.Tile.Row + "," + chip.Tile.Col + ", Следующая: "+ move.Row + "," + move.Col + ", Очки: " + score);
                return -1000; // Запрещаем выходить
            }
            else
            {
                int priorityDiff = move.Priority - chip.Tile.Priority;
                if (priorityDiff > 0)
                {
                    score += priorityDiff * 50; // Бонус за продвижение к более ценным клеткам
                }
                else if (priorityDiff < 0)
                {
                    score += priorityDiff * 100; // Большой штраф за движение обратно
                }
            }
        }

        //Если фишка все еще в стартовой зоне, даем приоритет чтобы она вышла
        if (chip.Tile.Priority < 0 && chip.Tile.Priority < move.Priority)
        {
            int priorityDiff = Mathf.Abs(move.Priority) - Mathf.Abs(chip.Tile.Priority);
            score += Mathf.Abs(priorityDiff) * 50;
        }
        
        //если фишка не приближается то нафиг
        float distanceCurrentScore = GetDistanceToFinish(player, chip.Tile);
        float distanceNextScore = GetDistanceToFinish(player, move);
        if (Math.Abs(distanceCurrentScore - distanceNextScore) < 0.1f || distanceCurrentScore - distanceNextScore > 0.1f)
        {
            Debug.Log("Не приближается. Текущая: "+ chip.Tile.Row + "," + chip.Tile.Col + ", Следующая: "+ move.Row + "," + move.Col + ", Очки: " + score);
            return -1000;
        }

        // Приоритет движения к цели
        int distanceScore = GetDistanceScore(player, move);
        //Усиливаем важность движения к цели только если клетка уже не в цели
        int priority = isInTargetZone ? 1 : 2;
        score += distanceScore * priority; // Усиливаем важность движения к цели

        // Штраф за отклонение от цели
        float directionFactor = GetDirectionFactor(player, chip, move);
        int fine = Mathf.RoundToInt(directionFactor * 50);
        score += fine; // Если движение в сторону цели — бонус, если вбок — штраф
        
        Debug.Log("Текущая: "+ chip.Tile.Row + "," + chip.Tile.Col + ", Следующая: "+ move.Row + "," + move.Col + ", Очки: " + score);
        return score;
    }

    private float GetDirectionFactor(Player player, Chip chip, HexTile move)
    {
        Vector2 toTarget = (GetClosestTarget(player, move).Position - chip.Tile.Position).normalized;
        Vector2 moveDir = (move.Position - chip.Tile.Position).normalized;

        return Vector2.Dot(toTarget, moveDir); // 1.0 - строго в сторону цели, 0 - перпендикулярно, -1 - назад
    }

    private HexTile GetClosestTarget(Player player, HexTile move)
    {
        HexTile closestTarget = player.TargetZones
            .OrderBy(target => Vector2.Distance(move.Position, target.Position))
            .FirstOrDefault();

        return closestTarget;
    }

    private int GetDistanceScore(Player player, HexTile move)
    {
        HexTile closestTarget = player.TargetZones
            .Where(target => target.Chip == null) // Оставляем только свободные клетки
            .OrderBy(target => Vector2.Distance(move.Position, target.Position))
            .FirstOrDefault();

        if (closestTarget == null) return 0;

        float distance = Vector2.Distance(move.Position, closestTarget.Position);
        return Mathf.RoundToInt(100 - distance); // Чем ближе к цели, тем лучше
    }
    
    private float GetDistanceToFinish(Player player, HexTile move)
    {
        HexTile lastTarget = player.TargetZones // Оставляем только свободные клетки
            .FirstOrDefault(target => target.Priority == 4);

        if (lastTarget == null) return 0;

        if (lastTarget.Chip != null)
        {
            lastTarget = player.TargetZones
                .Where(target => target.Chip == null) // Оставляем только свободные клетки
                .OrderBy(target => Vector2.Distance(move.Position, target.Position))
                .FirstOrDefault();
        }
        
        if (lastTarget == null) return 0;

        float distance = Vector2.Distance(move.Position, lastTarget.Position);
        return 100 - distance; // Чем ближе к цели, тем лучше
    }

}