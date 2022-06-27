﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimeManager : Singleton<TimeManager>{

    private void Awake()
    {
        if (_instance == null) _instance = this;
        
    }

    //제한시간 타이머
    public float limitTime;
    public Text timer;


    private void Update() {
        limitTime -= Time.deltaTime;
        //소수점을 제외하여 간단하게 표시
        timer.text = "Time: " + Mathf.Round(limitTime);


        if(BattleManager.Instance.mode == ModeType.TimeToKill){
            TimeOver();

        }
        else{

        }

    }


    //시간이 0초가 되면 시간 세기를 멈춘다.
    public void TimeOver(){
        if(Mathf.Round(limitTime) <= 0){
            timer.text = "TIME OVER!";
            Debug.Log("제한 시간이 끝났습니다!");
        //    StartCoroutine("GoToResult");

        }
    }


/*
    IEnumerator GoToResult(){
        yield return new WaitForSeconds(3f);
        Debug.Log("엔딩화면으로 돌아갑니다.");
        SceneManager.LoadScene("Result");

    }
   
   */
}
