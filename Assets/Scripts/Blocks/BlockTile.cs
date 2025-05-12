using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BlockTile : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image hooverImage;
    [SerializeField] private Image activeImage;
    [SerializeField] private Image futureDeleteImage;
    
    private bool _IsDark;
    private RectTransform _rectTransform;
    private Sprite _mainLightSprite;
    private Sprite _mainDarkSprite;
    private Image _mainImage;

    public bool IsSelected { get; set; }
    
    public bool IsOccupied { get; set; }
    
    public bool TempOccupied { get; set; }
    
    public BlocksBoard BlocksBoard { get; set; }
    public Vector2Int GridPosition { get; set; }
    
    public Block Block  { get; private set; }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Index: "+ BlocksBoard.Tiles.IndexOf(this) + "Row = " + GridPosition.x + ", Col = " + GridPosition.y);
    }

    private void Awake()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
        _mainImage = GetComponent<Image>();
    }

    public void SetData(Vector2Int pos, BlocksBoard blocksBoard)
    {
        BlocksBoard = blocksBoard;
        GridPosition = pos;
        RemoveBall();
    }

    public void SetTheme(Sprite hooverSprite, Sprite activeSprite, Sprite mainLightSprite, Sprite mainDarkSprite)
    {
//        image.sprite = sprite;
        hooverImage.sprite = hooverSprite;
        activeImage.sprite = activeSprite;
        _mainLightSprite = mainLightSprite;
        _mainDarkSprite = mainDarkSprite;
        SetColor(_IsDark);
    }
    
    public void SetColor(bool isDark)
    {
        _IsDark = isDark;
        if (isDark)
        {
            _mainImage.sprite = _mainDarkSprite;
        }
        else
        {
            _mainImage.sprite = _mainLightSprite;
        }
    }    
    
    public void RemoveBall()
    {
        Block = null;
    }

    public void Activate()
    {
        hooverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
        IsSelected = true;
        IsOccupied = true;
    }
    
    public void Deactivate()
    {
        hooverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(false);
        IsSelected = false;
        IsOccupied = false;
    }
    
    public void ActivateFutureDelete(bool isActivate)
    {
        futureDeleteImage.gameObject.SetActive(isActivate);
    }
    
    public Tween Explode()
    {
        // Сохраняем начальные значения
        Vector3 originalScale = _rectTransform.localScale;
        Color originalColor = activeImage.color;

        Sequence sequence = DOTween.Sequence();

        // Увеличение масштаба и затем исчезновение
        sequence.Append(_rectTransform.DOScale(originalScale * 1.2f, 0.2f).SetEase(Ease.OutBack));
        sequence.Join(activeImage.DOFade(0f, 0.2f));

        // После анимации
        sequence.OnComplete(() =>
        {
            Deactivate();

            // Возврат в начальное состояние
            _rectTransform.localScale = originalScale;

            var color = activeImage.color;
            color.a = originalColor.a;
            activeImage.color = color;
        });

        return sequence;
    }
    
    public void ActivateSelected(bool isActivate)
    {
        if (IsOccupied)
        {
            return;
        }

        IsSelected = isActivate;
        hooverImage.gameObject.SetActive(isActivate);
    }
}