using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class HUDUIManager : Singleton<HUDUIManager>
{

    public Character player;
    public GameObject regenNum;

    [Header ("Text")]

    public TMP_Text nicknameText;
    public TMP_Text killNumText;
    public TMP_Text scoreText;
    public TMP_Text leftPLText;
    public TMP_Text regenNumText;

    [Header ("InfoUI")]
    public GameObject playreInfo;
  //  public GameObject playerInfoTimer;
    public GameObject timer;



    private void Start() {

        nicknameText.text = photonView.Owner.NickName;
        killNumText.text = "0";
        scoreText.text = "0";
     //   leftPLText.text = (시작화면에서 설정한 플레이어의 수)
        regenNumText.text = "0";
    }

    private void Awake() {

        if (_instance == null){
            _instance = this;
        }
  
    }

    private void Update() {
        ShowRegenCount();
        ShowKillCount();
        ShowScore();
        ShowLeftPlayers();
    }
    public void DeathMatch(){
        playreInfo.SetActive(true);
        timer.SetActive(false);
        regenNum.SetActive(false);
       
    }

    public void OnShotMatch(){
        playreInfo.SetActive(true);
        timer.SetActive(false);
        regenNum.SetActive(false);
        
    }

    public void TimerMatch(){
        //시간 제한 모드에서는 시간이 모두 흐른 뒤 엔딩 씬으로 넘어간다
        playreInfo.SetActive(true);
        timer.SetActive(true);
        regenNum.SetActive(true);
       
    }

    //플레이어의 부활 횟수를 세는 함수
    public void ShowRegenCount(){

       foreach(Player p in PhotonNetwork.PlayerList){
        
           for(int i=0; i<BattleManager.Instance.players.Count;++i)
            {
                if(photonView.Owner.ActorNumber == p.ActorNumber)
                {
                    regenNumText.text = BattleManager.Instance.players[i].stat.deathCount.ToString();
                }       
            }     
            
       }
    }

    public void ShowKillCount(){
         foreach(Player p in PhotonNetwork.PlayerList){
        
            for(int i=0; i<BattleManager.Instance.players.Count;++i)
                {
                   
                    if(photonView.Owner.ActorNumber == p.ActorNumber)
                    {
                        killNumText.text = BattleManager.Instance.players[i].stat.killCount.ToString();

                    }       
                }     
            
       }
    }

    public void ShowScore(){
       foreach(Player p in PhotonNetwork.PlayerList){
        
           for(int i=0; i<BattleManager.Instance.players.Count;++i)
            {
                if(BattleManager.Instance.players[i].photonView.Owner.ActorNumber == p.ActorNumber)
                {
                    scoreText.text = p.GetScore().ToString();
                }       
            }     
            
       }
    }


    public void ShowLeftPlayers(){

    }


}
