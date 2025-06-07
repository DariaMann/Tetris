using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationManagerTetris : MonoBehaviour
{
    [SerializeField] private Transform board;
    [SerializeField] private Transform goast;
    [SerializeField] private Transform grid;
    
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject upPanel;
    [SerializeField] private CameraRenderer cameraRenderer;
    
    [SerializeField] private Transform boardEdu;
    [SerializeField] private Transform goastEdu;
    [SerializeField] private GameObject bottomTextEdu;
    
    void Update()
    {
        bool isTablet = GameHelper.IsTablet();
        if (Screen.width > Screen.height) // Горизонтальная ориентация
        {
            HorizontalOrientationTablet();
            
//            panel.anchoredPosition = landscapePosition;
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
//            panel.anchoredPosition = portraitPosition;
        }
    }
    
    private void VerticalOrientationTablet()
    {
        boardEdu.localPosition = new Vector2(2.23f,0);
        goastEdu.localPosition = new Vector2(2.23f,0);
        
        cameraRenderer.vertical = 0.5f;
        
        board.localPosition = new Vector2(2.23f,0);
        goast.localPosition = new Vector2(2.23f,0);
        grid.localPosition = new Vector2(2.23f,0);
        
        bottomTextEdu.SetActive(false);

        if (GameHelper.IsEdication && !GameHelper.IsUIEdication)
        {
            leftPanel.SetActive(false);
            rightPanel.SetActive(false);
            upPanel.SetActive(false);
        }
        else
        {
            leftPanel.SetActive(true);
            rightPanel.SetActive(false);
            upPanel.SetActive(false);
        }
    }
    
    private void VerticalOrientationPhone()
    {
        boardEdu.localPosition = new Vector2(0f,0f);
        goastEdu.localPosition = new Vector2(0f,0f);

        if (GameHelper.HaveAds)
        {
            cameraRenderer.vertical = GameHelper.IsEdication ? 0.25f : 0.5f;
            cameraRenderer.WidthOrHeight = GameHelper.IsEdication ? 0.25f : 0.4f;
            board.localPosition = new Vector2(0f,-0.5f);
            goast.localPosition = new Vector2(0f,-0.5f);
            grid.localPosition = (GameHelper.IsEdication && !GameHelper.IsUIEdication) ? new Vector2(0f,0f) : new Vector2(0f,-0.5f);
        }
        else
        {
            cameraRenderer.vertical = GameHelper.IsEdication ? 0.25f : 0.5f;
            cameraRenderer.WidthOrHeight = GameHelper.IsEdication ? 0.25f : 0.5f;
            board.localPosition = new Vector2(0f,-1f);
            goast.localPosition = new Vector2(0f,-1f);
            grid.localPosition = (GameHelper.IsEdication && !GameHelper.IsUIEdication) ? new Vector2(0f,0f) : new Vector2(0f,-1f);
        }

        if (GameHelper.IsEdication && !GameHelper.IsUIEdication)
        {
            bottomTextEdu.SetActive(true);
            leftPanel.SetActive(false);
            rightPanel.SetActive(false);
            upPanel.SetActive(false);
        }
        else
        {
            bottomTextEdu.SetActive(false);
            leftPanel.SetActive(false);
            rightPanel.SetActive(false);
            upPanel.SetActive(true);
        }
    }

    private void HorizontalOrientationTablet()
    {
        boardEdu.localPosition = new Vector2(0,0);
        goastEdu.localPosition = new Vector2(0,0);
        
        cameraRenderer.vertical = 0.5f;
        
        board.localPosition = new Vector2(0,0);
        goast.localPosition = new Vector2(0,0);
        grid.localPosition = new Vector2(0,0);
        
        bottomTextEdu.SetActive(false);

        if (GameHelper.IsEdication && !GameHelper.IsUIEdication)
        {
            leftPanel.SetActive(false);
            rightPanel.SetActive(false);
            upPanel.SetActive(false);
        }
        else
        {
            leftPanel.SetActive(false);
            rightPanel.SetActive(true);
            upPanel.SetActive(false);
        }
    }

}
