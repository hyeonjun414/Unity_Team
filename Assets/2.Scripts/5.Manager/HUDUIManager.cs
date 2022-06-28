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

   // public Character player;
    public TMP_Text leftPLText;

    [Header ("InfoUI")]
    public GameObject timer;
    public GameObject leftPlayer;
    public GameObject playerInfo;

   

    private void Awake() {
        if (_instance == null){
            _instance = this;
        }
    }


}
