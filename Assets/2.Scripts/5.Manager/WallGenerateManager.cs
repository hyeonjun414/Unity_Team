using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;


public class WallGenerateManager : Singleton<WallGenerateManager>
{
    [Header("Wall")]
    public int MaxWallCount = 10;
    public Wall wall;



    public Vector3 spawnOffset = new Vector3(0, 1f, 0);       // 타일로부터 스폰 위치 차이

    Point spawnTilePos = new Point();
    int spawnTileNum;                   //  스폰될 타일 번호
    Vector3 curSpawnPos;                //  스폰될 위치

    private void Awake()
    {
        if (_instance == null) _instance = this;


    }

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        Spawn();
    }

    private void Spawn()
    {

        for (int i = 0; i < MaxWallCount; i++)
        {
            SpawnWall();
        }

    }

    public void SpawnWall()
    {
        SetSpawnPos();
        PhotonNetwork.Instantiate("Wall", curSpawnPos + spawnOffset, Quaternion.identity);
        ItemSpawnManager.Instance.emptyTileCheckList[spawnTileNum] = false;

    }


    public Vector3 SetSpawnPos()
    // 좌표 만들기
    {
        int mapSize = MapManager.Instance.map.mapSize;

        spawnTilePos = new Point(Random.Range(1, mapSize - 1), Random.Range(1, mapSize - 1));
        spawnTileNum = mapSize * spawnTilePos.y + spawnTilePos.x;

        while (!ItemSpawnManager.Instance.emptyTileCheckList[spawnTileNum] == true)     // 비어있는 타일이 아닐경우 계속 랜덤 좌표 계산
        {
            spawnTilePos = new Point(Random.Range(1, mapSize - 1), Random.Range(1, mapSize - 1));

            spawnTileNum = mapSize * spawnTilePos.y + spawnTilePos.x;
        }

        curSpawnPos = MapManager.Instance.map.grid[spawnTileNum].transform.position;
        return curSpawnPos;
    }

}
