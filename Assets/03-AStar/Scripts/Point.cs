using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public Point parent;
    public float F;
    public float G;
    public float H;
    public int x;
    public int y;
    public int sign;
    public int checkSign;
    public bool isDraw;

    public Point(int x, int y, Point parent = null)
    {
        this.x = x;
        this.y = y;
        this.parent = parent;
        sign = 0;
        checkSign = 0;
        isDraw = false;
    }
}