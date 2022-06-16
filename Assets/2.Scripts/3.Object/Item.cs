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
    private ItemManager itemManager;

    private void Awake() {
        itemManager = GetComponent<ItemManager>();
     //   itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();
    }

    //아이템의 타입
    enum Type { HEAL, POWERUP, DASH, SEEINGTHORUGH, };

    //아이템을 먹었을 때의 효과
    private void ItemEffect() {
        Type itemType = Type.HEAL;

        if(itemType == Type.HEAL){
            //ItemManager의 HealPotion 함수 
            itemManager.HealPotion();


        }
        else if(itemType == Type.POWERUP){
            //ItemManager의 PowerUpPotion 함수 
            itemManager.PowerUpPotion();

        }
        else if(itemType == Type.DASH){
            //ItemManager의 Dash 함수 
            itemManager.DashItem();

        }
        else if(itemType == Type.SEEINGTHORUGH){
            //ItemManager의 SeeingThrough 함수 
            itemManager.SeeingThrough();

        }

    }





    



















}
