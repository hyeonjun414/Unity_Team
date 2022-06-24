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
    
    public void BattleResult(string nickName, int kill, int death, int rank ,string mode)
    {
        Text[] texts = (transform.GetChild(0)).GetComponentsInChildren<Text>();
        for(int i=0; i<texts.Length;++i)
        {
            switch(texts[i].gameObject.name)
            {
                case "Name":
                {
                    texts[i].text = nickName;
                }break;
                case "Kill":
                {
                    texts[i].text = kill.ToString();
                }break;
                case "Death":
                {
                    texts[i].text = death.ToString();
                }break;
                case "Rank":
                {
                    texts[i].text = rank.ToString();
                }break;   
            }
        }

        //모드에따라 다르게 넘어가게 
    }

    
}
