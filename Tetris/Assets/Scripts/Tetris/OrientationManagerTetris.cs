using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationManagerTetris : MonoBehaviour
{
    [SerializeField] private Transform board;
    [SerializeField] private Transform goast;
    [SerializeField] private Transform grid;
    
    
    void Update()
    {
        if (Screen.width > Screen.height) // Горизонтальная ориентация
        {
            HorizontalOrientation();
//            panel.anchoredPosition = landscapePosition;
        }
        else // Вертикальная ориентация
        {
            VerticalOrientation();
//            panel.anchoredPosition = portraitPosition;
        }
    }
    
    private void VerticalOrientation()
    {
        board.localPosition = new Vector2(2.23f,0);
        goast.localPosition = new Vector2(2.23f,0);
        grid.localPosition = new Vector2(2.23f,0);
    }

    private void HorizontalOrientation()
    {
        board.localPosition = new Vector2(0,0);
        goast.localPosition = new Vector2(0,0);
        grid.localPosition = new Vector2(0,0);
    }
}
