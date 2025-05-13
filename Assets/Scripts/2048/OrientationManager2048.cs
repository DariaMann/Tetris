using UnityEngine;

public class OrientationManager2048 : MonoBehaviour
{
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private GameObject topPanel;
    
    [SerializeField] private GameObject eduText;
    [SerializeField] private GameObject eduBoardText;
    [SerializeField] private SquareUI board;
    [SerializeField] private RectTransform boardRect;
    
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
        rightPanel.SetActive(false);
        topPanel.SetActive(true);
        
        eduText.SetActive(true);
        eduBoardText.SetActive(false);
        board.Padding = 65;
        Vector2 pos = boardRect.anchoredPosition;
        pos.y = 0; // любое нужное значение
        boardRect.anchoredPosition = pos;
    }
    
    private void VerticalOrientationPhone()
    {
        rightPanel.SetActive(false);
        topPanel.SetActive(true);
        
        eduText.SetActive(false);
        eduBoardText.SetActive(true);
        board.Padding = 35;
        Vector2 pos = boardRect.anchoredPosition;
        pos.y = 50f; // любое нужное значение
        boardRect.anchoredPosition = pos;
    }

    private void HorizontalOrientationTablet()
    {
        rightPanel.SetActive(true);
        topPanel.SetActive(false);
        
        eduText.SetActive(true);
        eduBoardText.SetActive(false);
        board.Padding = 90;
        Vector2 pos = boardRect.anchoredPosition;
        pos.y = 50f; // любое нужное значение
        boardRect.anchoredPosition = pos;
    }
}