using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviourPun
{
    [Header("Player")]
    public List<Character> players;
    //살아있는 플레이어 
    public List<Character> alivePlayer;
    //사망한 플레이어
    public List<Character> deadPlayer;

    [Header("Text")]
    public Text resultText;
    public GameObject resultTextObj;

    [Header("UI")]
    public RegenUI regenUI;
  
    public static BattleManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;

    }

    public void SetUpMode()
    {
        object mode;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameData.GAME_MODE, out mode))
        {
            int modeNum = (int)mode;
            switch (modeNum)
            {
                case 0:
                    SetUpDeathMatch();
                    break;
                case 1:
                    SetUpDeathMatch();
                    break;
                case 2:
                    SetUpTimerMode();
                    break;
            }
        }
    }

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

    }

    //플레이어가 죽었을 때 판정
    public void PlayerOut(Character deadPL)
    {

        //alivePlayer 리스트에서 죽은 플레이어를 뺀다.
        alivePlayer.Remove(deadPL);
        //deadPlayer 리스트에 죽은 플레이어를 더해준다.
        deadPlayer.Add(deadPL);
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
    
    

    //플레이어가 한 명 남았을 때 그라운드를 끝냄.

    public void FinalWinner(){
        if(alivePlayer.Count == 1 && PhotonNetwork.IsMasterClient){
            
            //각 플레이어들에게 메시지를 보냄.
            //BattleOverMessage();
            Debug.Log("게임이 끝났습니다.");
            photonView.RPC("BattleOverMessage", RpcTarget.All);
        }
    }



}
