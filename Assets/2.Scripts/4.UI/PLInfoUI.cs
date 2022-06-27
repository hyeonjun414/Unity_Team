using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class PLInfoUI : MonoBehaviour
{

    public Character player;

    [Header("Text")]
    public TMP_Text nicknameText;
    public TMP_Text killNumText;
    public TMP_Text scoreText;
    public TMP_Text deathNumText;

    [Header("InfoUI")]
    public GameObject playerInfoUI;


    private void Start() {

        nicknameText.text = PhotonNetwork.LocalPlayer.NickName;
        killNumText.text = "0";
        scoreText.text = "0";
        deathNumText.text = "0";
    }

    private void Awake() {
        player = FindObjectOfType<Character>();
    }
    public void SetUp(Character player){
        this.player = player;
    }



    //플레이어의 부활 횟수를 세는 함수
    public void ShowDeathCount(){

        deathNumText.text = player.stat.deathCount.ToString();
    }

    public void ShowKillCount(){
        killNumText.text = player.stat.killCount.ToString();
    }

    public void ShowScore(){
        scoreText.text = player.stat.score.ToString();
    }










}
