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
        itemImage.sprite = data.icon;
        itemImage.enabled = true;
    }

    public void ResetItem(){
        curItemData = null;
        itemImage.sprite = null;
        itemImage.enabled = false;
    }

    public void ChangeItem(ItemData data1, ItemData data2){




    }

}
