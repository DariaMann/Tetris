using UnityEngine;

public class OrientationManagerLines98 : MonoBehaviour
{
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private GameObject topPanel;
    [SerializeField] private SquareUIGrid board;
    [SerializeField] private EducationLines98 education;
    
    [SerializeField] private GameObject undoButtonPhone;
    [SerializeField] private GameObject undoButtonTablet;
    [SerializeField] private GameObject undoButtonPhoneEdu;
    [SerializeField] private GameObject undoButtonTabletEdu;
    
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
        board.Padding = 35;
        education.SetOrientation(true);
        
        undoButtonPhone.SetActive(true);
        undoButtonPhoneEdu.SetActive(true);
        undoButtonTablet.SetActive(false);
        undoButtonTabletEdu.SetActive(false);
    }
    
    private void VerticalOrientationPhone()
    {
        rightPanel.SetActive(false);
        topPanel.SetActive(true);
        board.Padding = 35;
        education.SetOrientation(true);
        
        undoButtonPhone.SetActive(true);
        undoButtonPhoneEdu.SetActive(true);
        undoButtonTablet.SetActive(false);
        undoButtonTabletEdu.SetActive(false);
    }

    private void HorizontalOrientationTablet()
    {
        rightPanel.SetActive(true);
        topPanel.SetActive(false);
        board.Padding = GameHelper.HaveAds ? 75 : 35;
        education.SetOrientation(false);
        
        undoButtonPhone.SetActive(false);
        undoButtonPhoneEdu.SetActive(false);
        undoButtonTablet.SetActive(true);
        undoButtonTabletEdu.SetActive(true);
    }
}