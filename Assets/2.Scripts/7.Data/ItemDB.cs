using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemDB", menuName = "Data/ItemDB")]
public class ItemDB : ScriptableObject
{
    [Header("Item Data")]
    public ItemData[] itemList;
}
