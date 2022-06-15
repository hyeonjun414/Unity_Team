using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCheckManager : Singleton<InputCheckManager>
{
    //input 종류 - null(입력실패) , 움직임, 회전, 공격, 방어, 아이템사용, 슬롯체인지
    //null , 슬롯체인지는 영향없음
    public int isReadyCount=0;
    List<Character> players;
    private void Awake()
    {
        if (_instance == null)  _instance = this;
        StartCoroutine(GetPlayers());
      
    }
    public void AddJudgement(Character player, string commandType)
    {

    }
    public void CheckMoveAvailability()
    {

    }
    public void CheckActionAvailability()
    {

    }
    public void MoveJudge()
    {
        List<Character> listMove = new List<Character>();
        //이동을 먼저 다 함
    }
    public void Judge()
    {
        MoveJudge();
        List<Character> listAttack = new List<Character>();
        List<Character> listItemUse = new List<Character>();
        for(int i=0; i<players.Count; ++i)
        {
            if(players[i].playerInput == ePlayerInput.ATTACK)
       //         ||players[i].playerInput == ePlayerInput.BLOCK
       //         ||players[i].playerInput == ePlayerInput.USE_ITEM)
            {
                listAttack.Add(players[i]);
            }
        }
        for(int i=0; i<listAttack.Count; ++i) //어택커맨드를 한 플레이어들
        {
            int x = ((CharacterAction)listAttack[i].actionCommand).tileFront.posX;
            int y = ((CharacterAction)listAttack[i].actionCommand).tileFront.posY;
            if(MapManager.Instance.grid[x,y].eOnTileObject == eTileOccupation.PLAYER)
            {
                Character ohterPlayer = MapManager.Instance.grid[x,y].objectOnTile.GetComponent<Character>();
                if(ohterPlayer.playerInput == ePlayerInput.BLOCK)//앞에 적이 있고 방어를 눌렀고 그리고 방향이 이쪽이라면
                {
                    //플레이어 스턴
                }
                else if (ohterPlayer.characterStatus.hp>=0)//플레이어의 무브 처리를 이 이전에 한다는 가정하에
                {
                    ohterPlayer.Damaged((listAttack[i].GetComponent<Character>()).characterStatus.damage);
                }
                //else if(1)방향이 다르거나 등등
            }
        }
        
        for(int i=0; i<players.Count; ++i)
        {
            if(players[i].playerInput == ePlayerInput.USE_ITEM)
            {
                listItemUse.Add(players[i]);
            }
        }
        for(int i=0; i<listAttack.Count; ++i) //아이템사용커맨드를 한 플레이어들
        {
            int x = ((CharacterAction)listAttack[i].actionCommand).tileFront.posX;
            int y = ((CharacterAction)listAttack[i].actionCommand).tileFront.posY;
            if(MapManager.Instance.grid[x,y].eOnTileObject == eTileOccupation.PLAYER)
            {
                Character ohterPlayer = MapManager.Instance.grid[x,y].objectOnTile.GetComponent<Character>();
            }
        }

        //플레이어들의 버튼 입력 가능하도록 초기화
        for(int i=0 ;i<players.Count; ++i)
        {
            players[i].isInputAvailable=true;
        }
    }
    public void ResisterPlayer(Character player)
    {
        players.Add(player);
        ++isReadyCount;
    }
    IEnumerator GetPlayers()
    {
        yield return new WaitUntil(()=>isReadyCount >= MapManager.Instance.playerCount);
        for(int i=0 ;i<players.Count; ++i)
        {
            players[i].isInputAvailable=true;
        }  
    }



}
