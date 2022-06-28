using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class PlayerInfoUI : MonoBehaviour
{

    public Character player;

    [Header("Text")]
    public TMP_Text killNumText;
    public TMP_Text scoreText;
    public TMP_Text deathNumText;

    public void SetUp(Character player){
        this.player = player;
    }

    public void UpdateUI()
    {
        deathNumText.text = player.stat.deathCount.ToString();
        killNumText.text = player.stat.killCount.ToString();
        scoreText.text = player.stat.score.ToString();
    }
}
