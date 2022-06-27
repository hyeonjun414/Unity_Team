using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    ItemSlotUnit[] itemSlots;

    private void Start()
    {
        itemSlots = GetComponentsInChildren<ItemSlotUnit>();
    }

    public void UpdateUI(){
        List<ItemData> list = ItemManager.Instance.itemList;

        for (int i = 0; i< itemSlots.Length;i++){
            int count = list.Count;
            if(i < count){
                itemSlots[i].AddItem(list[i]);
            }
            else{
                itemSlots[i].ResetItem();
            }
        }

    }
}
