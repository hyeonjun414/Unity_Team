using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    private Character cplayer;


    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.name == "Player"){
            //var contactedPlayer = collision.gameObject;
            //Debug.Log(contactedPlayer + "랑 충돌!");

        }
    }


    public void PotionBasic(){
        //캐릭터가 포션을 먹으면 포션은 사라지도록 한다.
        

    }



    public void HPPotion(){
        //라이프가 2개 늘어남
        //cplayer.hp = 2 + cplayer.hp;
        //늘어난 라이프가 5개 이상일 경우 5개로 고정해줌
        //if(cplayer.hp >5){
        //    cplayer.hp = 5;
        //}


    }


    public void OffensePowerPotion(){
        //공격력이 2배로 증가


    }

    public void DashPotion(){
        //두 칸 이동 가능


    }

    public void SeeingThrough(){
        //벽 투시
        //오브젝트 알파값
    }







}
