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
    public PLInfoUI infoUI;

   // public Character player;
    public TMP_Text leftPLText;

    [Header ("InfoUI")]
    public GameObject timer;
    public GameObject leftPlayer;
    public GameObject playerInfo;

   

    private void Start() {
        leftPLText.text = PhotonNetwork.PlayerList.Length.ToString();

    }
    private void Awake() {
        if (_instance == null){
            _instance = this;
        }
    }

    private void Update() {
        infoUI.ShowDeathCount();
        infoUI.ShowKillCount();
        infoUI.ShowScore();
        LeftPlayers();
    }
    public void DeathMatch(){
        playerInfo.SetActive(true);
        timer.SetActive(false);
       
    }

    public void OnShotMatch(){
        playerInfo.SetActive(true);
        timer.SetActive(false);
    }

    public void TimerMatch(){
        //시간 제한 모드에서는 시간이 모두 흐른 뒤 엔딩 씬으로 넘어간다
        playerInfo.SetActive(true);
        timer.SetActive(true);
    }

    public void LeftPlayers(){
        leftPLText.text = BattleManager.Instance.alivePlayer.Count.ToString();

    }

}
