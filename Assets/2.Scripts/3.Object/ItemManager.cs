using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private characterStatus cplayer;

        public void HealPotion(){
        //라이프가 2개 늘어남
        cplayer.hp = 2 + cplayer.hp;
        //늘어난 라이프가 5개 이상일 경우 5개로 고정해줌
        if(cplayer.hp > 5){
            cplayer.hp = 5;
        }
    }


    public void PowerUpPotion(){
        //공격력이 2배로 증가

    }

    public void DashItem(){
        //같은 이동을 한 턴에 연속 두번 처리

    }

    public void SeeingThrough(){
        //벽 투시
        //오브젝트 알파값
    }


    public void ItemFunction(){

    }








   
}
