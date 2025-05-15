using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Ball : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image image;
    [SerializeField] private BounceEffect bounceEffect;
    [SerializeField] private float moveSpeed = 2;
    [SerializeField] private float appearDuration = 0.3f;
    [SerializeField] private float startScale = 0.4f;
    
    private LineBoard _lineBoard;
    private RectTransform _rectTransform;
    
    public LineTile Tile  { get; set; }
    public int IndexSprite  { get; set; }
    
    public bool IsEnabled  { get; private set; }
    
    public bool IsVisible  { get; set; }
    
    public bool IsSelected  { get; set; }

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Ball: Row = " + Tile.GridPosition.x + ", Col = " + Tile.GridPosition.y);
        if (IsEnabled)
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
        
        if (selected)
        {
            _lineBoard.SetSelection(this);
            ActivateJumpAnimation(true);
            AudioManager.Instance.PlayClickChipSound();
        }
        else
        {
            _lineBoard.SetSelection(null);
            ActivateJumpAnimation(false);
        }
    }
    
    public void ActivateJumpAnimation(bool activate)
    {
        if (activate)
        {
            bounceEffect.StartBounce();
        }
        else
        {
            bounceEffect.StopBounce();
        }
    }
    
    public void ShakingAnimation()
    {
        bounceEffect.ShakeOnce();
    }   
    
    public void ExplodeAnimation()
    {
        bounceEffect.Explode();
    }

    public void SetData(Sprite sprite, int index, LineTile tile, LineBoard lineBoard)
    {
        _lineBoard = lineBoard;
        IndexSprite = index;
        image.sprite = sprite;
        Tile = tile;
    }
    
    public void DisabledBall()
    {
        IsEnabled = false;
        image.raycastTarget = IsEnabled;
        if (GameManagerLines98.Instance.ShowFuture)
        {
            _rectTransform.localScale = Vector3.one * startScale;
        }
        else
        {
            _rectTransform.localScale = Vector3.zero;
        }
    }
    
    public void EnabledBall()
    {
        if (IsEnabled)
        {
            return;
        }

        IsEnabled = true;
        image.raycastTarget = IsEnabled;
        _rectTransform.DOScale(Vector3.one, appearDuration).SetEase(Ease.OutBack); // увеличиваем до нормального
    }
    
    public void FastMove(LineTile toTile)
    {
        Tile.RemoveBall();
        
        Tile = toTile;
        toTile.SetBall(this);

        // Возвращаем под Tile и выравниваем
        transform.SetParent(Tile.transform, false);
        _rectTransform.anchoredPosition = Vector2.zero;

        // Возвращаем Stretch, если нужно
        _rectTransform.anchorMin = Vector2.zero;
        _rectTransform.anchorMax = Vector2.one;
        _rectTransform.offsetMin = new Vector2(6, 6);   // Left & Bottom
        _rectTransform.offsetMax = new Vector2(-6, -6); // Right & Top
    }
    
    public IEnumerator MoveCoroutine(List<LineTile> path, LineTile finishTile)
    {
        yield return StartCoroutine(MoveStepByStep(path, finishTile));
    }
    
    private IEnumerator MoveStepByStep(List<LineTile> path, LineTile finishTile)
    {
        SetSelection(false);

        // Сохраняем текущий размер
        Vector2 originalSize = _rectTransform.rect.size;

        // Отключаем Stretch
        _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        _rectTransform.pivot = new Vector2(0.5f, 0.5f);
        _rectTransform.sizeDelta = originalSize;

        // Временно перемещаем в Canvas (на экран)
        transform.SetParent(_lineBoard.transform, true); // true — сохраняем мировую позицию

        Tile.RemoveBall();
        LineTile lastTile = Tile;

        foreach (LineTile step in path)
        {
            if (lastTile == step) continue;

            Vector3 targetWorldPos = step.transform.position;

            while (Vector3.Distance(transform.position, targetWorldPos) > 0.01f)
            {
                while (GameHelper.IsPause)
                    yield return null;

                transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
            
            lastTile = step;

//            Tile = step;
//            Tile.SetBall(this);
        }
        Tile = finishTile;
        finishTile.SetBall(this);

        // Возвращаем под Tile и выравниваем
        transform.SetParent(Tile.transform, false);
        _rectTransform.anchoredPosition = Vector2.zero;

        // Возвращаем Stretch, если нужно
        _rectTransform.anchorMin = Vector2.zero;
        _rectTransform.anchorMax = Vector2.one;
        _rectTransform.offsetMin = new Vector2(6, 6);   // Left & Bottom
        _rectTransform.offsetMax = new Vector2(-6, -6); // Right & Top
    }

}