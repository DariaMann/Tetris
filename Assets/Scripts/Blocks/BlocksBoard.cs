using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BlocksBoard : MonoBehaviour
{
    [SerializeField] private bool isEducation;
    [SerializeField] private EducationBlocks education;

    [SerializeField] private GameObject scorePlusPrefab;
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private ThemeBlocks themeBlocks;
    [SerializeField] private SquareUIGrid squareUiGrid;
    [SerializeField] private SaveScores saveScores;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private Button undoButton;
    
    [SerializeField] private int gridSize = 9;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private List<BlockShape> blockTypes;
    [SerializeField] private List<Block> blocks;
    
    public bool IsEducation
    {
        get => isEducation;
        set => isEducation = value;
    }
    
    public EducationBlocks Education
    {
        get => education;
        set => education = value;
    }
    
    public BlockTile EnableTile { get; set; }
    
    public List<Block> Blocks
    {
        get => blocks;
        set => blocks = value;
    }

    public ThemeBlocks ThemeBlocks
    {
        get => themeBlocks;
        set => themeBlocks = value;
    }
    
    public List<BlockTile> Tiles { get; set; } = new List<BlockTile>();
    
    public Stack<SaveDataBlocks> EventSteps { get; set; } = new Stack<SaveDataBlocks>();
    
    public bool IsPaused { get; private set; } = false;
    
    public SquareUIGrid SquareUiGrid
    {
        get => squareUiGrid;
        set => squareUiGrid = value;
    }

    private void Start()
    {
        if (isEducation)
        {
            return;
        }
        LoadLastPlay();
        CheckUndoButtonState();
        
        if (!GameHelper.GetEducationState(MiniGameType.Blocks))
        {
            education.ShowEducation(true);
            GameHelper.SetEducationState(MiniGameType.Blocks, true);
        }
    }

    private void OnEnable()
    {
        BlocksEvents.CheckIfBlockCanBePlaced += CheckIfBlockCanBePlaced;
    }

    private void OnDisable()
    {
        BlocksEvents.CheckIfBlockCanBePlaced -= CheckIfBlockCanBePlaced;
    }

    void OnApplicationQuit()
    {
        if (isEducation)
        {
            return;
        }
        SaveLastPlay();
    }

    void OnApplicationPause(bool pause)
    {
        if (isEducation)
        {
            return;
        }
        if (pause)
        {
            SaveLastPlay();
        }
    }
    
    private void OnDestroy()
    {
        if (isEducation)
        {
            return;
        }
        SaveLastPlay();
    }
    
    public void OnChangeBlocks()
    {
        CreateBlocks();
    }
    
    private void LoadLastPlay()
    {
        SaveDataBlocks saveData = GameHelper.SaveBlocks.SaveDataBlocks;
        if (saveData == null)
        {
            NewGame();
            return;
        }

        saveScores.ChangeScore(saveData.Score, false);
        saveScores.IsWin = saveData.IsWin;
        GenerateGrid();
        FullLoadGrid(saveData.SaveBlocksTile);
        CreateBlocks(saveData.Blocks);
        CheckInteractableBlocks();
        CheckUndoButtonState();
    }

    public void LoadStartEducation()
    {
        GenerateGrid();
    }
    
    public void LoadEducation(SaveDataBlocks saveData)
    {
        FullLoadGrid(saveData.SaveBlocksTile);
        CreateBlocks(saveData.Blocks);
        
        themeBlocks.SetTheme(GameHelper.Theme);
    }

    private void SaveLastPlay()
    {
        if (gameOver.IsGameOver)
        {
            GameHelper.SaveBlocks.SaveDataBlocks = null;
            JsonHelper.SaveBlocks(GameHelper.SaveBlocks);
            return;
        }
        SaveDataBlocks data = new SaveDataBlocks(saveScores.IsWin, saveScores.CurrentScore, Tiles, blocks);

        GameHelper.SaveBlocks.SaveDataBlocks = data;
        JsonHelper.SaveBlocks(GameHelper.SaveBlocks);
    }
    
    private void NewGame()
    {
        saveScores.ChangeScore(0, false);
        GenerateGrid();
        CreateBlocks();
    }
    
    public void Again()
    {
        gameOver.ShowGameOverPanel(false);
        
        saveScores.ChangeScore(0, false);
        ResetAll();
        CreateBlocks();
        CheckInteractableBlocks();
        CheckUndoButtonState();
    }
    
    public void PausedGame(bool isPause)
    {
        IsPaused = isPause;
    }
    
    public void GameOver()
    {
        gameOver.ShowGameOverPanel(true, saveScores.IsWin);
    }

    public void FullLoadGrid(List<SaveBlocksTile> saveBlocksTile)
    {
        for (int i = 0; i < saveBlocksTile.Count; i++)
        {
            if (saveBlocksTile[i].IsFull)
            {
                Tiles[i].Activate();
            }
        }
    }
    
    public void ResizeBlocks()
    {
        foreach (var block in blocks)
        {
            block.Resize(squareUiGrid.CellSize, true);
        }
    }   
    
    public void RescaleBlocks(float newScale)
    {
        foreach (var block in blocks)
        {
            block.Rescale(newScale);
        }
    }
    
    public void ResetAll()
    {
        ResetGrid();
        EventSteps.Clear();
    }  
       
    public void ResetAllAll()
    {
        foreach (var tile in Tiles)
        {
            Destroy(tile.gameObject);
        }
        Tiles.Clear();
    }  
    
    public void ResetGrid()
    {
        foreach (var tile in Tiles)
        {
            tile.Deactivate();
        }
    }

    public void CheckUndoButtonState()
    {
        if (EventSteps.Count > 0)
        {
            undoButton.interactable = true;
        }
        else
        {
            undoButton.interactable = false;
        }
    }
    
    public void OnUndo()
    {
        if (EventSteps.Count > 0)
        {
            SaveDataBlocks saveData = EventSteps.Pop();

            saveScores.ChangeScore(saveData.Score, false);
            ResetGrid();
            FullLoadGrid(saveData.SaveBlocksTile);
            CreateBlocks(saveData.Blocks);
            CheckInteractableBlocks();
            CheckUndoButtonState();
        }
    }
    
    public void AddStepEventObject()
    {
        if (isEducation)
        {
            return;
        }
        SaveDataBlocks data = new SaveDataBlocks(saveScores.IsWin, saveScores.CurrentScore, Tiles, blocks);
        EventSteps.Push(data);
        CheckUndoButtonState();
    }
    
    public void CreateBlocks()
    {
//        int countBlocks = 3;
//        while (countBlocks > 0)
//        {
//            int randomIndex = Random.Range(0, blockTypes.Count);
//            GameObject blockGameObject = Instantiate(blockPrefab, blockParent);
//            Block block = blockGameObject.GetComponent<Block>();
//            block.CreateBlock(blockTypes[randomIndex]);
//            Blocks.Add(block);
//            countBlocks--;
//        }

        foreach (var block in blocks)
        {
            int randomIndex = Random.Range(0, blockTypes.Count);
            block.CreateBlock(blockTypes[randomIndex]);
        }
    }   
    
    public void CreateBlocks(List<SaveBlock> blockTypes)
    {
        for (int i = 0; i < blockTypes.Count; i++)
        {
            if (blockTypes[i].BlockShape != null)
            {
                blocks[i].CreateBlock(blockTypes[i].BlockShape);
            }

            if (!blockTypes[i].IsEnable)
            {
                blocks[i].Deactivate();
            }
        }
    }
    
    public bool IsAllBlocksDeactivated()
    {
        foreach (var block in blocks)
        {
            if (block.IsActive)
                return false;
        }

        return true;
    }

    public Block GetSelectedBlock()
    {
        foreach (var block in blocks)
        {
            if (block.IsSelected)
                return block;
        }

        return null;
    }

    private void GenerateGrid()
    {
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                GameObject cellGo = Instantiate(cellPrefab, gridParent);
                BlockTile cell = cellGo.GetComponent<BlockTile>();

                cell.SetData(new Vector2Int(x, y), this);

                // Определяем блок 3x3 и цвет
                int blockX = x / 3;
                int blockY = y / 3;
                int blockIndex = blockY * 3 + blockX;

                // Назначаем цвет (пример: чередование)
                bool isDark = (blockX + blockY) % 2 == 0;

                // Применяем цвет
                cell.SetColor(isDark);

                Tiles.Add(cell);
            }
        }

//        theme.SetTheme(GameHelper.Theme);
    }

    private void CheckIfBlockCanBePlaced()
    {
        var selectedTiles = new List<BlockTile>();
        foreach (var tile in Tiles)
        {
            if (tile.IsSelected && !tile.IsOccupied)
            {
                selectedTiles.Add(tile);
                tile.IsSelected = false;
//                tile.Activate();
            }
        }

        var selectedBlock = GetSelectedBlock();
        if (selectedBlock == null)
        {
            return;
        }

        if (selectedBlock.TotalSquareNumber != selectedTiles.Count)
        {
            if (isEducation)
            {
                education.StartPlay();
            }
            BlocksEvents.MoveBlockToStartPosition();
            return;
        }

        AddStepEventObject();

        foreach (var tiles in selectedTiles)
        {
            tiles.Activate();
        }
        
        GetSelectedBlock().Deactivate();
        
        var (tilesToClear, score) = CheckLinesAndGetScore();
        
        if (tilesToClear.Count == 0)
        {
            AudioManager.Instance.PlayClickChipSound();
            CheckInteractableBlocks(); // если нечего очищать — сразу вызвать
        }
        else
        {
            List<Tween> tweens = new List<Tween>();

            foreach (var tile in tilesToClear)
            {
                var tween = tile.Explode();
                tweens.Add(tween);
            }

            // Когда завершится последняя анимация — вызываем метод
            DOTween.Sequence()
                .AppendInterval(tweens.Max(t => t.Duration()))
                .OnComplete(() => {
                    CheckInteractableBlocks();
                    AudioManager.Instance.PlaySuccessLineSound();
                    
//                    Vector3 center = GetCenterAnchoredPosition(tilesToClear);
//                    ShowScorePlusAnimtion(center, score);
                    if (isEducation)
                    {
                        education.ChangeStep();
                        return;
                    }
                    ShowScorePlusAnimationFromUI(tilesToClear, score);
                });
        }

        if (isEducation)
        {
            return;
        }
        
        if (score > 0)
        {
            saveScores.ChangeScore(score);
        }

        if (IsAllBlocksDeactivated())
        {
            CreateBlocks();
            CheckInteractableBlocks();
        }

    }
    
    private Vector3 GetCenterWorldPosition(List<BlockTile> blocks)
    {
        Vector3 sum = Vector3.zero;
        foreach (var b in blocks)
            sum += b.GetComponent<RectTransform>().position; // глобальная позиция
        return sum / blocks.Count;
    }

    public void ShowScorePlusAnimationFromUI(List<BlockTile> deletedBlocks, int score)
    {
        if (deletedBlocks == null || deletedBlocks.Count == 0) return;

        // Получаем среднюю мировую позицию
        Vector3 worldCenter = GetCenterWorldPosition(deletedBlocks);

        // Переводим в локальную позицию канваса
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mainCanvas.transform as RectTransform,
            Camera.main.WorldToScreenPoint(worldCenter),
            Camera.main,
            out Vector2 anchoredPos
        );

        GameObject go = Instantiate(scorePlusPrefab, mainCanvas.transform);
        go.transform.SetSiblingIndex(1);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = anchoredPos;

        Debug.Log($"[ShowScore] anchoredPosition: {anchoredPos}");

        go.GetComponent<ScorePlusAnimation>().Play(score);
    }

    public BlockTile GetTile(int x, int y)
    {
        if (x < 0 || x >= gridSize || y < 0 || y >= gridSize)
            return null;

        return Tiles[y * gridSize + x];
    }

    private (List<BlockTile> tilesToClear, int score) CheckLinesAndGetScore()
    {
        List<BlockTile> tilesToClear = new List<BlockTile>();
        HashSet<BlockTile> uniqueTiles = new HashSet<BlockTile>();
        int score = 0;
        int comboCount = 0;

        // Проверка по горизонтали
        for (int y = 0; y < gridSize; y++)
        {
            bool fullRow = true;
            for (int x = 0; x < gridSize; x++)
            {
                if (!GetTile(x, y).IsOccupied)
                {
                    fullRow = false;
                    break;
                }
            }

            if (fullRow)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    uniqueTiles.Add(GetTile(x, y));
                }

                score += 10;
                comboCount++;
            }
        }

        // Проверка по вертикали
        for (int x = 0; x < gridSize; x++)
        {
            bool fullColumn = true;
            for (int y = 0; y < gridSize; y++)
            {
                if (!GetTile(x, y).IsOccupied)
                {
                    fullColumn = false;
                    break;
                }
            }

            if (fullColumn)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    uniqueTiles.Add(GetTile(x, y));
                }

                score += 10;
                comboCount++;
            }
        }

        // Проверка 3x3 блоков
        for (int blockY = 0; blockY < 3; blockY++)
        {
            for (int blockX = 0; blockX < 3; blockX++)
            {
                bool fullBlock = true;
                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        int gx = blockX * 3 + x;
                        int gy = blockY * 3 + y;

                        if (!GetTile(gx, gy).IsOccupied)
                        {
                            fullBlock = false;
                            break;
                        }
                    }

                    if (!fullBlock) break;
                }

                if (fullBlock)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            int gx = blockX * 3 + x;
                            int gy = blockY * 3 + y;

                            uniqueTiles.Add(GetTile(gx, gy));
                        }
                    }

                    score += 20;
                    comboCount++;
                }
            }
        }

        // ✅ Добавляем 20% за каждую дополнительную комбинацию
        if (comboCount > 1)
        {
            float bonusMultiplier = 1 + 0.2f * (comboCount - 1); // например, при 3 комбинациях: 1 + 0.2 * 2 = 1.4
            score = Mathf.RoundToInt(score * bonusMultiplier);
        }

        tilesToClear = new List<BlockTile>(uniqueTiles);
        return (tilesToClear, score);
    }

    public List<BlockTile> GetAllSelectedTiles()
    {
        List<BlockTile> selectedTiles = new List<BlockTile>();
        foreach (var tile in Tiles)
        {
            if (tile.IsSelected && !tile.IsOccupied)
            {
                selectedTiles.Add(tile);
            }
        }

        return selectedTiles;
    }

    public void CheckPotentialForDelete()
    {
        DeactivateAddFutureDelete();
        
        List<BlockTile> previewTiles = GetAllSelectedTiles(); // метод, который ты используешь для получения текущих клеток под фигурой
        List<BlockTile> toClear = GetPotentialClearedTiles(previewTiles);

        foreach (var tile in toClear)
        {
            tile.ActivateFutureDelete(true); // или выделить, показать эффект и т.п.
        }
    }
    
    public void DeactivateAddFutureDelete()
    {
        foreach (var tile in Tiles)
        {
            tile.ActivateFutureDelete(false);
        }
    }
    
    public List<BlockTile> GetPotentialClearedTiles(List<BlockTile> previewTiles)
    {
        var clearedTiles = new HashSet<BlockTile>();

        // Проверка: фигура действительно помещается
        if (previewTiles == null || previewTiles.Count == 0) return clearedTiles.ToList();

        // Временное размещение: считаем, что эти клетки будут заняты
        foreach (var tile in previewTiles)
        {
            tile.IsSelected = true;
        }

        // Проверка линий
        for (int i = 0; i < gridSize; i++)
        {
            var row = Tiles.Where(t => t.GridPosition.y == i).ToList();
            if (row.All(t => t.IsOccupied || t.IsSelected))
                foreach (var t in row) clearedTiles.Add(t);

            var column = Tiles.Where(t => t.GridPosition.x == i).ToList();
            if (column.All(t => t.IsOccupied || t.IsSelected))
                foreach (var t in column) clearedTiles.Add(t);
        }

        // Проверка 3x3 квадратов
        for (int by = 0; by < gridSize; by += 3)
        {
            for (int bx = 0; bx < gridSize; bx += 3)
            {
                var square = Tiles.Where(t =>
                    t.GridPosition.x >= bx && t.GridPosition.x < bx + 3 &&
                    t.GridPosition.y >= by && t.GridPosition.y < by + 3).ToList();

                if (square.All(t => t.IsOccupied || t.IsSelected))
                    foreach (var t in square) clearedTiles.Add(t);
            }
        }

        // Сброс временного выделения
        foreach (var tile in previewTiles)
        {
            tile.IsSelected = false;
        }

        return clearedTiles.ToList();
    }
    
    public List<BlockTile> GetValidHoveredTiles(Block draggedBlock)
    {
        List<BlockTile> result = new List<BlockTile>();

        if (draggedBlock.BlockShape == null)
            return result;

        var shape = draggedBlock.BlockShape;

        // Найти ближайший Tile к первой клетке фигуры (pivot)
        var pivotBlock = draggedBlock.Squares[0];
        Vector3 pivotWorldPos = pivotBlock.transform.position;

        var centerTile = Tiles
            .OrderBy(t => Vector2.Distance(t.transform.position, pivotWorldPos))
            .FirstOrDefault(t => !t.IsOccupied);

        if (centerTile == null)
            return result;

        Vector2Int centerGridPos = centerTile.GridPosition;

        // Найти смещение в Grid координатах между pivot блоком и его Grid позицией в Shape
        Vector2Int pivotLocalIndex = GetFirstTrueCell(shape); // например, (0,0) — зависит от формы
        if (pivotLocalIndex == new Vector2Int(-1, -1)) return result;

        int shapeRowCount = shape.rows;
        int shapeColCount = shape.columns;

        int pivotRow = pivotLocalIndex.y;
        int pivotCol = pivotLocalIndex.x;

        for (int row = 0; row < shapeRowCount; row++)
        {
            for (int col = 0; col < shapeColCount; col++)
            {
                if (!shape.board[row].column[col])
                    continue;

                // 🧠 БЕЗ инверсии
                Vector2Int offset = new Vector2Int(col - pivotCol, row - pivotRow);
                Vector2Int targetGridPos = centerGridPos + offset;

                var tile = Tiles.FirstOrDefault(t => t.GridPosition == targetGridPos);
                if (tile == null || tile.IsOccupied || result.Contains(tile))
                    return new List<BlockTile>();

                result.Add(tile);
            }
        }

        return result;
    }

// Вспомогательный метод: находит первую включённую ячейку в фигуре
    private Vector2Int GetFirstTrueCell(BlockShape shape)
    {
        for (int row = 0; row < shape.rows; row++)
        {
            for (int col = 0; col < shape.columns; col++)
            {
                if (shape.board[row].column[col])
                    return new Vector2Int(col, row);
            }
        }
        return new Vector2Int(-1, -1);
    }

    public void CheckInteractableBlocks()
    {
        int countNotInteractable = 0;
        int countActive = 0;
        
        foreach (var block in blocks)
        {
            if (!block.IsActive)
            {
                continue;
            }
            bool anyAvailable = HasAnyValidPlacement(block.BlockShape);
            block.SetInteractable(anyAvailable);

            if (!anyAvailable)
            {
                countNotInteractable += 1;
            }
            countActive += 1;
        }

        if (countNotInteractable == countActive && countActive > 0)
        {
            GameOver();
        }
    }
    
    public bool HasAnyValidPlacement(BlockShape shape)
    {
        int shapeWidth = shape.columns;
        int shapeHeight = shape.rows;

        for (int y = 0; y <= 9 - shapeHeight; y++) // 9 — высота поля
        {
            for (int x = 0; x <= 9 - shapeWidth; x++) // 9 — ширина поля
            {
                bool canPlace = true;

                for (int dy = 0; dy < shapeHeight; dy++)
                {
                    for (int dx = 0; dx < shapeWidth; dx++)
                    {
                        if (!shape.board[dy].column[dx]) continue; // Пропускаем пустые ячейки

                        BlockTile tile = GetTile(x + dx, y + dy);
                        if (tile == null || tile.IsOccupied)
                        {
                            canPlace = false;
                            break;
                        }
                    }

                    if (!canPlace)
                        break;
                }

                if (canPlace)
                    return true;
            }
        }

        return false;
    }

}