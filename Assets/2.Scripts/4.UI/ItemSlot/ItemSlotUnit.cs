using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUnit : MonoBehaviour
{

    public Image itemImage;
    //public Sprite itemIcon;
    public ItemData curItemData;

    public void AddItem(ItemData data){
        curItemData = data;
        itemImage = data.image;
    }

    public void ResetItem(){
        curItemData = null;
        itemImage = null;
    }

}
