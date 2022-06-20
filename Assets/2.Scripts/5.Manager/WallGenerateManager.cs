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



    public Vector3 spawnOffset = new Vector3(0, 1f, 0);       // ??쇰줈遺???ㅽ룿 ?꾩튂 李⑥씠

    Point spawnTilePos = new Point();
    int spawnTileNum;                   //  ?ㅽ룿?????踰덊샇
    Vector3 curSpawnPos;                //  ?ㅽ룿???꾩튂

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
    // 醫뚰몴 留뚮뱾湲?
    {
        int mapSize = MapManager.Instance.map.mapSize;

        spawnTilePos = new Point(Random.Range(1, mapSize - 1), Random.Range(1, mapSize - 1));
        spawnTileNum = mapSize * spawnTilePos.y + spawnTilePos.x;

        while (!ItemSpawnManager.Instance.emptyTileCheckList[spawnTileNum] == true)     // 鍮꾩뼱?덈뒗 ??쇱씠 ?꾨땺寃쎌슦 怨꾩냽 ?쒕뜡 醫뚰몴 怨꾩궛
        {
            spawnTilePos = new Point(Random.Range(1, mapSize - 1), Random.Range(1, mapSize - 1));

            spawnTileNum = mapSize * spawnTilePos.y + spawnTilePos.x;
        }

        curSpawnPos = MapManager.Instance.map.grid[spawnTileNum].transform.position;
        return curSpawnPos;
    }

}
