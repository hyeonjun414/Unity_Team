using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    ItemSlotUnit[] itemSlots;





    
    public void UpdateUI(){
        itemSlots = GetComponentsInChildren<ItemSlotUnit>();

        for(int i = 0; i< itemSlots.Length;i++){
            int count = ItemManager.Instance.itemList.Count;
            if(i < count){
                itemSlots[i].AddItem(ItemManager.Instance.itemList[i]);
            }
            else{
                itemSlots[i].ResetItem();
            }
        }

    }
}
