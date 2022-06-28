using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("Player Name")]
    public TMP_Text playerNameText;

    [Header("HP Bar")]
    public HPBar hpBar;

    [Header("Player Score")]
    public PlayerInfoUI playerInfoUI;

    public void SetUp(Character player)
    {
        playerNameText.text = player.nickName;
        player.statusUI = this;
        hpBar.SetUp(player);
        playerInfoUI.SetUp(player);
    }

    public void UpdateStatusUI()
    {
        hpBar.HPUI();
        playerInfoUI.UpdateUI();
    }

}
