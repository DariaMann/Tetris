using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Chip : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float moveSpeed = 2;
    
    private CheckersManager _checkersManager;
    private List<HexTile> _availableTiles = new List<HexTile>();
    
    public HexTile Tile { get; private set; }
    
    public Player Player { get; set; }
    public bool IsSelected { get; private set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Row = " + Tile.Row + ", Col = " + Tile.Col);
        if (Player.State == PlayerState.Player && _checkersManager.CurrentPlayer == Player)
        {
            SetSelection(true);
        }
    }
    
    public void SetSelection(bool selected)
    {
        if (IsSelected == selected)
        {
            return;
        }
        IsSelected = selected;
        Tile.SetSelection(selected);
        if (selected)
        {
            _checkersManager.SetSelection(this);
            ShowAvailableWay();
        }
        else
        {
            _checkersManager.SetSelection(null);
            HideAvailableWay();
        }
    }
    
    public void ShowAvailableWay()
    {
        if (_availableTiles.Count > 0)
        {
            Debug.LogError("Не удалены пути");
        }
        // Поиск возможных ходов
        _availableTiles = _checkersManager.FindWays(this);

        // Выделение возможных ходов (можно менять цвет)
        foreach (HexTile tile in _availableTiles)
        {
            tile.SetSelection(true, _checkersManager.ShowHint);
        }
    }
    
    public void HideAvailableWay()
    {
        foreach (var tile in _availableTiles)
        {
            tile.SetSelection(false);
        }
        _availableTiles.Clear();
    }
    
    public void SetData(HexTile tile, CheckersManager checkersManager)
    {
        _checkersManager = checkersManager;
//        transform.localPosition = tile.Position;
        Tile = tile;
        tile.SetChip(this);
        SetSelection(false);
    }
    
    public IEnumerator MoveCoroutine(HexTile toTile)
    {
        List<HexTile> path = _checkersManager.GetPathToTarget(Tile, toTile);
        if (path.Count > 0)
        {
            Player.IsPlaying = true;
            yield return StartCoroutine(MoveStepByStep(path));
            Player.IsPlaying = false;
        }
    }
    
//    public void Move(HexTile toTile)
//    {
//        List<HexTile> path = _checkersManager.GetPathToTarget(Tile, toTile);
//        if (path.Count > 0)
//        {
//            StartCoroutine(MoveStepByStep(path));
//        }
//    }

    private IEnumerator MoveStepByStep(List<HexTile> path)
    {
        HexTile fromTile = Tile;
        SetSelection(false);
        foreach (HexTile step in path)
        {
            if (Tile == step)
            {
                continue;
            }
            while (Vector3.Distance(transform.position, step.Position) > 0.01f)
            {
                // Ждем, пока пауза не закончится
                while (_checkersManager.IsPaused)
                {
                    yield return null;
                }
                transform.position = Vector3.MoveTowards(transform.position, step.Position, moveSpeed * Time.deltaTime);
                yield return null;
            }
            Tile.RemoveChip();

            transform.position = step.Position;
            Tile = step;
            Tile.SetChip(this);
        }
        
        _checkersManager.AddStepEventObject(fromTile, this);
        
        if (_checkersManager.CheckWin(Player))
        {
            _checkersManager.PlayerFinish(Player);
        }
    }
    
    public void UndoMove(HexTile newTile)
    {
        Tile.RemoveChip();
        
        transform.position = newTile.Position;
        Tile = newTile;
        Tile.SetChip(this);
    }


//    public void Move(HexTile toTile)
//    {
//        StartCoroutine(MoveCoroutine(toTile));
//    }
//    
//    private IEnumerator MoveCoroutine(HexTile toTile)
//    {
//        SetSelection(false);
//        
//        while (Vector3.Distance(transform.position, toTile.Position) > 0.01f)
//        {
//            transform.position = Vector3.MoveTowards(transform.position, toTile.Position, moveSpeed * Time.deltaTime);
//            yield return null;
//        }
//        Tile.RemoveChip();
//        
//        transform.position = toTile.Position;
//        Tile = toTile;
//        toTile.SetChip(this);
//    }
}