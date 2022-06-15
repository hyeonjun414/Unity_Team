using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public int mapSize;
    public TileNode[] grid;

    public List<Vector2> startPos;

    private void Awake()
    {
        grid = GetComponentsInChildren<TileNode>();
        startPos = new List<Vector2>();
        for(int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                grid[mapSize * i + j].posX = j;
                grid[mapSize * i + j].posY = i;

            }
        }
        SetStartPos();
    }
    public void SetStartPos()
    {
        startPos.Add(new Vector2(mapSize - 1, 0));
        startPos.Add(new Vector2(0, 0));
        startPos.Add(new Vector2(0, mapSize-1));
        startPos.Add(new Vector2(mapSize-1, mapSize-1));
    }

    public TileNode GetTileNode(int y, int x)
    {
        print(grid[mapSize * y + x]);
        return grid[mapSize * y + x];
    }
}