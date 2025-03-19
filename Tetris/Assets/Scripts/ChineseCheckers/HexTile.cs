using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexTile : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Vector2 position;
    [SerializeField] private GameObject selection;
    
    private CheckersManager _checkersManager;

    public int Row { get; private set; }

    public int Col { get; private set; }

    public bool IsOccupied { get; private set; }
    
    public Chip Chip { get; private set; }
    
    public bool IsSelected { get; private set; }
    
    public Vector2 Position
    {
        get => position;
        set => position = value;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Row = " + Row + ", Col = " + Col);
        if (_checkersManager.SelectedChip != null && IsSelected)
        {
            StartCoroutine(PlayerMoveCoroutine());
        }
    }
    
    public IEnumerator PlayerMoveCoroutine()
    {
        yield return StartCoroutine(_checkersManager.SelectedChip.MoveCoroutine(this));
        _checkersManager.StartNextTurn();
    }

    public void SetData(Vector2 pos, int row, int col, CheckersManager checkersManager)
    {
        Position = pos;
        Row = row;
        Col = col;
        _checkersManager = checkersManager;
        SetOccupied(false);
    }
    
    public void SetSelection(bool selected, bool showSelection = true)
    {
        IsSelected = selected;
        selection.SetActive(showSelection && selected);
    }
    
    public void SetOccupied(bool occupied)
    {
        IsOccupied = occupied;
    }
    
    public void SetChip(Chip chip)
    {
        Chip = chip;
        SetOccupied(true);
    }
    
    public void RemoveChip()
    {
        Chip = null;
        SetOccupied(false);
    }
}