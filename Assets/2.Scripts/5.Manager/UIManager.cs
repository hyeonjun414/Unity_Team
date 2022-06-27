using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{

    [Header("HUD")]
    public PlayerStatusUI statusUI;
    public ItemSlotUI itemSlotUI;


    private void Awake()
    {
        if(_instance == null)
            _instance = this;
    }

}
