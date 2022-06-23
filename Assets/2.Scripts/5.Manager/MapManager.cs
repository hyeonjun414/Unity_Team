
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;



public class MapManager : Singleton<MapManager>
{
    public Map map;
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }

    private void Start()
    {
        map = FindObjectOfType<Map>();
    }

    public bool BoundaryCheck(Point curPoint, Point diffPoint)
    {
        curPoint += diffPoint;

        if (curPoint.x < 0 || map.mapSize <= curPoint.x ||
            curPoint.y < 0 || map.mapSize <= curPoint.y ||
            map.GetTileNode(curPoint).eOnTileObject == eTileOccupation.WALL)
            return false;
        else
            return true;
    }

    public TileNode GetEmptyNode()
    {
        TileNode emptyNode = null;
        while(true)
        {
            Point point = new Point(Random.Range(0, map.mapSize), Random.Range(0, map.mapSize));
            emptyNode = map.GetTileNode(point);
            if(emptyNode.eOnTileObject == eTileOccupation.EMPTY)
            {
                break;
            }
        }
        return emptyNode;
    }
}
