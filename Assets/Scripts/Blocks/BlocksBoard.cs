using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlocksBoard : MonoBehaviour
{
    [SerializeField] private bool isEducation;
    [SerializeField] private SquareUIGrid squareUiGrid;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private List<BlockShape> blockTypes;
    [SerializeField] private List<Block> blocks;
    
    public int GridSize { get; set; } = 9;

    public bool IsEducation
    {
        get => isEducation;
        set => isEducation = value;
    }
    
    public SquareUIGrid SquareUiGrid
    {
        get => squareUiGrid;
        set => squareUiGrid = value;
    }
    
    public List<Block> Blocks
    {
        get => blocks;
        set => blocks = value;
    }

    public List<BlockTile> Tiles { get; set; } = new List<BlockTile>();

    private void OnEnable()
    {
        BlocksEvents.CheckIfBlockCanBePlaced += CheckIfBlockCanBePlaced;
    }

    private void OnDisable()
    {
        BlocksEvents.CheckIfBlockCanBePlaced -= CheckIfBlockCanBePlaced;
    }

    public void CreateBlocks()
    {
        foreach (var block in blocks)
        {
            int randomIndex = Random.Range(0, blockTypes.Count);
            block.CreateBlock(blockTypes[randomIndex]);
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

    public void GenerateGrid()
    {
        for (int y = 0; y < GridSize; y++)
        {
            for (int x = 0; x < GridSize; x++)
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
    
    public void CreateBlocks(List<SaveBlock> types)
    {
        for (int i = 0; i < types.Count; i++)
        {
            if (types[i].BlockShape != null)
            {
                blocks[i].CreateBlock(types[i].BlockShape);
            }

            if (!types[i].IsEnable)
            {
                blocks[i].Deactivate();
            }
        }
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
                GameManagerBlocks.Instance.Education.StartPlay();
            }
            BlocksEvents.MoveBlockToStartPosition();
            return;
        }

        if (!isEducation)
        {
            GameManagerBlocks.Instance.AddStepEventObject();
        }

        foreach (var tiles in selectedTiles)
        {
            tiles.Activate();
        }
        
        GetSelectedBlock().Deactivate();
        
        var (tilesToClear, score) = GameManagerBlocks.Instance.CheckLinesAndGetScore(this);
        
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
                        GameManagerBlocks.Instance.Education.ChangeStep();
                        return;
                    }
                    GameManagerBlocks.Instance.ShowScorePlusAnimationFromUI(tilesToClear, score);
                });
        }

        if (isEducation)
        {
            return;
        }
        
        if (score > 0)
        {
            GameManagerBlocks.Instance.SaveScores.ChangeScore(score);
        }

        if (IsAllBlocksDeactivated())
        {
            CreateBlocks();
            CheckInteractableBlocks();
        }

    }

    public BlockTile GetTile(int x, int y)
    {
        if (x < 0 || x >= GridSize || y < 0 || y >= GridSize)
            return null;

        return Tiles[y * GridSize + x];
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
        for (int i = 0; i < GridSize; i++)
        {
            var row = Tiles.Where(t => t.GridPosition.y == i).ToList();
            if (row.All(t => t.IsOccupied || t.IsSelected))
                foreach (var t in row) clearedTiles.Add(t);

            var column = Tiles.Where(t => t.GridPosition.x == i).ToList();
            if (column.All(t => t.IsOccupied || t.IsSelected))
                foreach (var t in column) clearedTiles.Add(t);
        }

        // Проверка 3x3 квадратов
        for (int by = 0; by < GridSize; by += 3)
        {
            for (int bx = 0; bx < GridSize; bx += 3)
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
#if UNITY_EDITOR
            GameManagerBlocks.Instance.GameOver();
            return;
#endif
            AppodealManager.Instance.ShowInterstitial();
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