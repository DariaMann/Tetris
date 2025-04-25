using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D)), RequireComponent(typeof(Image))]
public class BlockSquare: MonoBehaviour
{
    [SerializeField] private Image occupideImage;

    private RectTransform _rectTransform;
    private BoxCollider2D _boxCollider;
    private Image _mainImage;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _mainImage = GetComponent<Image>();
    }

    private void Start()
    {
        occupideImage.gameObject.SetActive(false);
    }

    public void SetTheme(Sprite mainSprite)
    {
        _mainImage.sprite = mainSprite;
    }
    
    public void SetSize(Vector2 squareSize)
    {
        _rectTransform.sizeDelta = squareSize;
    }
    
    public void SetPosition(Vector2 newPos)
    {
        _rectTransform.localPosition = newPos;
    }
    
    public void Activate()
    {
        _boxCollider.enabled = true;
        gameObject.SetActive(true);
    }
    
    public void Deactivate()
    {
        _boxCollider.enabled = false;
        gameObject.SetActive(false);
    }
}