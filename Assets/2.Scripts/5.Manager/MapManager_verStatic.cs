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
    public bool BoundaryCheck(int y, int x, Vector2 vec)
    {
        x += (int)vec.x;
        y += (int)vec.y;
        if (x < 0 || map.mapSize <= x ||
            y < 0 || map.mapSize <= y)
            return false;
        else
            return true;
    }
}
