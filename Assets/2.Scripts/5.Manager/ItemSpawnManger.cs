using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManger : Singleton<ItemSpawnManger>
{
    public Item[] spawnItemType;             // 스폰할 아이템 타입
    int spawnItemTypeNum;
    public Item[,] item;


    public int maxItemCount = 25;   // 맵에 최대 소환될 수 있는 아이템 개수
    public int curItemCount = 0;

    public int maxSpawnItemCount = 10;       // 쿨타임 마다 스폰할 아이템 개수
    public int curSpawnItemCount = 0;

    public bool[,] emptyTileCheckList;

    public float spawnCountDown = 5.0f;          // 스폰 쿨타임
    public float countdown = 5.0f;        // 시간 계산용 쿨타임
    public float itemLife = 5.0f;         // 아이템 수명

    public Vector3 spawnOffset = new Vector3(0, 1.5f, 0);       // 타일로부터 스폰 위치 차이

    int itemSpawnTileX;                   // 아이템 스폰될 타일의 X축 좌표
    int itemSpawnTileY;                   // 아이템 스폰될 타일의 Y축 좌표
    Vector3 curItemSpawnPos;                 // 아이템 스폰될 위치

    private void Awake()
    {
        if (_instance == null) _instance = this;
    }

    private void Start()
    {
        maxItemCount = MapManager.Instance.mapSizeX * MapManager.Instance.mapSizeY - 4;
        emptyTileCheckList = new bool[MapManager.Instance.mapSizeX, MapManager.Instance.mapSizeY];
        item = new Item[MapManager.Instance.mapSizeX, MapManager.Instance.mapSizeY];
        MakeSpawnEmptyCheckList();
    }

    private void Update()
    {
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
                    SetSpawnPos();                              // 아이템 좌표 가져오기
                    SetSpawnItem();                             // 소환할 아이템 종류 가져오기
                    SpawnItem();
                }
            }
        }
    }
    public void SpawnItem()
    {
        item[itemSpawnTileX, itemSpawnTileY] = Instantiate(spawnItemType[spawnItemTypeNum], curItemSpawnPos, Quaternion.identity);

        item[itemSpawnTileX, itemSpawnTileY].posX = itemSpawnTileX;
        item[itemSpawnTileX, itemSpawnTileY].posY = itemSpawnTileY;

        //해당 노드를 아이템 점유 타일로 변경
        MapManager.Instance.grid[itemSpawnTileX, itemSpawnTileY].eOnTileObject = eTileOccupation.ITEM;
        curItemCount++;
    }


    public Item SetSpawnItem()
    {
        spawnItemTypeNum = Random.Range(0, spawnItemType.Length);


        return spawnItemType[spawnItemTypeNum];
    }

    public void MakeSpawnEmptyCheckList()
    {
        for (int i = 0; i < MapManager.Instance.mapSizeX; ++i)
        {
            for (int j = 0; j < MapManager.Instance.mapSizeY; ++j)
            {
                if (MapManager.Instance.grid[i, j].eOnTileObject == eTileOccupation.EMPTY)
                {
                    emptyTileCheckList[i, j] = true;
                }
                else
                {
                    emptyTileCheckList[i, j] = false;
                }
            }
        }
    }


    public Vector3 SetSpawnPos()
    {
        int check = 0;
        while (!emptyTileCheckList[itemSpawnTileX, itemSpawnTileY] == true)     // 비어있는 타일이 아닐경우 계속 랜덤 좌표 계산
        {
            Debug.Log("반복횟수" + check++);
            itemSpawnTileX = Random.Range(0, MapManager.Instance.mapSizeX);
            itemSpawnTileY = Random.Range(0, MapManager.Instance.mapSizeY);
        }

        curItemSpawnPos = MapManager.Instance.grid[itemSpawnTileX, itemSpawnTileY].transform.position + spawnOffset;

        return curItemSpawnPos;
    }

}
