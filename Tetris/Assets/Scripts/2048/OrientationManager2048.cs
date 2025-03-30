using UnityEngine;

public class OrientationManager2048 : MonoBehaviour
{
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private GameObject topPanel;
    
    void Update()
    {
        bool isTablet = IsTablet();
        if (Screen.width > Screen.height) // Горизонтальная ориентация
        {
            HorizontalOrientationTablet();
        }
        else // Вертикальная ориентация
        {
            if (!isTablet)
            {
                VerticalOrientationPhone();
            }
            else
            {
                VerticalOrientationTablet();
            }
        }
    }
    
    private void VerticalOrientationTablet()
    {
        rightPanel.SetActive(false);
        topPanel.SetActive(true);
    }
    
    private void VerticalOrientationPhone()
    {
        rightPanel.SetActive(false);
        topPanel.SetActive(true);
    }

    private void HorizontalOrientationTablet()
    {
        rightPanel.SetActive(true);
        topPanel.SetActive(false);
    }
    
    private bool IsTablet()
    {
        float dpi = Screen.dpi;
        float width = Screen.width / dpi;
        float height = Screen.height / dpi;
        float diagonalInInches = Mathf.Sqrt(width * width + height * height);

        return diagonalInInches >= 6.5f; // Обычно планшеты > 6.5 дюймов
    }
}