using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{ 
    public HPHeart hpHeart;
    public Character player;
    public List<HPHeart> HPs = new List<HPHeart>();

    

    private void Awake() {
        player = FindObjectOfType<Character>();
  
    }

    private void FixedUpdate() {
        if(player != null)
            HPUI();    
    }
    
    public void SetUp(Character player){
        this.player = player;
    }

    public int HPNum = 5;


    

    //HP UI 구현

    public void HPUI(){
        //Character의 HP 수에 따라 HPBar에 들어가는 Life의 개수가 달라진다
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
