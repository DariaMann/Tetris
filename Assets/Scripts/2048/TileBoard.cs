using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    [SerializeField] private bool isEducation;
    [SerializeField] private Tile2024 tilePrefab;
    [SerializeField] private TileState[] tileStates;

    private TileGrid _grid;
    private List<Tile2024> _tiles;
    private bool _waiting;
    
    private Vector2 _touchStartPos;
    private Vector2 _touchEndPos;
    private bool _isDragging = false;
    
    public List<Tile2024> Tiles
    {
        get => _tiles;
        set => _tiles = value;
    }

    private void Awake()
    {
        _grid = GetComponentInChildren<TileGrid>();
        _tiles = new List<Tile2024>(16);
    }

    public void ClearBoard()
    {
        foreach (var cell in _grid.Cells) {
            cell.Tile = null;
        }

        foreach (var tile in _tiles) {
            Destroy(tile.gameObject);
        }

        _tiles.Clear();
    }
    
    public void CreateTile(SaveTile2024 saveTile)
    {
        Tile2024 tile = Instantiate(tilePrefab, _grid.transform);
        tile.SetState(tileStates[saveTile.StateNumber], true);
        tile.Spawn(_grid.GetCellByCoordinates(saveTile.X, saveTile.Y), _grid.transform);
        _tiles.Add(tile);
    }

    public void CreateTile()
    {
        Tile2024 tile = Instantiate(tilePrefab, _grid.transform);
        tile.PlaySpawnAnimation();
        tile.SetState(tileStates[0]);
        tile.Spawn(_grid.GetRandomEmptyCell(), _grid.transform);
        _tiles.Add(tile);
    }
    
    public void CreateTile(int x, int y)
    {
        Tile2024 tile = Instantiate(tilePrefab, _grid.transform);
        tile.PlaySpawnAnimation();
        tile.SetState(tileStates[0]);
        tile.Spawn(_grid.GetCellByCoordinates(x, y), _grid.transform);
        _tiles.Add(tile);
        
        GameManager.Instance.Education.ChangeStepAfterTouch();
    }

    private void Update()
    {
        if (_waiting) return;
        if (GameHelper.IsEdication && !isEducation) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            Move(Vector2Int.up, 0, 1, 1, 1);
        } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
            Move(Vector2Int.left, 1, 1, 0, 1);
        } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            Move(Vector2Int.down, 0, 1, _grid.Height - 2, -1);
        } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
            Move(Vector2Int.right, _grid.Width - 2, -1, 0, 1);
        }
        
        // Мобильные свайпы
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase) {
                case TouchPhase.Began:
                    _touchStartPos = touch.position;
                    break;
                case TouchPhase.Ended:
                    _touchEndPos = touch.position;
                    DetectSwipe();
                    break;
            }
        }

        // Перетаскивание мышью
        if (Input.GetMouseButtonDown(0)) {
            _touchStartPos = Input.mousePosition;
            _isDragging = true;
        }
        if (Input.GetMouseButtonUp(0) && _isDragging) {
            _touchEndPos = Input.mousePosition;
            _isDragging = false;
            DetectSwipe();
        }
    }
    
    private void DetectSwipe() 
    {
        Vector2 swipeDelta = _touchEndPos - _touchStartPos;

        if (swipeDelta.magnitude < 50) return; // Игнорируем маленькие движения

        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y)) {
            if (swipeDelta.x > 0) {
                Move(Vector2Int.right, _grid.Width - 2, -1, 0, 1);
            } else {
                Move(Vector2Int.left, 1, 1, 0, 1);
            }
        } else {
            if (swipeDelta.y > 0) {
                Move(Vector2Int.up, 0, 1, 1, 1);
            } else {
                Move(Vector2Int.down, 0, 1, _grid.Height - 2, -1);
            }
        }
    }

    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        if (isEducation)
        {
            if (GameManager.Instance.EnableMoveDirection != direction)
            {
                return;
            }
        }
        bool changed = false;
        bool saveLast = false;
        bool isMerge = false;
        
        for (int x = startX; x >= 0 && x < _grid.Width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < _grid.Height; y += incrementY)
            {
                TileCell cell = _grid.GetCell(x, y);

                if (cell.Occupied) {
                    changed |= MoveTile(cell.Tile, direction, saveLast, ref isMerge);
                    saveLast = changed;
                }
            }
        }

        if (changed) {
            if (isMerge)
            {
                AudioManager.Instance.PlaySuccessLineSound();
            }
            else
            {
                 AudioManager.Instance.PlayClickChipSound();
            }
            if (isEducation)
            {
                GameManager.Instance.Education.SetEnableMoveDirection(Vector2Int.zero);
                GameManager.Instance.Education.HideTwoStep();
                StartCoroutine(WaitForChangesEducationStepTwo());
                return;
            }
            StartCoroutine(WaitForChanges());
        }

        GameManager.Instance.CheckUndoButtonState();
    }

    private bool MoveTile(Tile2024 tile, Vector2Int direction, bool saveLast, ref bool isMerge)
    {
        TileCell newCell = null;
        TileCell adjacent = _grid.GetAdjacentCell(tile.Cell, direction);

        while (adjacent != null)
        {
            if (adjacent.Occupied)
            {
                if (CanMerge(tile, adjacent.Tile))
                {
                    if (!saveLast && !isEducation)
                    {
                        GameManager.Instance.EventSteps.Push(CreateStepEvent());
                    }
                    MergeTiles(tile, adjacent.Tile);
                    isMerge = true;
                    return true;
                }

                break;
            }

            newCell = adjacent;
            adjacent = _grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            if (!saveLast && !isEducation)
            {
                GameManager.Instance.EventSteps.Push(CreateStepEvent());
            }
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    private bool CanMerge(Tile2024 a, Tile2024 b)
    {
        return a.State == b.State && !b.Locked;
    }

    private void MergeTiles(Tile2024 a, Tile2024 b)
    {
        _tiles.Remove(a);
        a.Merge(b.Cell);

        int index = Mathf.Clamp(IndexOf(b.State) + 1, 0, tileStates.Length - 1);
        TileState newState = tileStates[index];

        b.SetState(newState);
        b.PlayMergeAnimation();
        if (isEducation)
        {
            return;
        }
        GameManager.Instance.SaveScores.ChangeScore(newState.number);
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i]) {
                return i;
            }
        }

        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        _waiting = true;

        yield return new WaitForSeconds(0.1f);

        _waiting = false;

        foreach (var tile in _tiles) {
            tile.Locked = false;
        }

        if (_tiles.Count != _grid.Size) {
            CreateTile();
        }

        if (CheckForGameOver()) {
            GameManager.Instance.GameOver();
        }
    }
    
    private IEnumerator WaitForChangesEducationStepTwo()
    {
        _waiting = true;

        yield return new WaitForSeconds(0.1f);

        _waiting = false;

        foreach (var tile in _tiles) {
            tile.Locked = false;
        }

        if (_tiles.Count != _grid.Size) {
            CreateTile(2,2);
        }

    }

    public bool CheckForGameOver()
    {
        if (_tiles.Count != _grid.Size) {
            return false;
        }

        foreach (var tile in _tiles)
        {
            TileCell up = _grid.GetAdjacentCell(tile.Cell, Vector2Int.up);
            TileCell down = _grid.GetAdjacentCell(tile.Cell, Vector2Int.down);
            TileCell left = _grid.GetAdjacentCell(tile.Cell, Vector2Int.left);
            TileCell right = _grid.GetAdjacentCell(tile.Cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.Tile)) {
                return false;
            }

            if (down != null && CanMerge(tile, down.Tile)) {
                return false;
            }

            if (left != null && CanMerge(tile, left.Tile)) {
                return false;
            }

            if (right != null && CanMerge(tile, right.Tile)) {
                return false;
            }
        }

        return true;
    }
    
    public void OnUndo()
    {
        if (GameManager.Instance.EventSteps.Count > 0)
        {
            RestoreStepEvent(GameManager.Instance.EventSteps.Pop());
        }
    }
    
    private Step2048 CreateStepEvent()
    {
        Step2048 step = new Step2048();
    
        foreach (var tile in _tiles)
        {
            TileEvent tileSnap = new TileEvent()
            {
                X = tile.Cell.Coordinates.x,
                Y = tile.Cell.Coordinates.y,
                StateIndex = tile.State.index
            };
            step.Tiles.Add(tileSnap);
        }

        step.Steps = GameManager.Instance.SaveScores.CurrentScore;

        return step;
    }
    
    private void RestoreStepEvent(Step2048 step)
    {
        ClearBoard();

        foreach (var tileSnap in step.Tiles)
        {
            Tile2024 tile = Instantiate(tilePrefab, _grid.transform);
            tile.SetState(tileStates[tileSnap.StateIndex]);
            tile.Spawn(_grid.GetCellByCoordinates(tileSnap.X, tileSnap.Y), _grid.transform);
            _tiles.Add(tile);
        }

        GameManager.Instance.SaveScores.ChangeScore(step.Steps, false);
        GameManager.Instance.CheckUndoButtonState();
    }

}