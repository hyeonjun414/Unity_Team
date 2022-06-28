using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimeManager : Singleton<TimeManager>{

    //제한시간 타이머
    public float limitTime;
    private bool isGameOver;

    private void Awake()
    {
        if (_instance == null) _instance = this;

        isGameOver = false;
    }


    private void Update() {
        if (isGameOver) return;

        limitTime -= Time.deltaTime;
        
        if(BattleManager.Instance.mode == ModeType.TimeToKill){
            // 시간 갱신
            UIManager.Instance.topTextUI.UpdateUI();
            TimeOver();
        }
    }


    //시간이 0초가 되면 시간 세기를 멈춘다.
    public void TimeOver(){
        if(Mathf.Round(limitTime) <= 0){
            isGameOver = true;
            Debug.Log("제한 시간이 끝났습니다!");
            BattleManager.Instance.GameOver();
        }
    }
}
