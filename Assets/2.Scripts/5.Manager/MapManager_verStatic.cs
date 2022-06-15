using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;



public class MapManager_verStatic : Singleton<MapManager_verStatic>
{
    public int playerCount;
    public Character player;
    public Map map;
    private void Awake()
    {
        if (_instance == null) 
            _instance = this;
    }

    private void Start()
    {

        //플레이어 인원 받아서 할당하기
        playerCount = 4;
        if (playerCount > 4) playerCount = 4;//혹시 네트워크에러? 등으로 최대인원보다 

        //if(호스트면) 
        GeneratePlayer();
    }

    private void GeneratePlayer()
    {
        for (int i = 0; i < playerCount; ++i)
        {
            Vector2 tilePos = map.startPos[i];
            print(map.startPos[i]);
            Character objPlayer = Instantiate(player, Vector3.zero, Quaternion.identity);
            if (i == 0)
            {
                objPlayer.isLocal = true;
                CinemachineVirtualCamera virCam = GameObject.Find("LocalCamera").GetComponent<CinemachineVirtualCamera>();
                virCam.Follow = objPlayer.transform;
                objPlayer.SetUp(map.GetTileNode((int)tilePos.x, (int)tilePos.y));
            }
            else
            {
                objPlayer.SetUp(map.GetTileNode((int)tilePos.x, (int)tilePos.y));
            }
        }
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
