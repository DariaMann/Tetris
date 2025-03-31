using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SquareUI : MonoBehaviour
{
    private RectTransform rectTransform;

    public float padding = 50f; // Отступ от краёв экрана

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        ResizeSquare();
    }

    void Update()
    {
        ResizeSquare(); // Подстраиваем каждый кадр, если изменяется размер экрана
    }

    void ResizeSquare()
    {
        RectTransform parentRect = rectTransform.parent.GetComponent<RectTransform>();
        if (parentRect == null) return;

        // Размер родительского контейнера
        float parentWidth = parentRect.rect.width;
        float parentHeight = parentRect.rect.height;

        // Учитываем отступы
        float maxSize = Mathf.Min(parentWidth, parentHeight) - (2 * padding);
        maxSize = Mathf.Max(maxSize, 0); // Не даём значению уйти в отрицательное

        // Устанавливаем квадратный размер
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxSize);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxSize);
    }
}