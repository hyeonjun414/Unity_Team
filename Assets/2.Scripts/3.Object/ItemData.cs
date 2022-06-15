using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemData", menuName = "Item/ItemData")]

public class ItemData : ScriptableObject 
{
    public enum Type { HEAL, POWERUP, DASH, SEEINGTHORUGH, };

    new public string name;
    public string Description;

    


    

    [Header("Item UI")]
    public Sprite icon;
    public Item prefab;


    [Header("Item Function")]
    public bool Heal;

   








}
