﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class InputCheckManager : Singleton<InputCheckManager>
{
    //input 종류 - null(입력실패) , 움직임, 회전, 공격, 방어, 아이템사용, 슬롯체인지
    //null , 슬롯체인지는 영향없음
    //의식의 흐름()=> 매 노트카운트 끝부분 콜라이더에 닿으면 
    //InputCheckManager.Judge() 호출 => MoveJudge => ActualMovement => Judge 
    //=> 플레이어 키 입력 가능하게 초기화
    [HideInInspector]
    public int isReadyCount=0;
    List<Character> players;
    private void Awake()
    {
        if (_instance == null)  _instance = this;
        
        if(PhotonNetwork.IsMasterClient)
        {
            players = new List<Character>();
            StartCoroutine(GetPlayers());
        }
        
    }
    
    public void ResisterPlayer(Character player) 
    //네트워크에 접속한 플레이어들이 awake에서 이 함수를 호출하여 호스트의 input매니저에 등록
    {

        players.Add(player);
        ++isReadyCount;

    }
    IEnumerator GetPlayers()
    {
        yield return new WaitUntil(()=>isReadyCount >= MapManager_verStatic.Instance.playerCount);
        //전부 등록되면 게임 시작
        
        for(int i=0 ;i<players.Count; ++i)
        {
            players[i].isInputAvailable = true;
        }
    }

    public void Judge()
    {
        CheckPlayersAvailability();
        MoveJudge();
        AttackJudge();
        ItemJudge();
        ResetPlayers();
    }
    public TileNode GetHeadingNode(Character player) //플레이어가 누른 방향의 노드 (앞 아님)
    {
        TileNode headingNode = MapManager_verStatic.Instance.map.GetTileNode(
                (int)player.characterStatus.curPositionY +  (int)player.playerHeadingPos.y,
                (int)player.characterStatus.curPositionX +  (int)player.playerHeadingPos.x);
        return headingNode;    
    }
    public void CheckPlayersAvailability()
    {
        for (int i = 0; i < players.Count; ++i)
        {
            if (players[i] == null)
            {
                players.Remove(players[i]);
            }
        }
    }
    public void MoveJudge()
    {
        Debug.Log("인풋매니저경유");
        List<Character> listMove = new List<Character>();
        //이동을 먼저 다 함
        for (int i = 0; i < players.Count; ++i)
        {
            if (players[i].playerInput == ePlayerInput.MOVE)
            {
                listMove.Add(players[i]);
            }
        }

        for (int i = 0; i < listMove.Count; ++i) //무브커맨드를 한 플레이어들
        {
            TileNode headingNode = GetHeadingNode(listMove[i]);
            
            if(MapManager_verStatic.Instance.map.GetTileNode(headingNode.posY,headingNode.posX).eOnTileObject == eTileOccupation.PLAYER)
            {
                listMove[i].isMoving = false;
            }
            for (int j = i + 1; j < listMove.Count; ++j)
            {
                if (headingNode == GetHeadingNode(listMove[j]))
                {
                    listMove[i].isMoving = false;
                    listMove[i].isCrashing = true;
                }
            }

        }
        ActualMovement(listMove);
    }

    public void ActualMovement(List<Character> listMove)
    {
        for (int i = 0; i < listMove.Count; ++i)
        {
            if (listMove[i].isCrashing)
            {
                //부딪히는 애니메이션처리 앞으로 부딪히고 바로 뒤로 복귀
                listMove[i].isMoving = false;
                listMove[i].isCrashing = false; //초기화
            }
            if (listMove[i].isMoving)
            {
                listMove[i].characterStatus.curPositionX = GetHeadingNode(listMove[i]).posX;
                listMove[i].characterStatus.curPositionY = GetHeadingNode(listMove[i]).posY;
                //실제로 움직이는거 구현 후
                MapManager_verStatic.Instance.map.GetTileNode(listMove[i].characterStatus.curPositionY,listMove[i].characterStatus.curPositionX).eOnTileObject = eTileOccupation.EMPTY;
                (GetHeadingNode(listMove[i])).eOnTileObject = eTileOccupation.PLAYER;
            }
        }
    }
    public void AttackJudge()
    {
        List<Character> listAttack = new List<Character>();

        for (int i = 0; i < players.Count; ++i)
        {
            if (players[i].playerInput == ePlayerInput.ATTACK)
            //         ||players[i].playerInput == ePlayerInput.BLOCK
            //         ||players[i].playerInput == ePlayerInput.USE_ITEM)
            {
                listAttack.Add(players[i]);
            }
        }
        for (int i = 0; i < listAttack.Count; ++i) //어택커맨드를 한 플레이어들
        {
            int x = ((CharacterAction)listAttack[i].actionCommand).tileFront.posX;
            int y = ((CharacterAction)listAttack[i].actionCommand).tileFront.posY;
            if(MapManager_verStatic.Instance.map.GetTileNode(y,x).eOnTileObject == eTileOccupation.PLAYER)
            {
                Character ohterPlayer = MapManager_verStatic.Instance.map.GetTileNode(y,x).objectOnTile.GetComponent<Character>();
                if(ohterPlayer.playerInput == ePlayerInput.BLOCK)//앞에 적이 있고 방어를 눌렀고 그리고 방향이 이쪽이라면
                {
                    //플레이어 스턴
                }
                else if (ohterPlayer.characterStatus.hp >= 0)//플레이어의 무브 처리를 이 이전에 한다는 가정하에
                {
                    ohterPlayer.Damaged((listAttack[i].GetComponent<Character>()).characterStatus.damage);
                }
                //else if(1)방향이 다르거나 등등
            }
        }

    }
    public void ItemJudge()
    {
        List<Character> listItemUse = new List<Character>();
        for (int i = 0; i < players.Count; ++i)
        {
            if (players[i].playerInput == ePlayerInput.USE_ITEM)
            {
                listItemUse.Add(players[i]);
            }
        }
        for (int i = 0; i < listItemUse.Count; ++i) //아이템사용커맨드를 한 플레이어들
        {
            int x = ((CharacterAction)listItemUse[i].actionCommand).tileFront.posX;
            int y = ((CharacterAction)listItemUse[i].actionCommand).tileFront.posY;
            // if(MapManager.Instance.grid[x,y].eOnTileObject == eTileOccupation.PLAYER)
            // {
            //     Character ohterPlayer = MapManager.Instance.grid[x,y].objectOnTile.GetComponent<Character>();
            // }
        }
    }
    public void ResetPlayers()
    {
        //플레이어들의 버튼 입력 가능하도록 초기화
        for (int i = 0; i < players.Count; ++i)
        {
            players[i].isInputAvailable = true;
            players[i].playerInput = ePlayerInput.NULL;
            players[i].playerHeadingPos = Vector2.zero;
            players[i].isMoving = true;//초기화
            players[i].isCrashing = false;
        }
    }








}
