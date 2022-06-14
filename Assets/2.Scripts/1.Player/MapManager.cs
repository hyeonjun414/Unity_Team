using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{

    private void Awake()
    {
        if (_instance == null)  _instance = this;

        //if(호스트면) 
        //GenerateMap -> 혹은 맵을 시작 전 룸에서 미리 선택하고 넘어오는지?

        ReadRoomData();
    }
    public void ReadRoomData()
    {
        
    }
}
