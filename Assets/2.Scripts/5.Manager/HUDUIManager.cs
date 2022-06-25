using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDUIManager : Singleton<HUDUIManager>
{ 
    private void Awake() {
        if (_instance == null){
            _instance = this;
        }
    }

    public GameObject timer;

   // public Character player;

    //public List<int> HPGroup  = new List<int>();

    
    public void DeathMatch(){
        timer.SetActive(false);
    }

    public void OnShotMatch(){
        timer.SetActive(false);

    }

    public void TimerMatch(){
        //시간 제한 모드에서는 시간이 모두 흐른 뒤 엔딩 씬으로 넘어간다
        //타이머 
        timer.SetActive(true);

    }



/*
    public void SetModeUI(BattleManager battleType){
       // battleType.SetUpMode

    }
*/


}
