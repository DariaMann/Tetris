using UnityEngine;

[CreateAssetMenu(menuName = "Tile State")]
public class TileState : ScriptableObject
{
    public int index;
    public int number;
    public Color backgroundColor;
    public Color textColor;
}