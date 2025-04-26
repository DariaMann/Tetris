using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BlocksBoard : MonoBehaviour
{
    [SerializeField] private ThemeBlocks theme;
    [SerializeField] private SquareUIGrid squareUiGrid;
    [SerializeField] private SaveScores saveScores;
    [SerializeField] private GameOver gameOver;
    [SerializeField] private Button undoButton;
    
    [SerializeField] private int gridSize = 9;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private List<BlockShape> blockTypes;
    [SerializeField] private List<Block> blocks;

    public List<Block> Blocks
    {
        get => blocks;
        set => blocks = value;
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
        LoadLastPlay();
        CheckUndoButtonState();
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
        SaveLastPlay();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveLastPlay();
        }
    }
    
    private void OnDestroy()
    {
        SaveLastPlay();
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
        CheckUndoButtonState();
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
        ResetGrid();
        CreateBlocks();
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
            CheckUndoButtonState();
        }
    }
    
    public void AddStepEventObject()
    {
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
            blocks[i].CreateBlock(blockTypes[i].BlockShape);
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

        foreach (var tile in tilesToClear)
        {
            tile.Explode();
        }

        if (score > 0)
        {
            saveScores.ChangeScore(score);
        }

        if (IsAllBlocksDeactivated())
        {
            CreateBlocks();
        }
        
//        if (!CheckGameOver(blocks))
//        {
//            Debug.Log("Игра окончена — нет возможных ходов!");
//            GameOver();
//        }
    }

//    public bool CheckShowHoover(BlockTile checkTile)
//    {
//        var selectedTiles = new List<BlockTile>();
//        foreach (var tile in Tiles)
//        {
//            if (tile.IsSelected && !tile.IsOccupied)
//            {
//                selectedTiles.Add(tile);
////                tile.Activate();
//            }
//        }
//
//        var selectedBlock = GetSelectedBlock();
//        if (selectedBlock == null)
//        {
//            return false;
//        }
//
//        if (selectedBlock.TotalSquareNumber != selectedTiles.Count)
//        {
//            return false;
//        }
//
//        if (selectedTiles.Contains(checkTile))
//        {
//            return true;
//        }
//
//        return false;
//    }
    
    BlockTile GetTile(int x, int y) => Tiles[y * gridSize + x];

    private (List<BlockTile> tilesToClear, int score) CheckLinesAndGetScore()
    {
        List<BlockTile> tilesToClear = new List<BlockTile>();
        HashSet<BlockTile> uniqueTiles = new HashSet<BlockTile>();
        int score = 0;

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

                score += 10; // например, 10 баллов за строку
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

                score += 10; // например, 10 баллов за колонку
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

                    score += 20; // например, 20 баллов за квадрат 3x3
                }
            }
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

        var blocks = draggedBlock.Squares;

        if (blocks.Count == 0)
            return result;

        // 1. Выбираем "опорный" блок — например, первый в списке
        var pivotBlock = blocks[0];
        Vector2 pivotWorldPos = pivotBlock.transform.position;

        // 2. Ищем ближайший Tile к pivot-блоку
        var centerTile = Tiles
            .Where(t => !t.IsOccupied)
            .OrderBy(t => Vector2.Distance(t.transform.position, pivotWorldPos))
            .FirstOrDefault();

        if (centerTile == null)
            return result;

        Vector2 centerTilePos = centerTile.transform.position;

        // 3. Считаем смещения от pivot-блока до всех остальных
        foreach (var block in blocks)
        {
            Vector2 offset = (Vector2)block.transform.position - pivotWorldPos;
            Vector2 targetPos = centerTilePos + offset;

            var nearestTile = Tiles
                .OrderBy(t => Vector2.Distance(t.transform.position, targetPos))
                .FirstOrDefault();

            if (nearestTile == null || nearestTile.IsOccupied || result.Contains(nearestTile))
            {
                return new List<BlockTile>(); // хотя бы одна клетка недоступна — всё отменяем
            }

            result.Add(nearestTile);
        }

        return result;
    }


//    public List<BlockTile> GetHoveredTilesByProximity(List<GameObject> blocks)
//    {
//        var hovered = new List<BlockTile>();
//
//        var allTiles = Tiles; // доступ к списку всех BlockTile
//
////        var blocks = GetComponentsInChildren<Transform>()
////            .Where(t => t.CompareTag("Block")).ToList();
//
//        foreach (var block in blocks)
//        {
//            var closest = allTiles
//                .Where(t => !t.IsOccupied)
//                .OrderBy(t => Vector2.Distance(t.transform.position, block.transform.position))
//                .FirstOrDefault();
//
//            if (closest != null && !hovered.Contains(closest))
//            {
//                hovered.Add(closest);
//            }
//        }
//
//        return hovered;
//    }

}