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

    public void Judge()
    {
        if(PhotonNetwork.IsMasterClient)
        {

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
