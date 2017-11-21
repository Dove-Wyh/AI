using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    public GameObject obstacleCube;
    public GameObject startCube;
    public GameObject endCube;
    public GameObject checkCube;
    public GameObject pathCube;


    public static int width = 10;
    public static int height = 10;

    private Point[,] map = new Point[width, height];

    void Start()
    {
        InitMap();
        Point start = null;
        Point end = null;
        foreach (var point in map)
        {
            if (point.sign == 2)
            {
                start = point;
            }
            if (point.sign == 3)
            {
                end = point;
            }
        }
        StartCoroutine(FindPath(start, end));

        //StartCoroutine(ShowPath(end));
    }

    private void InitMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = new Point(x, y);
            }
        }
        //障碍物的标志位1,起点标志为2,终点标志为3
        map[3, 2].sign = 1;
        map[3, 3].sign = 1;
        map[3, 4].sign = 1;
        map[3, 5].sign = 1;
        map[4, 5].sign = 1;
        map[5, 5].sign = 1;
        map[6, 5].sign = 1;
        map[7, 5].sign = 1;
        map[7, 4].sign = 1;
        map[7, 3].sign = 1;
        map[7, 2].sign = 1;
        map[5, 3].sign = 2;
        map[5, 6].sign = 3;
        map[4, 6].sign = 1;
        map[4, 7].sign = 1;
        map[6, 6].sign = 1;
        map[6, 7].sign = 1;
        //在unity中显示起点与终点和障碍物
        foreach (Point point in map)
        {
            switch (point.sign)
            {
                case 1:
                    GameObject.Instantiate(obstacleCube, new Vector3(point.x, 0, point.y), Quaternion.identity);
                    break;
                case 2:
                    GameObject.Instantiate(startCube, new Vector3(point.x, 0, point.y), Quaternion.identity);
                    break;
                case 3:
                    GameObject.Instantiate(endCube, new Vector3(point.x, 0, point.y), Quaternion.identity);
                    break;
            }
        }
    }

    IEnumerator FindPath(Point start, Point end)
    {
        List<Point> openList = new List<Point>();
        List<Point> closeList = new List<Point>();
        openList.Add(start);
        start.checkSign = 1;
        while (openList.Count > 0)
        {
            Point point = FindMinF(openList);
            openList.Remove(point);
            closeList.Add(point);
            point.checkSign = 2;//checkSign默认是0,在openList中是1,在closeList中是2
            //在unity中表示
            if (point.sign != 2 && point.sign != 3 && point.isDraw == false)//如果不是起点或者终点
            {
                GameObject.Instantiate(checkCube, new Vector3(point.x, -0.45f, point.y), Quaternion.identity);
                point.isDraw = true;
                yield return new WaitForSeconds(0.05f);
            }
            List<Point> surroundPoints = GetSurroundPoints(point);//不存在已经在关闭列表中的结点
            foreach (Point surroundPoint in surroundPoints)
            {
                if (surroundPoint.checkSign == 1)//如果周围的点已经在开启列表中,需要根据F值判断是否需要更新父亲
                {
                    float currentG = CaclG(point);
                    if (currentG < surroundPoint.G)//更新父节点
                    {
                        surroundPoint.parent = point;
                        surroundPoint.G = currentG;
                        surroundPoint.F = surroundPoint.G + surroundPoint.H;
                    }
                }
                else//没有父节点
                {
                    surroundPoint.parent = point;
                    CaclF(surroundPoint, end);
                    openList.Add(surroundPoint);
                }
            }
            //判断终点是否在openList中,如果在代表已经找到了
            if (openList.IndexOf(end) > -1)
            {
                break;
            }
        }
        StartCoroutine(ShowPath(end));
    }

    IEnumerator ShowPath(Point end)
    {
        Point temp = end;
        while (true)
        {
            if (temp.parent == null)
            {
                break;
            }
            //在unity中表示
            if (temp.sign != 2 && temp.sign != 3)//如果不是起点或者终点
            {
                //new WaitForSeconds(0.05f);
                GameObject.Instantiate(pathCube, new Vector3(temp.x, -0.4f, temp.y), Quaternion.identity);
                yield return new WaitForSeconds(0.05f);
            }
            temp = temp.parent;
        }
        yield return null;
    }

    private List<Point> GetSurroundPoints(Point point)
    {
        List<Point> surroundPoints = new List<Point>();
        Point right = null;
        Point down = null;
        Point left = null;
        Point up = null;
        if (point.y + 1 < width)
        {
            right = map[point.x, point.y + 1];
            if (right.sign != 1 && right.checkSign != 2)//不是障碍物并且不在关闭列表中
            {
                surroundPoints.Add(right);
            }
        }
        if (point.x + 1 < height)
        {
            down = map[point.x + 1, point.y];
            if (down.sign != 1 && down.checkSign != 2)//不是障碍物并且不在关闭列表中
            {
                surroundPoints.Add(down);
            }
        }
        if (point.y - 1 > -1)
        {
            left = map[point.x, point.y - 1];
            if (left.sign != 1 && left.checkSign != 2)//不是障碍物并且不在关闭列表中
            {
                surroundPoints.Add(left);
            }
        }
        if (point.x - 1 > -1)
        {
            up = map[point.x - 1, point.y];
            if (up.sign != 1 && up.checkSign != 2)//不是障碍物并且不在关闭列表中
            {
                surroundPoints.Add(up);
            }
        }
        return surroundPoints;
    }

    private Point FindMinF(List<Point> openList)
    {
        float f = float.MaxValue;
        Point temp = null;
        foreach (Point point in openList)
        {
            if (point.F < f)
            {
                temp = point;
                f = temp.F;
            }
        }
        return temp;
    }

    private void CaclF(Point current, Point end)
    {
        //F = G + H;
        float h = Mathf.Abs(current.x - end.x) + Mathf.Abs(current.y - end.y);
        float g;
        if (current.parent == null)//代表起点,只有起点没有父节点 
        {
            g = 0;
        }
        else
        {
            g = current.parent.G + 1;
        }
        float f = g + h;
        current.H = h;
        current.G = g;
        current.F = f;
    }

    private float CaclG(Point parent)
    {
        return parent.G + 1;
    }
}
