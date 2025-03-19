using UnityEngine;

public class UIOrientationManager : MonoBehaviour
{
    public RectTransform panel; // UI-панель, которую хотим двигать
    public Vector2 portraitPosition; // Координаты для вертикальной ориентации
    public Vector2 landscapePosition; // Координаты для горизонтальной ориентации

    private void Start()
    {
        UpdateUIPosition();
    }

    private void Update()
    {
        UpdateUIPosition();
    }

    void UpdateUIPosition()
    {
        if (Screen.width > Screen.height) // Горизонтальная ориентация
        {
            panel.anchoredPosition = landscapePosition;
        }
        else // Вертикальная ориентация
        {
            panel.anchoredPosition = portraitPosition;
        }
//        Debug.Log(panel.anchoredPosition);
    }
}