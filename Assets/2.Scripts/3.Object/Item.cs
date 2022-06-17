using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData data;
    //public int holdingTime = 5;
   
    private void OnTriggerEnter(Collider other){

        //아이템과 충돌한 플레이어 알려주기
        if (other.gameObject.tag == "Player")
        {
            var contactedPlayer = other.gameObject.GetComponent<Character>();
            Debug.Log(contactedPlayer + "랑 충돌!");
           // contactedPlayer.AddItem(itemData)

            //플레이어와 충돌한 아이템 삭제
            Destroy(gameObject);
            //플레이어의 보관함에 아이템 넣어줌
            PickedUp(contactedPlayer);
        }
    }
    //아이템을 슬롯에 넣어줌
    private void PickedUp(Character player){
        if(ItemManager.Instance.AddNum(data)){
            Debug.Log(player.name + "가 " + data.name + "을 아이템 보관함에 넣었습니다!");
        }
        else{
            Debug.Log("아이템을 둘 자리가 없습니다!");

        }
    }
   























}
