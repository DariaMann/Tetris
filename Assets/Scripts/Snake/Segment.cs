using UnityEngine;

public class Segment : MonoBehaviour
{
    public Vector2Int CurrentCell { get; set; } 
    
    public Vector2Int NextCell { get; set; } 
    
    public Vector2Int LastCell { get; set; }

    public void SetFirstCurrentPosition(Vector2Int currentCell)
    {
        LastCell = currentCell;
        CurrentCell = currentCell;
        transform.position = new Vector2(CurrentCell.x, CurrentCell.y);
    }
    
    public void SetCurrentPosition()
    {
        LastCell = CurrentCell;
        CurrentCell = NextCell;
        transform.position = new Vector2(NextCell.x, NextCell.y);
    }
}