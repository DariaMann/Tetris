﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Figures : MonoBehaviour
{


    float lastFall = 0f;

   public  int scores = 0;
    //public Transform figure;
    //void OnMouseDrag()
    //{
    //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    figure.position = new Vector2(mousePos.x, figure.position.y);
    //}
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-1, 0, 0);
            if (isValidGridPosition())
            {
                UpdateMatrixGrid();

            }
            else transform.position += new Vector3(1, 0, 0);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                transform.position += new Vector3(1, 0, 0);
                if (isValidGridPosition())
                {
                    UpdateMatrixGrid();

                }
                else transform.position += new Vector3(-1, 0, 0);
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    transform.Rotate(new Vector3(0, 0, - 90));
                    if (isValidGridPosition())
                    {
                        UpdateMatrixGrid();
                    }
                    else
                    {
                        transform.Rotate(new Vector3(0, 0, -90));
                    }
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - lastFall >= 1)
                {
                    transform.position += new Vector3(0,-1, 0);
                    if (isValidGridPosition())
                    {
                        UpdateMatrixGrid();
                    }
                    else
                    {
                        transform.position += new Vector3(0, 1, 0);
                        MatrixGrid.DeleteWholeRows();
                        Debug.Log("Удаление");
                        scores= MatrixGrid.Records();
                        FindObjectOfType<Move>().Records(scores);
                        FindObjectOfType<Move>().AppearanceRandome();
                        //Debug.Log("1");
                        enabled = false;
                    }
                    Debug.Log("Баллы "+ scores);
                    lastFall = Time.time;
                }
            }

        }

    }
     bool isValidGridPosition()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = MatrixGrid.RoundVector(child.position);
            if (!MatrixGrid.IsInsideBorder(v))
            {
                //Debug.Log("3");
                return false;//косание стен
            }
            if (MatrixGrid.grid[(int)v.x, (int)v.y] != null && MatrixGrid.grid[(int)v.x, (int)v.y].parent != transform)
            {
                //Debug.Log("4");
                return false;//косание другой фигуры

            }
        }
        return true;
    }
    void UpdateMatrixGrid()
    {
        for (int y = 0; y < MatrixGrid.column; ++y)
        {
            for (int x = 0; x < MatrixGrid.row; ++x)
            {
                if (MatrixGrid.grid[x, y] != null)
                {
                    if (MatrixGrid.grid[x, y].parent == transform)
                    {
                        MatrixGrid.grid[x, y] = null;
                    }
                }
            }
        }
        foreach (Transform child in transform)
        {
            Vector2 v = MatrixGrid.RoundVector(child.position);
            MatrixGrid.grid[(int)v.x, (int)v.y] = child;
        }
    }

}