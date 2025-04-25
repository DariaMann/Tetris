using UnityEngine;

public class OrientationManagerBlocks : MonoBehaviour
{
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private GameObject topPanel;
    
    [SerializeField] private Block leftBlock;
    [SerializeField] private Block rightBlock;
    
    [SerializeField] private RectTransform boardPanel;
    [SerializeField] private RectTransform blocksPanel;

    [SerializeField] private SquareUIGrid squareUiGrid;
    [SerializeField] private BlocksBoard blocksBoard;

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
        
        boardPanel.anchoredPosition = new Vector2(boardPanel.anchoredPosition.x, 35);
        blocksPanel.anchoredPosition = new Vector2(blocksPanel.anchoredPosition.x, 58);
        
        blocksBoard.RescaleBlocks(0.7f);

        leftBlock.RepositionX(100);
        rightBlock.RepositionX(-100);
        
        squareUiGrid.Padding = 80;
        squareUiGrid.ResizeSquare();
        blocksBoard.ResizeBlocks();
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
        
        squareUiGrid.Padding = 35;
        squareUiGrid.ResizeSquare();
        blocksBoard.ResizeBlocks();
    }

    private void HorizontalOrientationTablet()
    {
        rightPanel.SetActive(true);
        topPanel.SetActive(false);
        
        boardPanel.anchoredPosition = new Vector2(boardPanel.anchoredPosition.x, 55);
        blocksPanel.anchoredPosition = new Vector2(blocksPanel.anchoredPosition.x, 32);

        blocksBoard.RescaleBlocks(0.7f);
        
        leftBlock.RepositionX(150);
        rightBlock.RepositionX(-150);

        squareUiGrid.Padding = 110;
        squareUiGrid.ResizeSquare();
        blocksBoard.ResizeBlocks();
    }
}