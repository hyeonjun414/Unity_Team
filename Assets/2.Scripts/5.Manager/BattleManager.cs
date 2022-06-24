using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;
using TMPro;

public class BattleManager : MonoBehaviourPun
{
    //input 종류 - null(입력실패) , 움직임, 회전, 공격, 방어, 아이템사용, 슬롯체인지
    //null , 슬롯체인지는 영향없음
    //의식의 흐름()=> 매 노트카운트 끝부분 콜라이더에 닿으면 
    //InputCheckManager.Judge() 호출 => MoveJudge => ActualMovement => Judge 
    //=> 플레이어 키 입력 가능하게 초기화
    [HideInInspector]
    public int isReadyCount = 0;

    public List<Character> players;
    public GameObject battleResultPanel;
    public GameObject[] playerInfoUIs;


    [Header("Player")]

    //살아있는 플레이어 
    public List<Character> alivePlayer;
    //사망한 플레이어
    public List<Character> deadPlayer;

    [Header("Text")]
    public Text modeText;
    public Text resultText;
    public GameObject resultTextObj;

    [Header("UI")]
    public RegenUI regenUI;

    [Header("Mode")]
    public ModeType mode;
  
    public static BattleManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;

        
    }

    public void SetUpMode()
    {
        object modeData;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameData.GAME_MODE, out modeData))
        {
            mode = (ModeType)modeData;
            modeText.text = GameData.GetMode(mode);
            switch (mode)
            {
                case ModeType.LastFighter:
                    SetUpDeathMatch();
                    break;
                case ModeType.OneShot:
                    SetUpOneShotMode();
                    break;
                case ModeType.TimeToKill:
                    SetUpTimerMode();
                    break;
            }
        }
    }
    
    //     FinalWinner();
    // }

    private void Update() {
        //FinalWinner();
    }

    public void SetUpDeathMatch()
    {

    }
    
    public void SetUpOneShotMode()
    {
        // 한대 맞으면 죽는 데스매치
        foreach(Character p in players)
        {
            p.stat.hp = 1;
        }
    }
    public void SetUpTimerMode()
    {
        TimeManager.Instance.limitTime = 180f;
        foreach (Character p in players)
        {
            p.isRegen = true;
        }
    }

    public void RegisterAllPlayer()
    {
        players = FindObjectsOfType<Character>().ToList();

        //게임이 시작했을 때 들어온 모든 플레이어를 살아있는 플레이어 그룹에 넣는다.
        alivePlayer = FindObjectsOfType<Character>().ToList();


        // 등록이 완료되었다면 모드 설정에 따라 설정을 진행한다.
        SetUpMode();

    }



    //플레이어가 죽었을 때 판정 || 플레이어가 disconnect되었을 때 호출

    public void PlayerOut(Character deadPL)
    {

        //alivePlayer 리스트에서 죽은 플레이어를 뺀다.
        alivePlayer.Remove(deadPL);

        Debug.Log("playerOut: "+deadPL.playerId);
        
        //deadPlayer 리스트에 죽은 플레이어를 더해준다.
        deadPlayer.Add(deadPL);

        if(alivePlayer.Count ==1)
        {
            photonView.RPC("BattleOverMessage", RpcTarget.All);
            StartCoroutine(GameOver());
        }
        
    }

    [PunRPC]
    public void BattleOverMessage()
    {
        resultTextObj.SetActive(true);

        string myNickName = PhotonNetwork.LocalPlayer.NickName;

        //플레이어가 죽지 않았을 때
        foreach(Character player in alivePlayer){
            if(player.nickName == myNickName)
            {
                resultText.text = "YOU WIN!";
                return;
            }
       }
        //플레이어가 죽었을 때
        foreach(Character player in deadPlayer){
            if(player.nickName == myNickName)
            {
                resultText.text = "YOU LOSE!";
                return;
            }
        }

        
        
    }
    private void SetBattleResult()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            
            if(alivePlayer[0].photonView.Owner.ActorNumber == p.ActorNumber)
            {
                TMP_Text[] texts = playerInfoUIs[0].transform.GetComponentsInChildren<TMP_Text>();
                for(int i=0; i<texts.Length;++i)
                {
                    switch(texts[i].gameObject.name)
                    {
                        case "PlayerNick":
                        {
                            texts[i].text = alivePlayer[0].nickName;
                        }break;
                        case "Kill":
                        {
                            texts[i].text = p.GetScore().ToString();
                        }break;
                        case "Death":
                        {
                            texts[i].text = alivePlayer[0].stat.deathCount.ToString();
                        }break;
                        case "Rank":
                        {
                            texts[i].text = "승리";
                        }break;
                        
                    }

                }
            }
            for(int i=0; i<deadPlayer.Count;++i)
            {
                if(deadPlayer[i].photonView.Owner.ActorNumber == p.ActorNumber)
                {
                    TMP_Text[] texts = playerInfoUIs[i+1].transform.GetComponentsInChildren<TMP_Text>();
                    for(int j=0; j<texts.Length;++j)
                    {
                        switch(texts[j].gameObject.name)
                        {
                            case "PlayerNick":
                            {
                                texts[j].text = deadPlayer[i].nickName;
                            }break;
                            case "Kill":
                            {
                                texts[j].text = p.GetScore().ToString();
                            }break;
                            case "Death":
                            {
                                texts[j].text = deadPlayer[i].stat.deathCount.ToString();
                            }break;
                            case "Rank":
                            {
                                texts[j].text = "패배";
                            }break;
                            
                        }

                    }
                }
            }
            
        }
        battleResultPanel.SetActive(true);
    }

    IEnumerator GameOver(){
        yield return new WaitForSeconds(3f);  
        
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            
            if(alivePlayer[0].photonView.Owner.ActorNumber == p.ActorNumber)
            {
                Debug.Log("alive: "+p.NickName+": "+p.GetScore()); 
                Debug.Log("alivePlayer[0]"+alivePlayer[0].nickName+": "+alivePlayer[0].stat.deathCount);   
            }
            for(int i=0; i<deadPlayer.Count;++i)
            {
 
                if(deadPlayer[i].photonView.Owner.ActorNumber == p.ActorNumber)
                {
                    Debug.Log("dead: "+p.NickName+": "+p.GetScore());
                    Debug.Log("deadPlayer[i]"+deadPlayer[i].nickName+": "+deadPlayer[i].stat.deathCount);
                }
            }
            
        }
        SetBattleResult();

        
        //SceneManager.LoadScene("NewLobbyScene");

    }
    //플레이어가 한 명 남았을 때 그라운드를 끝냄.

    public void FinalWinner(){
        //if(alivePlayer.Count == 1){
            //각 플레이어들에게 메시지를 보냄.
            //BattleOverMessage();
            Debug.Log("게임이 끝났습니다.");
            photonView.RPC("BattleOverMessage", RpcTarget.All);

        //}
    }
}
