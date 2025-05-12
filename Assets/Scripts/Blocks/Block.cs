using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private BlocksBoard blocksBoard;
    [SerializeField] private GameObject blockImage;
    [SerializeField] private float offsetX;
    [SerializeField] private Vector2 squareSize = new Vector2(58, 58);
    [SerializeField] private Vector3 blockSelectedScale = new Vector3(1, 1, 1);
    [SerializeField] private Vector2 offset = new Vector2(0, 500 );

    private Vector2 _startAnchoredPosition;
    private Vector2 _startSizeDelta;
    private Vector2 _startPivot;
    private Vector2 _startAnchorMin;
    private Vector2 _startAnchorMax;
    
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private bool _isDraggable = true;
    private int _originalSiblingIndex;
    
    public Vector3 StartScale { get; set; } = new Vector3(0.7f, 0.7f, 0.7f);
    
    public float OffsetX
    {
        get => offsetX;
        set => offsetX = value;
    }

    public bool IsActive { get; set; } = true;

    public bool IsSelected { get; set; }

    public bool IsInteractable { get; set; } = true;
    
    public int TotalSquareNumber { get; set; }
    
    public BlockShape BlockShape { get; set; }
    
    public List<BlockSquare> Squares { get; set; } = new List<BlockSquare>();

    private void Awake()
    {
        _canvas = gameObject.GetComponentInParent<Canvas>();
        _rectTransform = gameObject.GetComponent<RectTransform>();
        
        _startAnchoredPosition = new Vector2(OffsetX, 0);
        _startSizeDelta = _rectTransform.sizeDelta;
        _startPivot = _rectTransform.pivot;
        _startAnchorMin = _rectTransform.anchorMin;
        _startAnchorMax = _rectTransform.anchorMax;
        
        _isDraggable = true; 
        IsActive = true;
    }

    private void OnEnable()
    {
        BlocksEvents.MoveBlockToStartPosition += ResetRectTransform;
    }

    private void OnDisable()
    {
        BlocksEvents.MoveBlockToStartPosition -= ResetRectTransform;
    }

    public void SetTheme(Sprite mainSprite)
    {
        foreach (var square in Squares)
        {
            square.SetTheme(mainSprite);
        }
    }
    
    public void SetInteractable(bool isActivate)
    {
        IsInteractable = isActivate;
        foreach (var square in Squares)
        {
            square.SetInteractable(isActivate);
        }
    }

    private void ResetRectTransform()
    {
        _rectTransform.anchoredPosition = _startAnchoredPosition;
        _rectTransform.sizeDelta = _startSizeDelta;
        _rectTransform.pivot = _startPivot;
        _rectTransform.anchorMin = _startAnchorMin;
        _rectTransform.anchorMax = _startAnchorMax;
    }

    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        foreach (var square in Squares)
        {
            square?.GetComponent<BlockSquare>().Activate();
        }

        IsActive = true;
    }
    
    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        foreach (var square in Squares)
        {
            square.Deactivate();
        }

        IsActive = false;
    }
    
    private void ResetBlock()
    {
        ResetRectTransform();
        foreach (var square in Squares)
        {
            Destroy(square.gameObject);
        }
        Squares.Clear();
    }

    public void CreateBlock(BlockShape blockShape)
    {
        ResetBlock();
        Activate();
        
        BlockShape = blockShape;
        TotalSquareNumber = GetNumberEnableSquares(blockShape);
        while (Squares.Count <= TotalSquareNumber)
        {
            GameObject squareGo = Instantiate(blockImage, transform);
            BlockSquare square = squareGo.GetComponent<BlockSquare>();
            Squares.Add(square);
        }
        
        Resize(blocksBoard.SquareUiGrid.CellSize);
        
//        squareSize = new Vector2(blocksBoard.SquareUiGrid.CellSize, blocksBoard.SquareUiGrid.CellSize);
//
//        foreach (var square in Squares)
//        {
//            RectTransform rect = square.GetComponent<RectTransform>();
//            rect.sizeDelta = squareSize;
//            
//            square.gameObject.transform.position = Vector3.zero;
//            square.gameObject.SetActive(false);
//        }
//
////        var squareRect = blockImage.GetComponent<RectTransform>();
////        var moveDistance = new Vector2(squareRect.rect.width*squareRect.localScale.x,
////            squareRect.rect.height*squareRect.localScale.y);
//        
//        var moveDistance = squareSize;
//
//        int currentIndexInList = 0;
//
//        for (int row = 0; row < blockShape.rows; row++)
//        {
//            for (int column = 0; column < blockShape.columns; column++)
//            {
//                if (blockShape.board[row].column[column])
//                {
//                    Squares[currentIndexInList].SetActive(true);
//                    Squares[currentIndexInList].GetComponent<RectTransform>().localPosition = 
//                        new Vector2(GetXPositionForBlockSquare(blockShape, column, moveDistance),
//                            GetYPositionForBlockSquare(blockShape, row, moveDistance));
//
//                    currentIndexInList++;
//                }
//            }
//        }

        for (int i = Squares.Count - 1; i >= 0; i--)
        {
            if (!Squares[i].gameObject.activeSelf)
            {
                GameObject hideBlock = Squares[i].gameObject;
                Squares.Remove(Squares[i]);
                Destroy(hideBlock);
            }
        }

        SetTheme(blocksBoard.ThemeBlocks.GetTileSprite(GameHelper.Theme));
    }

    public void RepositionX(float offset)
    {
        if (_rectTransform == null)
        {
            return;
        }
        OffsetX = offset;
        _startAnchoredPosition = new Vector2(OffsetX, 0);
        
        if (!IsSelected)
        {
            _rectTransform.anchoredPosition = _startAnchoredPosition;
        }
    }

    public void Rescale(float newScale)
    {
        if (_rectTransform == null)
        {
            return;
        }
        StartScale = new Vector3(newScale, newScale, newScale);
        if (!IsSelected)
        {
            _rectTransform.localScale = StartScale;
        }
    }

    public void Resize(float newSize, bool isForOrientation = false)
    {
        squareSize = new Vector2(newSize, newSize);
        foreach (var square in Squares)
        {
//            RectTransform rect = square.GetComponent<RectTransform>();
//            rect.sizeDelta = squareSize;

            square.SetSize(squareSize);
            
            square.gameObject.transform.position = Vector3.zero;
            if (!isForOrientation)
            {
                square.gameObject.SetActive(false);
            }
        }

//        var squareRect = blockImage.GetComponent<RectTransform>();
//        var moveDistance = new Vector2(squareRect.rect.width*squareRect.localScale.x,
//            squareRect.rect.height*squareRect.localScale.y);
        
        var moveDistance = squareSize;

        int currentIndexInList = 0;

        for (int row = 0; row < BlockShape.rows; row++)
        {
            for (int column = 0; column < BlockShape.columns; column++)
            {
                if (BlockShape.board[row].column[column])
                {
                    if (!isForOrientation)
                    {
                        Squares[currentIndexInList].gameObject.SetActive(true);
                    }
//                    Squares[currentIndexInList].GetComponent<RectTransform>().localPosition = 
//                        new Vector2(GetXPositionForBlockSquare(BlockShape, column, moveDistance),
//                            GetYPositionForBlockSquare(BlockShape, row, moveDistance));
                    Squares[currentIndexInList].SetPosition(new Vector2(GetXPositionForBlockSquare(BlockShape, column, moveDistance),
                        GetYPositionForBlockSquare(BlockShape, row, moveDistance)));

                    currentIndexInList++;
                }
            }
        }
    }
    
    private float GetXPositionForBlockSquare(BlockShape blockShape, int column, Vector2 moveDistance)
    {
        float centerOffset = (blockShape.columns - 1) / 2f;
        return (column - centerOffset) * moveDistance.x;
    }

    private float GetYPositionForBlockSquare(BlockShape blockShape, int row, Vector2 moveDistance)
    {
        float centerOffset = (blockShape.rows - 1) / 2f;
        return (centerOffset - row) * moveDistance.y;
    }

//    private float GetYPositionForBlockSquare(BlockShape blockShape, int row, Vector2 moveDistance)
//    {
//        float shiftOnY = 0f;
//        if (blockShape.rows > 1)
//        {
//            if (blockShape.rows % 2 != 0)
//            {
//                var middleSquareIndex = (blockShape.rows - 1) / 2;
//                var multiplier = (blockShape.rows - 1) / 2;
//                if (row < middleSquareIndex)
//                {
//                    shiftOnY = moveDistance.y * 1;
//                    shiftOnY *= multiplier;
//                }
//                else if (row > middleSquareIndex)
//                {
//                    shiftOnY = moveDistance.y * -1;
//                    shiftOnY *= multiplier;
//                }
//            }
//            else
//            {
//                var middleSquareIndex2 = (blockShape.rows == 2) ? 1 : (blockShape.rows / 2);
//                var middleSquareIndex1 = (blockShape.rows == 2) ? 0 : (blockShape.rows - 2);
//                var multiplier = (blockShape.rows) / 2;
//                
//                if (row == middleSquareIndex1 || row == middleSquareIndex2)
//                {
//                    if (row == middleSquareIndex2)
//                    {
//                        shiftOnY = (moveDistance.y / 2) * -1;
//                    } 
//                    if (row == middleSquareIndex1)
//                    {
//                        shiftOnY = moveDistance.y / 2;
//                    } 
//                }
//                if (row < middleSquareIndex1 && row < middleSquareIndex2)
//                {
//                    shiftOnY = moveDistance.y * 1;
//                    shiftOnY *= multiplier;
//                }
//                else if (row > middleSquareIndex1 && row > middleSquareIndex2)
//                {
//                    shiftOnY = moveDistance.y * -1;
//                    shiftOnY *= multiplier;
//                }
//            }
//        }
//
//        return shiftOnY;
//    }
//
//    private float GetXPositionForBlockSquare(BlockShape blockShape, int column, Vector2 moveDistance)
//    {
//        float shiftOnX = 0f;
//        if (blockShape.columns > 1)
//        {
//            if (blockShape.columns % 2 != 0)
//            {
//                var middleSquareIndex = (blockShape.columns - 1) / 2;
//                var multiplier = (blockShape.columns - 1) / 2;
//                if (column < middleSquareIndex)
//                {
//                    shiftOnX = moveDistance.x * -1;
//                    shiftOnX *= multiplier;
//                }
//                else if (column > middleSquareIndex)
//                {
//                    shiftOnX = moveDistance.x * 1;
//                    shiftOnX *= multiplier;
//                }
//            }
//            else
//            {
//                var middleSquareIndex2 = (blockShape.columns == 2) ? 1 : (blockShape.columns / 2);
//                var middleSquareIndex1 = (blockShape.columns == 2) ? 0 : (blockShape.columns - 1);
//                var multiplier = (blockShape.columns) / 2;
//                
//                if (column == middleSquareIndex1 || column == middleSquareIndex2)
//                {
//                    if (column == middleSquareIndex2)
//                    {
//                        shiftOnX = moveDistance.x / 2;
//                    } 
//                    if (column == middleSquareIndex1)
//                    {
//                        shiftOnX = (moveDistance.x / 2) * -1;
//                    } 
//                }
//                if (column < middleSquareIndex1 && column < middleSquareIndex2)
//                {
//                    shiftOnX = moveDistance.x * -1;
//                    shiftOnX *= multiplier;
//                }
//                else if (column > middleSquareIndex1 && column > middleSquareIndex2)
//                {
//                    shiftOnX = moveDistance.x * 1;
//                    shiftOnX *= multiplier;
//                }
//            }
//        }
//
//        return shiftOnX;
//    }

    private int GetNumberEnableSquares(BlockShape blockShape)
    {
        int number = 0;

        foreach (var row in blockShape.board)
        {
            foreach (var active in row.column)
            {
                if (active)
                {
                    number++;
                }
            }
        }

        return number;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsActive || !IsInteractable)
        {
            return;
        }
        
        AudioManager.Instance.PlayClickChipSound();
        
        IsSelected = true;
        _rectTransform.localScale = blockSelectedScale;
        
        _originalSiblingIndex = transform.GetSiblingIndex();  // Сохраняем текущий индекс в иерархии

        // Поднимаем объект, чтобы он оказался наверху среди детей
        transform.SetAsLastSibling();  // Это поставит объект в конец списка дочерних элементов родителя

        if (blocksBoard.IsEducation)
        {
            blocksBoard.Education.StopTutorial();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsInteractable)
        {
            return;
        }
        
       _rectTransform.anchorMin = new Vector2(0,0);
       _rectTransform.anchorMax = new Vector2(0,0);
       _rectTransform.pivot = new Vector2(0,0);

       Vector2 pos;
       RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
           eventData.position, Camera.main, out pos);
       _rectTransform.localPosition = pos + offset;
       
       // Получаем границы фигуры и поля
       Bounds blockBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(_canvas.transform, _rectTransform);
       RectTransform boardRect = blocksBoard.GetComponent<RectTransform>();
       Bounds boardBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(_canvas.transform, boardRect);

       // Проверяем, пересекаются ли они хотя бы частично
       if (!blockBounds.Intersects(boardBounds))
       {
           foreach (var tile in blocksBoard.Tiles)
           {
               tile.ActivateSelected(false);
               tile.ActivateFutureDelete(false);
           }

           return;
       }
       
       foreach (var tile in blocksBoard.Tiles)
       {
           tile.ActivateSelected(false);
       }

       List<BlockTile> hovered = blocksBoard.GetValidHoveredTiles(this);

       if (blocksBoard.IsEducation)
       {
           if (hovered.Count > 0 && hovered[0] == blocksBoard.EnableTile)
           {
               
           }
           else
           {
               return;
           }
       }
//       var hovered = blocksBoard.GetHoveredTilesByProximity(Squares);
       foreach (var tile in hovered)
       {
           tile.ActivateSelected(true);
//           blocksBoard.CheckPotentialForDelete();
       }
       
       foreach (var tile in blocksBoard.Tiles)
       {
           tile.ActivateFutureDelete(false); // свой метод для подсветки
       }

       var toDestroy = GetPreviewDestroyedTiles(hovered);

       foreach (var tile in toDestroy)
       {
           tile.ActivateFutureDelete(true); // свой метод для подсветки
       }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsInteractable)
        {
            return;
        }
        
        _rectTransform.localScale = StartScale;
        BlocksEvents.CheckIfBlockCanBePlaced();
        IsSelected = false;
        
        foreach (var tile in blocksBoard.Tiles)
        {
            tile.ActivateFutureDelete(false); // свой метод для подсветки
        }
        
        // Возвращаем объект на его первоначальное место в иерархии
        transform.SetSiblingIndex(_originalSiblingIndex);  // Возвращаем на прежнее место по индексу
    }

    public List<BlockTile> GetPreviewDestroyedTiles(List<BlockTile> hoveredTiles)
    {
        HashSet<BlockTile> result = new HashSet<BlockTile>();

        foreach (var tile in blocksBoard.Tiles)
        {
            bool isOccupied = tile.IsOccupied || hoveredTiles.Contains(tile);
            tile.TempOccupied = isOccupied; // создадим временное поле, чтобы ничего реально не менять
        }

        int size = 9; // размер поля 9x9

        // Проверка по строкам
        for (int y = 0; y < size; y++)
        {
            bool fullRow = true;
            List<BlockTile> rowTiles = new List<BlockTile>();

            for (int x = 0; x < size; x++)
            {
                var tile = blocksBoard.GetTile(x, y);
                rowTiles.Add(tile);
                if (!tile.TempOccupied)
                {
                    fullRow = false;
                }
            }

            if (fullRow)
                result.UnionWith(rowTiles);
        }

        // Проверка по столбцам
        for (int x = 0; x < size; x++)
        {
            bool fullColumn = true;
            List<BlockTile> columnTiles = new List<BlockTile>();

            for (int y = 0; y < size; y++)
            {
                var tile = blocksBoard.GetTile(x, y);
                columnTiles.Add(tile);
                if (!tile.TempOccupied)
                {
                    fullColumn = false;
                }
            }

            if (fullColumn)
                result.UnionWith(columnTiles);
        }

        // Проверка по квадратам 3x3
        for (int blockY = 0; blockY < size; blockY += 3)
        {
            for (int blockX = 0; blockX < size; blockX += 3)
            {
                bool fullBlock = true;
                List<BlockTile> blockTiles = new List<BlockTile>();

                for (int y = 0; y < 3; y++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        var tile = blocksBoard.GetTile(blockX + x, blockY + y);
                        blockTiles.Add(tile);
                        if (!tile.TempOccupied)
                        {
                            fullBlock = false;
                        }
                    }
                }

                if (fullBlock)
                    result.UnionWith(blockTiles);
            }
        }

        // Убираем временные метки
        foreach (var tile in blocksBoard.Tiles)
        {
            tile.TempOccupied = false;
        }

        return result.ToList();
    }

}