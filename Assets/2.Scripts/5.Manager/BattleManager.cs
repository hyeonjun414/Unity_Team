using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class BattleManager : MonoBehaviourPun
{
    //input 종류 - null(입력실패) , 움직임, 회전, 공격, 방어, 아이템사용, 슬롯체인지
    //null , 슬롯체인지는 영향없음
    //의식의 흐름()=> 매 노트카운트 끝부분 콜라이더에 닿으면 
    //InputCheckManager.Judge() 호출 => MoveJudge => ActualMovement => Judge 
    //=> 플레이어 키 입력 가능하게 초기화
    [HideInInspector]
    public int isReadyCount=0;

    public List<Character> players;

    public static BattleManager Instance {get;private set;}
    private void Awake()
    {
        if (Instance == null)  Instance = this;
   
    }
    public void RegisterAllPlayer()
    {
        players = FindObjectsOfType<Character>().ToList();
        players.Sort((Character a, Character b) =>
        {
            if (a.playerNumber < b.playerNumber)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        });
    }


    public void Judge()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            MoveJudge();

            photonView.RPC("ResetPlayers", RpcTarget.All);
        }
        //CheckPlayersAvailability();
        //AttackJudge();
        //ItemJudge();

    }
    public void CheckPlayersAvailability()
    {
        // 플레이어 명령 기반
        // 입력이 없다면 NULL 

    }
    public void MoveJudge()
    {
        List<Character> playerList = players.FindAll((x) =>
        x.eCurInput == ePlayerInput.MOVE_UP ||
        x.eCurInput == ePlayerInput.MOVE_RIGHT ||
        x.eCurInput == ePlayerInput.MOVE_DOWN ||
        x.eCurInput == ePlayerInput.MOVE_LEFT);

        List<TileNode> destNodes = new List<TileNode>();

        foreach(Character player in playerList)
        {
            // 각 플레이어가 도달할 다음 노드를 받아옴
            destNodes.Add(player.moveCommand.NodeDetect());
        }

        for(int i = 0; i < destNodes.Count; i++)
        {
            if (destNodes.FindAll((x) => x == destNodes[i]).Count >= 2 ||
                destNodes[i].eOnTileObject == eTileOccupation.PLAYER)
            {
                playerList[i].eCurInput = ePlayerInput.NULL;
            }
        }
        // 도착 노드에 대한 예외처리
        ActualMovement(playerList);
    }

    public void ActualMovement(List<Character> playerList)
    {
        foreach(Character player in playerList)
        {
            if(player.eCurInput != ePlayerInput.NULL)
            {
                print($"player{player.playerNumber} Move");
                player.photonView.RPC("Move", RpcTarget.All);
            }
        }
    }

    public void AttackJudge()
    {
        
    }
    public void ItemJudge()
    {
    }
    [PunRPC]
    public void ResetPlayers()
    {
        foreach(Character player in players)
        {
            //player.eCurInput = ePlayerInput.NULL;
        }
        RhythmManager.Instance.isBeat = true;
    }








}
