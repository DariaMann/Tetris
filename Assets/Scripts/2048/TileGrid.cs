using System.Linq;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public TileRow[] Rows { get; private set; }
    public TileCell[] Cells { get; private set; }

    public int Size => Cells.Length;
    public int Height => Rows.Length;
    public int Width => Size / Height;

    private void Awake()
    {
        Rows = GetComponentsInChildren<TileRow>();
        Cells = GetComponentsInChildren<TileCell>();

        for (int i = 0; i < Cells.Length; i++) {
            Cells[i].Coordinates = new Vector2Int(i % Width, i / Width);
        }
    }

    public TileCell GetCell(Vector2Int coordinates)
    {
        return GetCell(coordinates.x, coordinates.y);
    }

    public TileCell GetCell(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height) {
            return Rows[y].Cells[x];
        } else {
            return null;
        }
    }

    public TileCell GetAdjacentCell(TileCell cell, Vector2Int direction)
    {
        Vector2Int coordinates = cell.Coordinates;
        coordinates.x += direction.x;
        coordinates.y -= direction.y;

        return GetCell(coordinates);
    }

    public TileCell GetCellByCoordinates(int x, int y)
    {
        TileCell tileCell = Cells.ToList().Find(tile => tile.Coordinates.x == x && tile.Coordinates.y == y);
        return tileCell;
    } 

    public TileCell GetRandomEmptyCell()
    {
        int index = Random.Range(0, Cells.Length);
        int startingIndex = index;

        while (Cells[index].Occupied)
        {
            index++;

            if (index >= Cells.Length) {
                index = 0;
            }

            // all cells are occupied
            if (index == startingIndex) {
                return null;
            }
        }

        return Cells[index];
    }

}