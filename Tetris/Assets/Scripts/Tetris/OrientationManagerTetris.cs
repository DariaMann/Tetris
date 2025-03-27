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
    
    
    void Update()
    {
        bool isTablet = IsTablet();
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
        board.localPosition = new Vector2(2.23f,0);
        goast.localPosition = new Vector2(2.23f,0);
        grid.localPosition = new Vector2(2.23f,0);
        leftPanel.SetActive(true);
        rightPanel.SetActive(false);
        upPanel.SetActive(false);
    }
    
    private void VerticalOrientationPhone()
    {
        board.localPosition = new Vector2(0f,-1f);
        goast.localPosition = new Vector2(0f,-1f);
        grid.localPosition = new Vector2(0f,-1f);
        leftPanel.SetActive(false);
        rightPanel.SetActive(false);
        upPanel.SetActive(true);
    }

    private void HorizontalOrientationTablet()
    {
        board.localPosition = new Vector2(0,0);
        goast.localPosition = new Vector2(0,0);
        grid.localPosition = new Vector2(0,0);
        leftPanel.SetActive(false);
        rightPanel.SetActive(true);
        upPanel.SetActive(false);
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
