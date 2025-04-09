using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int Coordinates { get; set; }
    public Tile2024 Tile { get; set; }

    public bool Empty => Tile == null;
    public bool Occupied => Tile != null;
}