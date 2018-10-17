using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixGrid {
    public static int row = 8;//
    public static int column = 20;//
    public static int scores = 0;
    public static Transform[,] grid = new Transform[row, column];//

    public static Vector2 RoundVector(Vector2 v)//
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));//
    }
    public static bool IsInsideBorder(Vector2 pos)
    {
       // Debug.Log("xxxxxxxxxxxxxx "+(int)pos.x);
       // Debug.Log("Roooooooow "+ column);
        //Debug.Log("yyyyyyyyyyyyyyy "+(int)pos.y);
        return ((int)pos.x >= 2 && (int)pos.x <row && (int)pos.y >=3);

    }
    public static void DeleteRow(int y)//
    {
        for (int x = 2; x<row; ++x)
        {
            GameObject.Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
        scores++;
    }
    public static void DecreaseRow(int y)//
    {
        for(int x = 2;x<row;++x)
        {
            if (grid[x,y]!=null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }
    public static void DecreaseRowAbove(int y)//
    {
        for (int i = y; i<column;++i)
        {
            DecreaseRow(i);
        }
    }
    public static bool IsRowFull(int y)//
    {
        for (int x = 2; x < row; ++x)
        {
            if (grid[x,y] == null)
            {
                //Debug.Log("false");
                //Debug.Log("X"+x);
                //Debug.Log("Y" + y);
                return false;
                
            }
            
        }
        //Debug.Log("true");
        return true;
        
    }
    public static void DeleteWholeRows()//
    {
        for (int y = 0; y< column ; ++y)
        {
            if (IsRowFull(y))
            {
               // Debug.Log("YYYYYYYYYYYYYYYYYYYYYYYYY  " + y);
                DeleteRow(y);
                DecreaseRowAbove(y + 1);

                --y;
            }
            //Debug.Log("YYYYYYYYYYYYYYYYYYYYYYYYY  " + y);
        }
       // Debug.Log("Работает");
    }
    public static int Records()
    {
        return scores;
    }
}
