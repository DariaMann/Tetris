using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LineTile : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image image;
    public LineBoard  LineBoard { get; set; }
    public Vector2Int GridPosition { get; set; }
    public Ball Ball  { get; private set; }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Row = " + GridPosition.x + ", Col = " + GridPosition.y);
        if (LineBoard.SelectedBall != null && (Ball == null || (Ball != null && !Ball.IsEnabled)))
        {
            StartCoroutine(PlayerMoveCoroutine());
        }
    }
    
    public IEnumerator PlayerMoveCoroutine()
    {
        List<LineTile> path = LineBoard.GetPathToTarget(LineBoard.SelectedBall.Tile, this);
        if (path.Count > 0)
        {
            LineBoard.AddStepEventObject();
            yield return StartCoroutine(LineBoard.SelectedBall.MoveCoroutine(path, this));
            bool haveDeleted = LineBoard.CheckLines();
            if (!haveDeleted)
            {
                LineBoard.EnabledFutureBalls();
                LineBoard.SpawnRandomBalls(LineBoard.GenerateCount, true);
                LineBoard.CheckLines();
            }
        }
        else
        {
            Debug.Log("Нет пути");
            LineBoard.ShakingBalls();
        }
    }

    public void SetData(Vector2Int pos, LineBoard lineBoard)
    {
        LineBoard = lineBoard;
        GridPosition = pos;
        RemoveBall();
    }

    public void SetTheme(Sprite sprite)
    {
        image.sprite = sprite;
    }
    
    public void SetBall(Ball ball)
    {
        if (Ball != null)
        {
            RemoveEmptyBall();
        }

        Ball = ball;
    }
    
    public void RemoveBall()
    {
        Ball = null;
    }
    
    public void RemoveEmptyBall()
    {
        LineBoard.FutureBalls.Remove(Ball);
        Destroy(Ball.gameObject);
        RemoveBall();
    }

    public bool IsEmpty()
    {
        return Ball == null || (Ball != null && !Ball.IsEnabled);
    }
}