using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;



public class ResultEntry : MonoBehaviour
{
    public Player owner;
    public TMP_Text nickNameText;
    public TMP_Text killText;
    public TMP_Text deathText;
    public TMP_Text rankText;

    public void UpdateEntry(Character player)
    {
        nickNameText.text = player.nickName;
        killText.text = player.stat.killCount.ToString();
        deathText.text = player.stat.deathCount.ToString();
        rankText.text = player.stat.score.ToString();
        gameObject.SetActive(true);
    }

    public void ResetEntry()
    {
        gameObject.SetActive(false);
    }

    
}
