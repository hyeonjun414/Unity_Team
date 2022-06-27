using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{ 
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public Character player;
    public Image[] hpImages;

    public void SetUp(Character player){
        this.player = player;
    }

    public void HPUI(){
        //Character의 HP 수에 따라 HPBar에 들어가는 Life의 개수가 달라진다
        for(int i = 0; i< hpImages.Length; i++){
            if(i < player.stat.hp)
                hpImages[i].sprite = fullHeart;
            else
            {
                hpImages[i].sprite = emptyHeart;
            }
        }
        
    }





}
