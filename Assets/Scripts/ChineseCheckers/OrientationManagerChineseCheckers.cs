using UnityEngine;

public class OrientationManagerChineseCheckers : MonoBehaviour
{
    [SerializeField] private GameObject finishTextPanel;
    [SerializeField] private GameObject finishSmallTextPanel;
    [SerializeField] private RectTransform finishPlayersPanel;

    void Update()
    {
        bool isTablet = GameHelper.IsTablet();
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
        finishTextPanel.SetActive(true);
        finishSmallTextPanel.SetActive(false);
        
        RectTransform rt = finishPlayersPanel;
        Vector2 offsetMax = rt.offsetMax;
        offsetMax.y = -218f; // Top = 147, значит offsetMax.y = -147
        rt.offsetMax = offsetMax;
    }

    private void VerticalOrientationPhone()
    {
        finishTextPanel.SetActive(true);
        finishSmallTextPanel.SetActive(false);
        
        RectTransform rt = finishPlayersPanel;
        Vector2 offsetMax = rt.offsetMax;
        offsetMax.y = -218f; // Top = 147, значит offsetMax.y = -147
        rt.offsetMax = offsetMax;
    }

    private void HorizontalOrientationTablet()
    {
        finishTextPanel.SetActive(false);
        finishSmallTextPanel.SetActive(true);
        
        RectTransform rt = finishPlayersPanel;
        Vector2 offsetMax = rt.offsetMax;
        offsetMax.y = -147f; // Top = 147, значит offsetMax.y = -147
        rt.offsetMax = offsetMax;
    }
}