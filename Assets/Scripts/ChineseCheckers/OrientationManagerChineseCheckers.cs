using UnityEngine;

public class OrientationManagerChineseCheckers : MonoBehaviour
{
    [SerializeField] private GameObject finishTextPanel;
    [SerializeField] private GameObject finishSmallTextPanel;
    [SerializeField] private RectTransform finishPlayersPanel;
    
    [SerializeField] private RectTransform eduImagePage1;
    [SerializeField] private RectTransform eduTopTextPage1;
    
    [SerializeField] private RectTransform eduImagePage2Vert1;
    [SerializeField] private GameObject eduImagePage2Vert2;
    [SerializeField] private GameObject eduImagePage2Hor1;
    [SerializeField] private GameObject eduImagePage2Hor2;
    
    [SerializeField] private RectTransform eduTopTextPage3;
    [SerializeField] private GameObject eduColorTextTopPage3;
    [SerializeField] private GameObject eduColorTextLeftPage3;
    [SerializeField] private RectTransform eduColorImagePage3;
    
    [SerializeField] private GameObject speedButtonPhone;
    [SerializeField] private GameObject speedButtonTablet;
    [SerializeField] private GameObject speedButtonPhoneEdu;
    [SerializeField] private GameObject speedButtonTabletEdu;

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
        
        speedButtonPhone.SetActive(true);
        speedButtonPhoneEdu.SetActive(true);
        speedButtonTablet.SetActive(false);
        speedButtonTabletEdu.SetActive(false);
        
        RectTransform rt = finishPlayersPanel;
        Vector2 offsetMax = rt.offsetMax;
        offsetMax.y = -218f; // Top = 147, значит offsetMax.y = -147
        rt.offsetMax = offsetMax;

        eduImagePage1.offsetMin = new Vector2(eduImagePage1.offsetMin.x, 122);
        eduImagePage1.offsetMax = new Vector2(eduImagePage1.offsetMax.x, -228);
        
        eduTopTextPage1.offsetMin = new Vector2(35f, eduTopTextPage1.offsetMin.y);
        eduTopTextPage1.offsetMax = new Vector2(-35f, eduTopTextPage1.offsetMax.y);
        eduTopTextPage1.anchoredPosition = new Vector2(eduTopTextPage1.anchoredPosition.x, 30f);

        eduImagePage2Vert1.gameObject.SetActive(true);
        eduImagePage2Vert2.SetActive(true);
        eduImagePage2Hor1.SetActive(false);
        eduImagePage2Hor2.SetActive(false);

        eduImagePage2Vert1.anchoredPosition = new Vector2(eduImagePage2Vert1.anchoredPosition.x, -190f);
        
        eduTopTextPage3.anchoredPosition = new Vector2(eduTopTextPage3.anchoredPosition.x, -92f);
        eduColorTextTopPage3.SetActive(true);
        eduColorTextLeftPage3.SetActive(false);
        eduColorImagePage3.anchoredPosition = new Vector2(0f, eduColorImagePage3.anchoredPosition.y);
    }

    private void VerticalOrientationPhone()
    {
        finishTextPanel.SetActive(true);
        finishSmallTextPanel.SetActive(false);
        
        speedButtonPhone.SetActive(true);
        speedButtonPhoneEdu.SetActive(true);
        speedButtonTablet.SetActive(false);
        speedButtonTabletEdu.SetActive(false);
        
        RectTransform rt = finishPlayersPanel;
        Vector2 offsetMax = rt.offsetMax;
        offsetMax.y = -218f; // Top = 147, значит offsetMax.y = -147
        rt.offsetMax = offsetMax;
        
        eduImagePage1.offsetMin = new Vector2(eduImagePage1.offsetMin.x, 122);
        eduImagePage1.offsetMax = new Vector2(eduImagePage1.offsetMax.x, -228);
        
        eduTopTextPage1.offsetMin = new Vector2(5f, eduTopTextPage1.offsetMin.y);
        eduTopTextPage1.offsetMax = new Vector2(-5f, eduTopTextPage1.offsetMax.y);
        eduTopTextPage1.anchoredPosition = new Vector2(eduTopTextPage1.anchoredPosition.x, -30f);
        
        eduImagePage2Vert1.gameObject.SetActive(true);
        eduImagePage2Vert2.SetActive(true);
        eduImagePage2Hor1.SetActive(false);
        eduImagePage2Hor2.SetActive(false);
        
        eduImagePage2Vert1.anchoredPosition = new Vector2(eduImagePage2Vert1.anchoredPosition.x, -245f);
        
        eduTopTextPage3.anchoredPosition = new Vector2(eduTopTextPage3.anchoredPosition.x, -135f);
        eduColorTextTopPage3.SetActive(true);
        eduColorTextLeftPage3.SetActive(false);
        eduColorImagePage3.anchoredPosition = new Vector2(0f, eduColorImagePage3.anchoredPosition.y);
    }

    private void HorizontalOrientationTablet()
    {
        finishTextPanel.SetActive(false);
        finishSmallTextPanel.SetActive(true);
        
        speedButtonPhone.SetActive(false);
        speedButtonPhoneEdu.SetActive(false);
        speedButtonTablet.SetActive(true);
        speedButtonTabletEdu.SetActive(true);
        
        RectTransform rt = finishPlayersPanel;
        Vector2 offsetMax = rt.offsetMax;
        offsetMax.y = -147f; // Top = 147, значит offsetMax.y = -147
        rt.offsetMax = offsetMax;
        
        eduImagePage1.offsetMin = new Vector2(eduImagePage1.offsetMin.x, 64);
        eduImagePage1.offsetMax = new Vector2(eduImagePage1.offsetMax.x, -170); 
        
        eduTopTextPage1.offsetMin = new Vector2(35f, eduTopTextPage1.offsetMin.y);
        eduTopTextPage1.offsetMax = new Vector2(-35f, eduTopTextPage1.offsetMax.y);
        eduTopTextPage1.anchoredPosition = new Vector2(eduTopTextPage1.anchoredPosition.x, 30f);
        
        eduImagePage2Vert1.gameObject.SetActive(false);
        eduImagePage2Vert2.SetActive(false);
        eduImagePage2Hor1.SetActive(true);
        eduImagePage2Hor2.SetActive(true);
        
        eduImagePage2Vert1.anchoredPosition = new Vector2(eduImagePage2Vert1.anchoredPosition.x, -190f);
        
        eduTopTextPage3.anchoredPosition = new Vector2(eduTopTextPage3.anchoredPosition.x, -18f);
        eduColorTextTopPage3.SetActive(false);
        eduColorTextLeftPage3.SetActive(true);
        eduColorImagePage3.anchoredPosition = new Vector2(193f, eduColorImagePage3.anchoredPosition.y);
    }
}