using UnityEngine;

public class OrientationManager2048 : MonoBehaviour
{
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private GameObject topPanel;
    
    [SerializeField] private SquareUI board;
    [SerializeField] private RectTransform boardRect;
    
    [SerializeField] private GameObject eduText;
    [SerializeField] private GameObject eduBoardText;
    [SerializeField] private SquareUI eduBoard;
    [SerializeField] private RectTransform eduBoardRect;
    
    [SerializeField] private RectTransform undoButtonPhone;
    [SerializeField] private GameObject undoButtonTablet;
    [SerializeField] private GameObject undoButtonPhoneEdu;
    [SerializeField] private GameObject undoButtonTabletEdu;
    
    void Update()
    {
        if (GameHelper.IsDoScreenshot)
        {
            return;
        }
        
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
    
    public void SetCorrectUI(int height, int width, bool isTablet, bool isVertical)
    {
        Debug.Log(width + "x" + height+": isVertical = "+ isVertical+", isTablet = "+isTablet);
        
        if (!isVertical)
        {
            HorizontalOrientationTablet();
        }
        else
        {
            if (isTablet)
            {
                VerticalOrientationTablet();
            }
            else
            {
                VerticalOrientationPhone();
            }
        }
    }
    
    private void VerticalOrientationTablet()
    {
        rightPanel.SetActive(false);
        topPanel.SetActive(true);
        
        if (GameHelper.HaveAds)
        {
            undoButtonPhone.anchoredPosition = new Vector2(undoButtonPhone.anchoredPosition.x, -80f);
            board.Padding = 54;
        }
        else
        {
            undoButtonPhone.anchoredPosition = new Vector2(undoButtonPhone.anchoredPosition.x, -95f);
            board.Padding = 35;
        }
        
        Vector2 pos1 = boardRect.anchoredPosition;
        pos1.y = 0; // любое нужное значение
        boardRect.anchoredPosition = pos1;

        undoButtonPhone.gameObject.SetActive(true);
        undoButtonPhoneEdu.SetActive(true);
        undoButtonTablet.SetActive(false);
        undoButtonTabletEdu.SetActive(false);
        
        eduText.SetActive(true);
        eduBoardText.SetActive(false);
        eduBoard.Padding = 65;
        Vector2 pos = eduBoardRect.anchoredPosition;
        pos.y = 0; // любое нужное значение
        eduBoardRect.anchoredPosition = pos;
    }
    
    private void VerticalOrientationPhone()
    {
        rightPanel.SetActive(false);
        topPanel.SetActive(true);
        
        board.Padding = 35;
        Vector2 pos1 = boardRect.anchoredPosition;
        pos1.y = 0; // любое нужное значение
        boardRect.anchoredPosition = pos1;
        
        undoButtonPhone.gameObject.SetActive(true);
        undoButtonPhoneEdu.SetActive(true);
        undoButtonTablet.SetActive(false);
        undoButtonTabletEdu.SetActive(false);
        undoButtonPhone.anchoredPosition = new Vector2(undoButtonPhone.anchoredPosition.x, -95f);
        
        eduText.SetActive(false);
        eduBoardText.SetActive(true);
        eduBoard.Padding = 35;
        Vector2 pos = eduBoardRect.anchoredPosition;
        pos.y = 50f; // любое нужное значение
        eduBoardRect.anchoredPosition = pos;
    }

    private void HorizontalOrientationTablet()
    {
        rightPanel.SetActive(true);
        topPanel.SetActive(false);
        
        if (GameHelper.HaveAds)
        {
            board.Padding = 95;
            Vector2 pos1 = boardRect.anchoredPosition;
            pos1.y = 18; // любое нужное значение
            boardRect.anchoredPosition = pos1;
        }
        else
        {
            board.Padding = 35;
            Vector2 pos2 = boardRect.anchoredPosition;
            pos2.y = 0; // любое нужное значение
            boardRect.anchoredPosition = pos2;
        }
        
        undoButtonPhone.gameObject.SetActive(false);
        undoButtonPhoneEdu.SetActive(false);
        undoButtonTablet.SetActive(true);
        undoButtonTabletEdu.SetActive(true);
        undoButtonPhone.anchoredPosition = new Vector2(undoButtonPhone.anchoredPosition.x, -95f);
        
        eduText.SetActive(true);
        eduBoardText.SetActive(false);
        eduBoard.Padding = 90;
        Vector2 pos = eduBoardRect.anchoredPosition;
        pos.y = 50f; // любое нужное значение
        eduBoardRect.anchoredPosition = pos;
    }
}