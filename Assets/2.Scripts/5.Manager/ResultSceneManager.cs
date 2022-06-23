using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ResultSceneManager : MonoBehaviourPun
{
    public GameObject resultWindow;

    // 승리한 플레이어 정보
    string[] winnerNic;
    // 패배한 플레이어 정보
    string[] loserNic;

    public CharacterData characterData;
    public GameObject characterSet;
    public int characterIndex;


    [Header("WINNER")]
    // 승리한 플레이어 닉네임 띄울 텍스트
    public Text[] winnerNicUIText;

    [Header("LOSER")]
    // 패배한 플레이어 닉네임 띄울 텍스트
    public Text[] loserNicUIText;

    private void Start()
    {
        WinnerInfomation();
        LoserInformation();

        PlayerAnimPlay();
    }

    // 승리자 닉네임 결과창 UI에 뜨도록 설정
    public void WinnerInfomation()
    {
        for (int i = 0; i < BattleManager.Instance.alivePlayer.Count; i++)
        {
            winnerNic[i] = BattleManager.Instance.alivePlayer[i].nickName;
            winnerNicUIText[i].text = winnerNic[i];
        }
    }

    // 패배자 닉네임 결과창 UI에 뜨도록 설정
    public void LoserInformation()
    {
        for (int i = 0; i < BattleManager.Instance.deadPlayer.Count; i++)
        {
            loserNic[i] = BattleManager.Instance.deadPlayer[i].nickName;
            loserNicUIText[i].text = loserNic[i];
        }
    }

    // 플레이어 승리, 패배 애니메이션 재생
    public void PlayerAnimPlay()
    {
        for (int i = 0; i < BattleManager.Instance.alivePlayer.Count; i++)
        {
            BattleManager.Instance.alivePlayer[i].anim.SetTrigger("Victory");
        }

        for (int i = 0; i < BattleManager.Instance.deadPlayer.Count; i++)
        {
            BattleManager.Instance.alivePlayer[i].anim.SetTrigger("Crying");
        }
    }





}
