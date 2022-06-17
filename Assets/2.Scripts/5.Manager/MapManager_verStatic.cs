using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;



public class MapManager_verStatic : Singleton<MapManager_verStatic>
{
    public int playerCount=0;
    public Character player;
    public Map map;
    private void Awake()
    {
        if (_instance == null) 
            _instance = this;
    }
    public bool BoundaryCheck(Point curPoint, Point diffPoint)
    {
        curPoint += diffPoint;

        if (curPoint.x < 0 || map.mapSize <= curPoint.x ||
            curPoint.y < 0 || map.mapSize <= curPoint.y)
            return false;
        else
            return true;
    }
}
