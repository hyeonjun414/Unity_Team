using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDUI : MonoBehaviour
{ 
    public HPHeart hpHeart;
   
    public GameObject timer;

    public Character player;

    //public List<int> HPGroup  = new List<int>();
    public List<HPHeart> HPs = new List<HPHeart>();

    
    private void Start() {
        
    }


    private void Awake() {
        player = FindObjectOfType<Character>();
  
    }

    private void FixedUpdate() {
        if(player != null)
            HPUI();    
    }
    private void Update() {
       // HPNum = player.stat.hp;
    }

    public void SetUp(Character player){
        this.player = player;
    }

    public int HPNum = 5;


    public void DeathMatch(){
        //체력 바 
        timer.SetActive(false);


    }

    public void OnShotMatch(){

        timer.SetActive(false);

    }

    public void TimerMatch(){
        //시간 제한 모드에서는 시간이 모두 흐른 뒤 엔딩 씬으로 넘어간다

        //타이머 
        timer.SetActive(true);
        TimeManager.Instance.TimeOver();


        

    }


    //HP UI 구현

    public void HPUI(){

        //Character의 HP 수에 따라 HPBar에 들어가는 Life의 개수가 달라진다
        //player.stat.hp만큼
    //   HPs.heart.sprite = hpHeart.attackedHeart;

        for(int i = 0; i<HPs.Count; i++){
            if(i < player.stat.hp)
                HPs[i].heart.sprite = hpHeart.fullHeart;
            else
            {
                HPs[i].heart.sprite = hpHeart.attackedHeart;
            }
        }
        
    }





}
