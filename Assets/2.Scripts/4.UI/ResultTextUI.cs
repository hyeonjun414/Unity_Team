using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class ResultTextUI : MonoBehaviour
{
    public TMP_Text resultText;


    public void UpdateUI(ModeType mode)
    {
        switch(mode)
        {
            case ModeType.BattleRoyale:
            case ModeType.OneShot:
                SetBattleRoyaleResult();
                break;
            case ModeType.DeathMatch:
                SetDeathMatchResult();
                break;
        }
        gameObject.SetActive(true);
    }

    public void SetBattleRoyaleResult()
    {
        List<Character> pList = BattleManager.Instance.alivePlayer;
        string myNickName = PhotonNetwork.LocalPlayer.NickName;

        if (pList.Exists(player => player.nickName == myNickName))
        {
            resultText.text = "WIN!";
        }
        else
        {
            resultText.text = "LOSE";
        }
    }
    public void SetDeathMatchResult()
    {
        resultText.text = "TIME OVER!";
    }


}
