using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
   public Sprite defaltImage;

   //슬롯에 자리가 있는가?
   //public bool extraSlot = true;
   public Stack<Item> itemSlot;
   public ItemData data;

   private void Start() {
        itemSlot = new Stack<Item>();
   }

   
    //플레이어와 아이템이 충돌할 경우(플레이어가 아이템을 먹을 경우) 슬롯에 넣어준다.
   public void GetItem(Item item){

    itemSlot.Push(item);


        //슬롯이 꽉 찬 경우
        if(itemSlot.Count >= 2){
            Debug.Log("아이템 슬롯이 꽉 찼습니다!");
        }
        else{
            //아이템 슬롯의 개수가 1 증가하고, 해당 아이템의 이미지가 슬롯으로 들어온다.

        }
   }


    //플레이어 측에서 아이템을 사용함
   public void ItemUsed(){
        if(itemSlot.Count == 0){
            Debug.Log("슬롯에 아이템이 없습니다!");
        }
        else{
            //슬롯에서 아이템 하나가 빠짐

        }

   }


   public void itemComes(){
    //1번 자리의 아이템을 사용하면 2번 아이템이 1번 자리로
   }

   
}
