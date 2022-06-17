using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType { HEAL, POWERUP, DASH, SEEINGTHORUGH, };


[CreateAssetMenu(fileName = "ItemData", menuName = "Item/ItemData")]

public class ItemData : ScriptableObject 
{

    new public string name;
    public string Description;
    public ItemType itemType;
    


    [Header("Item UI")]
    public Sprite icon;
    public Image image;
    public Item prefab;


  //  [Header("Item Function")]

   








}
