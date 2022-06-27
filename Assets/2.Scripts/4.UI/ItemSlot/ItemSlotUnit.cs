using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUnit : MonoBehaviour
{
    public GameObject slotContents;

    public Image itemImage;
    public TMP_Text itemName;
    public ItemData curItemData;

    public void AddItem(ItemData data){
        slotContents.SetActive(true);
        curItemData = data;
        itemName.text = data.name;
        itemImage.sprite = data.icon;
        itemImage.enabled = true;
    }

    public void ResetItem(){
        slotContents.SetActive(false);
        curItemData = null;
        itemName.text = "";
        itemImage.sprite = null;
        itemImage.enabled = false;
    }


}
