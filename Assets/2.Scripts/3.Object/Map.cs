using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Point
{
    public int x;
    public int y;
    public Point(int y, int x)
    {
        this.y = y;
        this.x = x;
    }
    public static Point operator +(Point a, Point b)
    {
        return new Point(a.y + b.y, a.x + b.x);
    }
    public override string ToString()
    {
        return y.ToString() + ", " + x.ToString();
    }

}

public class Map : MonoBehaviour
{
    public int mapSize;
    public TileNode[] grid;

    public List<Point> startPos;

    private void Awake()
    {
        grid = GetComponentsInChildren<TileNode>();
        startPos = new List<Point>();
        for(int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                grid[mapSize * i + j].tilePos = new Point(i, j);

            }
        }
        SetStartPos();
    }
    public void SetStartPos()
    {
        startPos.Add(new Point(0, 0));
        startPos.Add(new Point(0, mapSize - 1));
        startPos.Add(new Point(mapSize - 1, 0));
        startPos.Add(new Point(mapSize-1, mapSize-1));
    }

    public TileNode GetTileNode(Point pos)
    {
        return grid[mapSize * pos.y + pos.x];
    }
}