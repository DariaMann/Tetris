using UnityEngine;

public class OrientationManagerBlocks : MonoBehaviour
{
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private GameObject topPanel;
    
    [SerializeField] private Block leftBlock;
    [SerializeField] private Block centerBlock;
    [SerializeField] private Block rightBlock;
    
    [SerializeField] private RectTransform boardPanel;
    [SerializeField] private RectTransform blocksPanel;

    [SerializeField] private SquareUIGrid squareUiGrid;
    [SerializeField] private BlocksBoard blocksBoard;
    
    
    
    [SerializeField] private GameObject panelEdu;
    [SerializeField] private RectTransform boardPanelEdu;
    [SerializeField] private RectTransform blocksPanelEdu;
    
    [SerializeField] private Block centerBlockEdu;

    [SerializeField] private SquareUIGrid squareUiGridEdu;
    [SerializeField] private BlocksBoard blocksBoardEdu;

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
        
        boardPanel.anchoredPosition = new Vector2(boardPanel.anchoredPosition.x, 35);

        if (GameHelper.HaveAds)
        {
            blocksPanel.anchoredPosition = new Vector2(blocksPanel.anchoredPosition.x, 80);
            blocksBoard.RescaleBlocks(0.5f);
        }
        else
        {
            blocksPanel.anchoredPosition = new Vector2(blocksPanel.anchoredPosition.x, 58);
            blocksBoard.RescaleBlocks(0.7f);
        }

        leftBlock.RepositionX(100);
        rightBlock.RepositionX(-100);
        
        leftBlock.RepositionOffsetY(450);
        centerBlock.RepositionOffsetY(450);
        rightBlock.RepositionOffsetY(450);

        squareUiGrid.Padding = 80;
        squareUiGrid.ResizeSquare();
        blocksBoard.ResizeBlocks();

        if (panelEdu.activeSelf)
        {
            boardPanelEdu.anchoredPosition = new Vector2(boardPanelEdu.anchoredPosition.x, 0);
            blocksPanelEdu.anchoredPosition = new Vector2(blocksPanelEdu.anchoredPosition.x, 58);

            blocksBoardEdu.RescaleBlocks(0.7f);
            
            centerBlockEdu.RepositionOffsetY(450);

            squareUiGridEdu.Padding = 80;
            squareUiGridEdu.ResizeSquare();
            blocksBoardEdu.ResizeBlocks();
        }
    }
    
    private void VerticalOrientationPhone()
    {
        rightPanel.SetActive(false);
        topPanel.SetActive(true);
        
        boardPanel.anchoredPosition = new Vector2(boardPanel.anchoredPosition.x, 61);
        blocksPanel.anchoredPosition = new Vector2(blocksPanel.anchoredPosition.x, 112);

        blocksBoard.RescaleBlocks(0.6f);
        
        leftBlock.RepositionX(90);
        rightBlock.RepositionX(-90);
        
        leftBlock.RepositionOffsetY(500);
        centerBlock.RepositionOffsetY(500);
        rightBlock.RepositionOffsetY(500);
        
        squareUiGrid.Padding = 35;
        squareUiGrid.ResizeSquare();
        blocksBoard.ResizeBlocks();

        if (panelEdu.activeSelf)
        {
            boardPanelEdu.anchoredPosition = new Vector2(boardPanelEdu.anchoredPosition.x, 24);
            blocksPanelEdu.anchoredPosition = new Vector2(blocksPanelEdu.anchoredPosition.x, 112);

            blocksBoardEdu.RescaleBlocks(0.6f);
            
            centerBlockEdu.RepositionOffsetY(500);

            squareUiGridEdu.Padding = 35;
            squareUiGridEdu.ResizeSquare();
            blocksBoardEdu.ResizeBlocks();
        }
    }

    private void HorizontalOrientationTablet()
    {
        rightPanel.SetActive(true);
        topPanel.SetActive(false);
        
        boardPanel.anchoredPosition = new Vector2(boardPanel.anchoredPosition.x, 55);

        if (GameHelper.HaveAds)
        {
            blocksPanel.anchoredPosition = new Vector2(blocksPanel.anchoredPosition.x, 75);
            blocksBoard.RescaleBlocks(0.5f);
            squareUiGrid.Padding = 130;
        }
        else
        {
            blocksPanel.anchoredPosition = new Vector2(blocksPanel.anchoredPosition.x, 32);
            blocksBoard.RescaleBlocks(0.7f);
            squareUiGrid.Padding = 110;
        }

        leftBlock.RepositionX(150);
        rightBlock.RepositionX(-150);
        
        leftBlock.RepositionOffsetY(370);
        centerBlock.RepositionOffsetY(370);
        rightBlock.RepositionOffsetY(370);
        
        squareUiGrid.ResizeSquare();
        blocksBoard.ResizeBlocks();

        if (panelEdu.activeSelf)
        {
            boardPanelEdu.anchoredPosition = new Vector2(boardPanelEdu.anchoredPosition.x, 0);
            blocksPanelEdu.anchoredPosition = new Vector2(blocksPanelEdu.anchoredPosition.x, 17);

            blocksBoardEdu.RescaleBlocks(0.7f);
            
            centerBlockEdu.RepositionOffsetY(370);

            squareUiGridEdu.Padding = 110;
            squareUiGridEdu.ResizeSquare();
            blocksBoardEdu.ResizeBlocks();
        }
    }
}