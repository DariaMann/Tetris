using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    [SerializeField] private Tile2024 tilePrefab;
    [SerializeField] private TileState[] tileStates;

    private TileGrid grid;
    private List<Tile2024> tiles;
    private bool waiting;
    
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private bool isDragging = false;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile2024>(16);
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.cells) {
            cell.tile = null;
        }

        foreach (var tile in tiles) {
            Destroy(tile.gameObject);
        }

        tiles.Clear();
    }

    public void CreateTile()
    {
        Tile2024 tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0]);
        tile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }

    private void Update()
    {
        if (waiting) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            Move(Vector2Int.up, 0, 1, 1, 1);
        } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
            Move(Vector2Int.left, 1, 1, 0, 1);
        } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            Move(Vector2Int.down, 0, 1, grid.Height - 2, -1);
        } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
            Move(Vector2Int.right, grid.Width - 2, -1, 0, 1);
        }
        
        // Мобильные свайпы
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase) {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    break;
                case TouchPhase.Ended:
                    touchEndPos = touch.position;
                    DetectSwipe();
                    break;
            }
        }

        // Перетаскивание мышью
        if (Input.GetMouseButtonDown(0)) {
            touchStartPos = Input.mousePosition;
            isDragging = true;
        }
        if (Input.GetMouseButtonUp(0) && isDragging) {
            touchEndPos = Input.mousePosition;
            isDragging = false;
            DetectSwipe();
        }
    }
    
    private void DetectSwipe() 
    {
        Vector2 swipeDelta = touchEndPos - touchStartPos;

        if (swipeDelta.magnitude < 50) return; // Игнорируем маленькие движения

        if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y)) {
            if (swipeDelta.x > 0) {
                Move(Vector2Int.right, grid.Width - 2, -1, 0, 1);
            } else {
                Move(Vector2Int.left, 1, 1, 0, 1);
            }
        } else {
            if (swipeDelta.y > 0) {
                Move(Vector2Int.up, 0, 1, 1, 1);
            } else {
                Move(Vector2Int.down, 0, 1, grid.Height - 2, -1);
            }
        }
    }

    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;

        for (int x = startX; x >= 0 && x < grid.Width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.Height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);

                if (cell.Occupied) {
                    changed |= MoveTile(cell.tile, direction);
                }
            }
        }

        if (changed) {
            StartCoroutine(WaitForChanges());
        }
    }

    private bool MoveTile(Tile2024 tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.Occupied)
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    MergeTiles(tile, adjacent.tile);
                    return true;
                }

                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    private bool CanMerge(Tile2024 a, Tile2024 b)
    {
        return a.state == b.state && !b.locked;
    }

    private void MergeTiles(Tile2024 a, Tile2024 b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        TileState newState = tileStates[index];

        b.SetState(newState);
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
        waiting = true;

        yield return new WaitForSeconds(0.1f);

        waiting = false;

        foreach (var tile in tiles) {
            tile.locked = false;
        }

        if (tiles.Count != grid.Size) {
            CreateTile();
        }

        if (CheckForGameOver()) {
            GameManager.Instance.GameOver();
        }
    }

    public bool CheckForGameOver()
    {
        if (tiles.Count != grid.Size) {
            return false;
        }

        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile)) {
                return false;
            }

            if (down != null && CanMerge(tile, down.tile)) {
                return false;
            }

            if (left != null && CanMerge(tile, left.tile)) {
                return false;
            }

            if (right != null && CanMerge(tile, right.tile)) {
                return false;
            }
        }

        return true;
    }

}