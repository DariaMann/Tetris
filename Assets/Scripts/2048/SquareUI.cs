using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SquareUI : MonoBehaviour
{
    private RectTransform _rectTransform;

    public float padding = 50f; // Отступ от краёв экрана

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
        
        ResizeSquare(); // Подстраиваем каждый кадр, если изменяется размер экрана
    }

    public void ResizeSquare()
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
    
    public void ResizeSquare(int targetWidth, int targetHeight)
    {
        // Берём минимальную сторону и отнимаем padding (в пикселях таргета)
        float maxSize = Mathf.Min(targetWidth, targetHeight) - (2 * padding);
        maxSize = Mathf.Max(maxSize, 0);

        // Если у тебя есть Canvas с CanvasScaler, нужно учесть его scaleFactor,
        // потому что RectTransform работает в единицах канвы, а targetWidth/Height — в пикселях.
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        float scale = 1f;
        if (parentCanvas != null)
        {
            scale = parentCanvas.scaleFactor;
        }

        // Преобразуем пиксели в канва-юниты
        float sizeInCanvasUnits = maxSize / scale;

        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeInCanvasUnits);
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeInCanvasUnits);
    }

}