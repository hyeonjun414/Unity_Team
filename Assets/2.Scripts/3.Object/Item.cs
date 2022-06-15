using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) {

        //아이템과 충돌한 플레이어 알려주기
        if(other.gameObject.name == "Player"){
            var contactedPlayer = other.gameObject;
            Debug.Log(contactedPlayer + "랑 충돌!");
            //플레이어와 충돌한 아이템 삭제
            Destroy(gameObject);
        }
    }
    public ItemData data;

    //아이템의 타입
    enum Type { HEAL, POWERUP, DASH, SEEINGTHORUGH, };

    private void TestFunc() {
        Type itemType = Type.HEAL;

        if(itemType == Type.HEAL){
            //ItemManager의 HealPotion 함수 가져오기

        }
        else if(itemType == Type.POWERUP){
            //ItemManager의 PowerUpPotion 함수 가져오기

        }
        else if(itemType == Type.DASH){
            //ItemManager의 Dash 함수 가져오기

        }
        else if(itemType == Type.SEEINGTHORUGH){
            //ItemManager의 SeeingThrough 함수 가져오기

        }

    }





    



















}
