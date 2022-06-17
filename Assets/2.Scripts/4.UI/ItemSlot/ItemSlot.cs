using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//아이템을 먹으면 해당 아이템의 데이터를 식별하여 알맞는 이미지를 끼워줌


public class ItemSlot : MonoBehaviour
{

   //아이템이 들어오면 기본 이미지가 해당 아이템 이미지로 변경됨
   public Image defaultImage1;
   public Image defaultImage2;

   public Image itemImage;

 
    public void ShowIconImage(){
       // defaultImage1 = ItemManager.Instance.itemList[0].image;

    }


  

}