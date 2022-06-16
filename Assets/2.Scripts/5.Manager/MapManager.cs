using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;



public class MapManager : Singleton<MapManager>
{
    public int playerCount;
    public TileNode prefNode;
    public Character player;
    public const float spaceBetweenTiles = 2.5f;
    public int mapSizeX;
    public int mapSizeY;
    public TileNode[,] grid;
    private Vector3[] playerSpawnPos = new Vector3[4];

    private void Awake()
    {
        if (_instance == null) _instance = this;

        //플레이어 인원 받아서 할당하기
        playerCount = 4;
        if (playerCount > 4) playerCount = 4;//혹시 네트워크에러? 등으로 최대인원보다 많게될 시 예외처리

        mapSizeX=10;//임시로 맵사이즈용 변수 지정
        mapSizeY=10;//임시로 맵사이즈용 변수 지정
        grid = new TileNode[mapSizeY,mapSizeX];

        Vector3 basePlayerPos = new Vector3(0f,0.25f,0f);
        playerSpawnPos[0] = new Vector3(spaceBetweenTiles*0,           basePlayerPos.y,    spaceBetweenTiles*0);
        playerSpawnPos[1] = new Vector3(spaceBetweenTiles*(mapSizeX-1),basePlayerPos.y,    -spaceBetweenTiles*(mapSizeY-1));
        playerSpawnPos[2] = new Vector3(spaceBetweenTiles*(mapSizeX-1),basePlayerPos.y,    spaceBetweenTiles*0);
        playerSpawnPos[3] = new Vector3(spaceBetweenTiles*0,           basePlayerPos.y,    -spaceBetweenTiles*(mapSizeY-1));



        //if(호스트면) 
        GenerateMap(); //-> 혹은 맵을 시작 전 룸에서 미리 선택하고 넘어오는지?

        ReadRoomData();


    }
    public void GenerateMap()
    {
        GameObject obj = new GameObject("Tiles");
        Debug.Log(mapSizeX);
        for(int i=0; i<mapSizeY; ++i)
        {
            for(int j=0; j<mapSizeX; ++j)
            {
                grid[i,j] = Instantiate(prefNode,new Vector3(j*spaceBetweenTiles,0,-i*spaceBetweenTiles),Quaternion.identity);
                grid[i,j].eOnTileObject = eTileOccupation.EMPTY;
                grid[i,j].posX = j;
                grid[i,j].posY = i;
                grid[i,j].transform.SetParent(obj.transform);
            }
        }

        for (int i = 0; i < playerCount; ++i)
        {
            Character objPlayer = Instantiate(player, playerSpawnPos[i], Quaternion.identity);
            if (i == 0)
            {
               // objPlayer.isLocal = true;
                CinemachineVirtualCamera virCam = GameObject.Find("LocalCamera").GetComponent<CinemachineVirtualCamera>();
                virCam.Follow = objPlayer.transform;
                //objPlayer.SetUp(grid[0, 0]);

            }
            else if (i == 1)
            {
               // objPlayer.SetUp(grid[mapSizeX - 1, mapSizeX - 1]);
            }
            else if (i == 2)
            {
               // objPlayer.SetUp(grid[0, mapSizeX - 1]);
            }
            else if (i == 3)
            {
                //objPlayer.SetUp(grid[mapSizeX - 1, 0]);
            }
            //해당 노드를 플레이어 점유 타일로 변경
            grid[objPlayer.characterStatus.curPositionX,objPlayer.characterStatus.curPositionY].eOnTileObject 
                = eTileOccupation.PLAYER;
        }


    }
    public bool BoundaryCheck(int y, int x, Vector2 vec)
    {
        x += (int)vec.x;
        y += (int)vec.y;
        if (x < 0 || mapSizeX <= x ||
            y < 0 || mapSizeY <= y)
            return false;
        else
            return true;
    }
    public void ReadRoomData()
    {

    }
}
