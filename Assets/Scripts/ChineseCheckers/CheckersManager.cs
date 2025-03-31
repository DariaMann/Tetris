using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CheckersManager: MonoBehaviour
{
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

    private List<PlayerInRating> _playersInRating = new List<PlayerInRating>();
    private float _showingTime = 2;
    private bool _isShowing;
    private int _firstPlayerId = -1;
    private Color[] _playerColors = {
        Color.blue,   // Нижний игрок
        Color.red,    // Верхний игрок
        Color.green,  // Левый верхний
        Color.yellow, // Левый нижний
        Color.magenta,// Правый верхний
        Color.cyan    // Правый нижний
    };
    
    public bool IsPlaying { get; set; } = false;
    
    public Stack<Step> EventSteps { get; set; } = new Stack<Step>();

    public Player CurrentPlayer { get; private set; }
    
    public bool IsPaused { get; private set; } = false;
    
    public int WinCount { get; private set; } = 0;
    
    public int Steps { get; private set; } = -1;
    public int CurrentPlayerIndex { get; private set; } = -1;
    
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

        FirstStart();

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
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveData();
        }
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
                _firstPlayerId = Players[i].ID;
                return Players[i];
            }
        }
        
        Debug.LogError("Ни одного играющего персонажа!");
        _firstPlayerId = -1;
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
            
            if (!isFirstStep && step.Chip.Player.ID == _firstPlayerId) // Значит был пройден круг ходов
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

    public void Reset()
    {
        foreach (var player in Players)
        {
            player.Reset();
        }
        SetCurrentPlayer(null);
        WinCount = 0;
        CurrentPlayerIndex = -1;
        ChangeStepCount(-1);
        EventSteps.Clear();
        SetSelection(null);
        hexMap.ClearChips();
    }
    
    public void ChangeStepCount(int step)
    {
        if (step < 0)
        {
            step = 0;
        }
        Steps = step;
        stepsText.text = "Steps: " + Steps;
    }
    
    public bool CheckWin(Player player)
    {
        return player.Chips.All(chip => player.TargetZones.Contains(chip.Tile));
    }

    public void PlayerFinish(Player player)
    {
        WinCount =+ 1;
        player.Finish();
        
        if (WinCount == 5)
        {
            EndGame();
        }
    }
    
    public void EndGame()
    {
        foreach (var player in Players)
        {
            Debug.Log($"Игрок {player.ID} занял место {player.WinNumber} !");
        }
        
        // Остановить ход
        StopAllCoroutines();

        // Показать сообщение о победе (можно добавить UI)
        ShowFinishPanel();
    }

    private void ShowFinishPanel()
    {
        foreach (var player in players)
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
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
            }
            nextPlayer = Players[CurrentPlayerIndex];

            if (CurrentPlayerIndex == 0) // Если начинается новый круг ходов
            {
                ChangeStepCount(Steps + 1);
            }

            countPlayer--;

        } while ((!nextPlayer.IsActive || nextPlayer.IsFinish) && countPlayer > 0);

        if (!nextPlayer.IsActive || nextPlayer.IsFinish)
        {
            Debug.LogError("Не осталось играющих игроков!");
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

//    private IEnumerator AIMove(Player aiPlayer)
//    {
//        yield return new WaitForSeconds(1f); // Задержка перед ходом
//
//        Chip bestChip = null;
//        HexTile bestMove = null;
//        int bestScore = int.MinValue;
//
//        foreach (var chip in aiPlayer.Chips)
//        {
//            List<HexTile> possibleMoves = FindWays(chip);
//
//            foreach (var move in possibleMoves)
//            {
//                int score = GetMoveScore(aiPlayer, move);
//                if (score > bestScore)
//                {
//                    bestScore = score;
//                    bestChip = chip;
//                    bestMove = move;
//                }
//            }
//        }
//
//        if (bestChip != null && bestMove != null)
//        {
//            yield return StartCoroutine(bestChip.MoveCoroutine(bestMove));
//        }
//
////        yield return new WaitForSeconds(0.5f);
//        StartNextTurn(); // Передаем ход следующему
//    }
//    

//    private int GetMoveScore(Player player, HexTile move)
//    {
//        // Чем ближе к целевой зоне — тем лучше
//        HexTile closestTarget = null;
//        float minDistance = float.MaxValue;
//
//        foreach (var target in player.TargetZones)
//        {
//            float distance = Vector2.Distance(move.Position, target.Position);
//            if (distance < minDistance)
//            {
//                minDistance = distance;
//                closestTarget = target;
//            }
//        }
//
//        return Mathf.RoundToInt(100 - minDistance); // Чем ближе, тем выше балл
//    }

    private IEnumerator AIMove(Player aiPlayer)
    {
        // Ждем, пока пауза не закончится
        while (IsPaused)
        {
            yield return null;
        }
        
        yield return new WaitForSeconds(0.5f); // Задержка перед ходом

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
                return -1000; // Запрещаем выходить
            }
            else
            {
                score += 10; // Легкий бонус за перемещение внутри зоны
            }
        }
        
        // Приоритет движения к цели
        int distanceScore = GetDistanceScore(player, move);
        score += distanceScore * 3; // Усиливаем важность движения к цели

//        // Прыжки все еще важны, но не критичны
//        if (IsJump(chip.Tile, move))
//        {
//            score += 30;
//            score += CountJumpChain(chip, move) * 20;
//        }

        // Штраф за отклонение от цели
        float directionFactor = GetDirectionFactor(player, chip, move);
        score += Mathf.RoundToInt(directionFactor * 50); // Если движение в сторону цели — бонус, если вбок — штраф

        return score;
    }

    private float GetDirectionFactor(Player player, Chip chip, HexTile move)
    {
        Vector2 toTarget = (GetClosestTarget(player, move).Position - chip.Tile.Position).normalized;
        Vector2 moveDir = (move.Position - chip.Tile.Position).normalized;

        return Vector2.Dot(toTarget, moveDir); // 1.0 - строго в сторону цели, 0 - перпендикулярно, -1 - назад
    }

    private bool IsMovingBackwards(Player player, Chip chip, HexTile move)
    {
        Vector2 directionToTarget = (GetClosestTarget(player, move).Position - chip.Tile.Position).normalized;
        Vector2 moveDirection = (move.Position - chip.Tile.Position).normalized;

        return Vector2.Dot(directionToTarget, moveDirection) < 0.3f; // Если угол менее 30 градусов к цели, считаем назад
    }
    
    private HexTile GetClosestTarget(Player player, HexTile move)
    {
        return player.TargetZones
            .OrderBy(target => Vector2.Distance(move.Position, target.Position))
            .FirstOrDefault();
    }

    private int GetDistanceScore(Player player, HexTile move)
    {
        HexTile closestTarget = player.TargetZones
            .OrderBy(target => Vector2.Distance(move.Position, target.Position))
            .FirstOrDefault();

        if (closestTarget == null) return 0;

        float distance = Vector2.Distance(move.Position, closestTarget.Position);
        return Mathf.RoundToInt(100 - distance); // Чем ближе к цели, тем лучше
    }
    
    private bool IsJump(HexTile from, HexTile to)
    {
        return Vector2.Distance(from.Position, to.Position) > hexMap.HexXOffset;
    }

    private int CountJumpChain(Chip chip, HexTile move)
    {
        int chainCount = 0;
        HashSet<HexTile> visited = new HashSet<HexTile>();
        HexTile currentTile = move;

        while (true)
        {
            List<HexTile> nextJumps = GetJumpMoves(currentTile);
            nextJumps.RemoveAll(t => visited.Contains(t));

            if (nextJumps.Count == 0) break;

            currentTile = nextJumps[0]; // Берем любой доступный прыжок
            visited.Add(currentTile);
            chainCount++;
        }

        return chainCount;
    }

    private List<HexTile> GetJumpMoves(HexTile tile)
    {
        List<HexTile> jumps = new List<HexTile>();

        foreach (HexTile neighbor in GetNeighbors(tile))
        {
            if (neighbor.IsOccupied)
            {
                Vector2 jumpPosition = (neighbor.Position - tile.Position) * 2 + tile.Position;
                HexTile jumpTile = hexMap.Tiles.Find(t => Vector2.Distance(t.Position, jumpPosition) < 0.01f);

                if (jumpTile != null && !jumpTile.IsOccupied)
                {
                    jumps.Add(jumpTile);
                }
            }
        }

        return jumps;
    }

}