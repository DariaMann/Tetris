using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class SquareUIGrid : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup grid;
    private RectTransform _rectTransform;
    
    public float padding = 50f; // Отступ от краёв экрана
    public int gridSize = 9;    // Кол-во клеток по одной стороне (для Lines 98 — 9)
    
    public float CellSize { get; set; }
    
    public float Padding
    {
        get => padding;
        set => padding = value;
    }

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

        ResizeSquare();
    }

    void Update()
    {
        if (GameHelper.IsDoScreenshot)
        {
            return;
        }
        
        ResizeSquare(); // Подстраиваем каждый кадр
    }

    public void ResizeSquare()
    {
        if (_rectTransform == null)
        {
            return;
        }
        RectTransform parentRect = _rectTransform.parent.GetComponent<RectTransform>();
        if (parentRect == null) return;

        float parentWidth = parentRect.rect.width;
        float parentHeight = parentRect.rect.height;

        float maxSize = Mathf.Min(parentWidth, parentHeight) - (2 * padding);
        maxSize = Mathf.Max(maxSize, 0);

        // Размер квадратного поля
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxSize);
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxSize);

        // Вычисляем размер клетки
        float spacingTotal = grid.spacing.x * (gridSize - 1);
        float cellSize = (maxSize - spacingTotal) / gridSize;

        CellSize = cellSize;

        grid.cellSize = new Vector2(cellSize, cellSize);
    }
}