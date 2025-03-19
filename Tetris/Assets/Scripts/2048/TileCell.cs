using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int coordinates { get; set; }
    public Tile2024 tile { get; set; }

    public bool Empty => tile == null;
    public bool Occupied => tile != null;
}