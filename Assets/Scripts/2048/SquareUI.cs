using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SquareUI : MonoBehaviour
{
    private RectTransform _rectTransform;

    public float padding = 50f; // Отступ от краёв экрана

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        ResizeSquare();
    }

    void Update()
    {
        ResizeSquare(); // Подстраиваем каждый кадр, если изменяется размер экрана
    }

    void ResizeSquare()
    {
        RectTransform parentRect = _rectTransform.parent.GetComponent<RectTransform>();
        if (parentRect == null) return;

        // Размер родительского контейнера
        float parentWidth = parentRect.rect.width;
        float parentHeight = parentRect.rect.height;

        // Учитываем отступы
        float maxSize = Mathf.Min(parentWidth, parentHeight) - (2 * padding);
        maxSize = Mathf.Max(maxSize, 0); // Не даём значению уйти в отрицательное

        // Устанавливаем квадратный размер
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxSize);
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxSize);
    }
}