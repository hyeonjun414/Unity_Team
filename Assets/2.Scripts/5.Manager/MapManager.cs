
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;



public class MapManager : Singleton<MapManager>
{
    public Map map;
    public MapData mapData;
    public MapType mapType;
    public Text mapText; 
    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        GenerateMap();
    }

    private void Start()
    {
       // map = FindObjectOfType<Map>();
    }

    public void GenerateMap()
    {
        object value;
        if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameData.GAME_MAP, out value))
        {
            mapType = (MapType)value;
            mapText.text = GameData.GetMap(mapType);

            map = Instantiate(mapData.maps[(int)mapType], Vector3.zero, Quaternion.identity);
        }
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
