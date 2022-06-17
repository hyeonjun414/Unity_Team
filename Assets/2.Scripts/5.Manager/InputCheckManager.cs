using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class InputCheckManager : MonoBehaviourPun
{
    //input 종류 - null(입력실패) , 움직임, 회전, 공격, 방어, 아이템사용, 슬롯체인지
    //null , 슬롯체인지는 영향없음
    //의식의 흐름()=> 매 노트카운트 끝부분 콜라이더에 닿으면 
    //InputCheckManager.Judge() 호출 => MoveJudge => ActualMovement => Judge 
    //=> 플레이어 키 입력 가능하게 초기화
    [HideInInspector]
    public int isReadyCount=0;
    public List<Character> players;

    public List<CachedPlayer> cachedPlayers;

    public static InputCheckManager Instance {get;private set;}
    private void Awake()
    {
        if (Instance == null)  Instance = this;
        

        cachedPlayers = new List<CachedPlayer>();
        
        players = new List<Character>();
        StartCoroutine(GetPlayers());
        
   
    }

    public class CachedPlayer
    {
        public CachedPlayer(int playerNumber,int command, int destX,int destY,int curPosX,int curPosY)
        {
            this.playerNumber = playerNumber;
            this.command = command;
            this.destX = destX;
            this.destY = destY;
            this.curPosX = curPosX;
            this.curPosY = curPosY;
        }
        public int playerNumber;
        public int command;
        public int destX;
        public int destY;
        public int curPosX;
        public int curPosY;
        public bool isMoving;
        public bool isCrashing;
    }
    public void CachePlayersCommand(int playerNumber , int command, int playerDestX, int playerDestY, int curPosX , int curPosY)
    {
        CachedPlayer cp = new CachedPlayer(playerNumber,command,playerDestX,playerDestY,curPosX,curPosY);
        cp.isMoving = true;
        cp.isCrashing = false;
        cachedPlayers.Add(cp);
    }
    
    public void ResisterPlayer(Character player) 
    //네트워크에 접속한 플레이어들이 awake에서 이 함수를 호출하여 호스트의 input매니저에 등록
    {
        players.Add(player);
        //cachedPlayers.Add(new CachedPlayer());
        ++isReadyCount;
        Debug.Log("isReadyCount: "+isReadyCount);
    }
    IEnumerator GetPlayers()
    {
        yield return new WaitUntil(()=>isReadyCount >= PhotonNetwork.PlayerList.Length);
        //전부 등록되면 게임 시작
        
        for(int i=0 ;i<players.Count; ++i)
        {
            players[i].isInputAvailable = true;
        }
    }

    public void Judge()
    {
        //CheckPlayersAvailability();
        MoveJudge();
        //AttackJudge();
        //ItemJudge();
        ResetPlayers();
    }
    // public TileNode GetHeadingNode(Character player) //플레이어가 누른 방향의 노드 (앞 아님)
    // {
    //     TileNode headingNode = MapManager_verStatic.Instance.map.GetTileNode(
    //             (int)player.characterStatus.curPositionY +  (int)player.playerHeadingPos.y,
    //             (int)player.characterStatus.curPositionX +  (int)player.playerHeadingPos.x);
    //     return headingNode;    
    // }
    // public TileNode GetHeadingNode(CachedPlayer player) //플레이어가 누른 방향의 노드 (앞 아님)
    // {
    //     TileNode headingNode = MapManager_verStatic.Instance.map.GetTileNode(
    //             (int)player.curPosY +  (int)player.playerHeadingDir.y,
    //             (int)player.curPosX +  (int)player.playerHeadingDir.x);
    //     return headingNode;    
    // }
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

        List<CachedPlayer> listMove = new List<CachedPlayer>();
        //List<Character> listMove = new List<Character>();
        //이동을 먼저 다 함
        // for (int i = 0; i < players.Count; ++i)
        // {
        //     if (players[i].playerInput == ePlayerInput.MOVE)
        //     {
        //         listMove.Add(players[i]);
        //     }
        // }

        for (int i = 0; i < cachedPlayers.Count; ++i)
        {
            if (cachedPlayers[i].command == 1)
            {
                listMove.Add(cachedPlayers[i]);
            }
        }
        // for (int i = 0; i < listMove.Count; ++i) //무브커맨드를 한 플레이어들
        // {
        //     TileNode headingNode = GetHeadingNode(listMove[i]);
            
        //     if(MapManager_verStatic.Instance.map.GetTileNode(headingNode.posY,headingNode.posX).eOnTileObject == eTileOccupation.PLAYER)
        //     {
        //         listMove[i].isMoving = false;
        //     }
        //     for (int j = i + 1; j < listMove.Count; ++j)
        //     {
        //         if (headingNode == GetHeadingNode(listMove[j]))
        //         {
        //             listMove[i].isMoving = false;
        //             listMove[i].isCrashing = true;
        //         }
        //     }

        // }
        for (int i = 0; i < listMove.Count; ++i) //무브커맨드를 한 플레이어들
        {
            TileNode destNode = MapManager_verStatic.Instance.map.GetTileNode(listMove[i].destY,listMove[i].destX);
            
            if(destNode.eOnTileObject == eTileOccupation.PLAYER)
            {
                listMove[i].isMoving = false;
            }
            for (int j = i + 1; j < listMove.Count; ++j)
            {
                if (destNode == MapManager_verStatic.Instance.map.GetTileNode(listMove[j].destY,listMove[j].destX))
                {
                    listMove[i].isMoving = false;
                    listMove[i].isCrashing = true;
                }
            }

        }
        ActualMovement(listMove);
    }

    // public void ActualMovement(List<Character> listMove)
    // {
    //     for (int i = 0; i < listMove.Count; ++i)
    //     {
    //         if (listMove[i].isCrashing)
    //         {
    //             //부딪히는 애니메이션처리 앞으로 부딪히고 바로 뒤로 복귀
    //             listMove[i].isMoving = false;
    //             listMove[i].isCrashing = false; //초기화
    //         }
    //         if (listMove[i].isMoving)
    //         {
    //             listMove[i].characterStatus.curPositionX = GetHeadingNode(listMove[i]).posX;
    //             listMove[i].characterStatus.curPositionY = GetHeadingNode(listMove[i]).posY;
    //             //실제로 움직이는거 구현 후
    //             MapManager_verStatic.Instance.map.GetTileNode(listMove[i].characterStatus.curPositionY,listMove[i].characterStatus.curPositionX).eOnTileObject = eTileOccupation.EMPTY;
    //             (GetHeadingNode(listMove[i])).eOnTileObject = eTileOccupation.PLAYER;
    //         }
    //     }
    // }
    public void ActualMovement(List<CachedPlayer> listMove)
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
                MapManager_verStatic.Instance.map.GetTileNode(listMove[i].curPosY,listMove[i].curPosX).eOnTileObject = eTileOccupation.EMPTY;
                MapManager_verStatic.Instance.map.GetTileNode(listMove[i].destX,listMove[i].destY).eOnTileObject = eTileOccupation.PLAYER;
                listMove[i].curPosX = listMove[i].destX;
                listMove[i].curPosY = listMove[i].destY;
                //실제로 움직이는거 구현 후
                   
            }

        }
        for(int i=0; i<listMove.Count;++i)
        {
            photonView.RPC("SendBackInfo",RpcTarget.AllBuffered,
                        new object[5]{listMove[i].playerNumber, 
                                    listMove[i].curPosX,
                                    listMove[i].curPosY,
                                    listMove[i].isMoving,
                                    listMove[i].isCrashing});
        
        }
    }
    [PunRPC]
    public void SendBackInfo(int playerNumber, int curPosX, int curPosY, bool isMoving, bool isCrashing)
    {
        if(PhotonNetwork.LocalPlayer.GetPlayerNumber() == playerNumber)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
            for(int i=0; i<objs.Length;++i)
            {
                Character player = objs[i].GetComponent<Character>();
                if(player.playerNumber == playerNumber)
                {
                    player.characterStatus.curPositionX = curPosX;
                    player.characterStatus.curPositionY = curPosY;
                    player.isCrashing = isCrashing;
                    player.isMoving = isMoving;
                }
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
    [PunRPC]
    public void ResetPlayers()
    {
        //플레이어들의 버튼 입력 가능하도록 초기화
        for (int i = 0; i < cachedPlayers.Count; ++i)
        {
            cachedPlayers[i].isInputAvailable = true;
            players[i].playerInput = ePlayerInput.NULL;
            players[i].playerHeadingPos = Vector2.zero;
            players[i].isMoving = true;//초기화
            players[i].isCrashing = false;
        }
    }








}
