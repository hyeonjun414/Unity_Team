using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class ItemSpawnManager : Singleton<ItemSpawnManager>
{
    public string[] spawnItemType;                // 스폰될 아이템 타입
    // public Item[] spawnItemType;                // 스폰될 아이템 타입

    public ItemDB itemDB;
    int spawnItemTypeNum;
    public Item[] item;

    public int maxItemCount = 25;           // 맵에 최대 소환될 수 있는 아이템 개수
    public int curItemCount = 0;

    public int maxSpawnItemCount = 3;       // 쿨타임 마다 스폰할 아이템 개수
    public int curSpawnItemCount = 0;

    public bool[] emptyTileCheckList;

    public float spawnCountDown = 5.0f;     // 스폰 쿨타임
    public float countdown = 5.0f;          // 시간 계산용 쿨타임

    public Vector3 spawnOffset = new Vector3(0, 1.5f, 0);       // 타일로부터 스폰 위치 차이

    Point itemSpawnTilePos = new Point();
    int itemSpawnTileX;                   // 아이템 스폰될 타일의 X축 좌표
    int itemSpawnTileY;                   // 아이템 스폰될 타일의 Y축 좌표
    int itemSpawnTileNum;                   // 아이템 스폰될 타일 번호
    int itemSpawnTileCheckNum;
    Vector3 curItemSpawnPos;                // 아이템 스폰될 위치

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

        item = new Item[MapManager.Instance.map.grid.Count];
        maxItemCount = MapManager.Instance.map.grid.Count;
        emptyTileCheckList = new bool[MapManager.Instance.map.grid.Count];

        MakeSpawnEmptyCheckList();
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        SpawnTimer();
        Spawn();
    }

    private void SpawnTimer()
    {
        if (countdown <= 0)
        {
            countdown = spawnCountDown;
        }
        else
        {
            countdown -= Time.deltaTime;
        }
    }

    private void Spawn()
    {
        if (countdown <= 0)
        {
            if (maxItemCount <= curItemCount)      // 현재 스폰된 아이템 수가 최대로 가질 수 있는 수보다 많을 경우
            {
                return;
            }
            else
            {
                if (maxItemCount - curItemCount < curSpawnItemCount)    // 비어있는 타일이 소환할 아이템보다 적을 경우
                {
                    curSpawnItemCount = maxItemCount - curItemCount;
                }
                else
                {
                    curSpawnItemCount = maxSpawnItemCount;
                }

                for (int i = 0; i < curSpawnItemCount; i++)     // 소환할 아이템 갯수만큼 아이템 소환
                {
                    MakeSpawnEmptyCheckList();
                    // 아이템 좌표 가져오기
                    SetSpawnItemType();                             // 소환할 아이템 종류 가져오기
                    SpawnItem();                                // 아이템 스폰
                }
            }
        }
    }

    public void SpawnItem()
    {
        PhotonNetwork.Instantiate(itemDB.itemList[spawnItemTypeNum].prefab.name, SetSpawnPos(), Quaternion.identity);

        curItemCount++;
    }

    public string SetSpawnItemType()
    {
        spawnItemTypeNum = Random.Range(0, itemDB.itemList.Length);

        return spawnItemType[spawnItemTypeNum];
    }

    public void MakeSpawnEmptyCheckList()
    // 비어있는 칸 체크
    {

        for (int i = 0; i < MapManager.Instance.map.grid.Count; ++i)
        {
            if (MapManager.Instance.map.grid[i].eOnTileObject == eTileOccupation.EMPTY)
            {
                emptyTileCheckList[i] = true;
            }
            else
            {
                emptyTileCheckList[i] = false;
            }
        }
    }

    public Vector3 SetSpawnPos()
    // 좌표 만들기
    {
        int mapSize = MapManager.Instance.map.mapSize;

        itemSpawnTilePos = new Point(Random.Range(0, mapSize), Random.Range(0, mapSize));
        itemSpawnTileNum = mapSize * itemSpawnTilePos.y + itemSpawnTilePos.x;

        while (!emptyTileCheckList[itemSpawnTileNum] == true)     // 비어있는 타일이 아닐경우 계속 랜덤 좌표 계산
        {
            itemSpawnTilePos = new Point(Random.Range(0, mapSize), Random.Range(0, mapSize));

            itemSpawnTileNum = mapSize * itemSpawnTilePos.y + itemSpawnTilePos.x;
        }

        curItemSpawnPos = MapManager.Instance.map.grid[itemSpawnTileNum].transform.position + spawnOffset;

        return curItemSpawnPos;
    }

}
