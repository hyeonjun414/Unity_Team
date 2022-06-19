using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAction : ActionCommand
{
    public TileNode tileFront;
    //public Character player;
    public override void Execute()
    {
        Attack();
        Block();
        UseItem();
        ChangeItemSlot();
    }
    private void Attack()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            player.anim.SetTrigger("Right Punch Attack");
            player.playerInput = ePlayerInput.ATTACK;
            
        }

    }
    private void Block()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            //?†ÎãàÎ©îÏù¥?òÏ∂îÍ∞Ä
            player.playerInput = ePlayerInput.BLOCK;
        }
        
   
    }
    private void UseItem()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            print("æ∆¿Ã≈€ ªÁøÎ");

            player.playerInput = ePlayerInput.USE_ITEM;
            ItemManager.Instance.UseItem(player, ItemManager.Instance.itemList[0]);
            ItemManager.Instance.RemoveNum(ItemManager.Instance.itemList[0]);

        }
    }
    private void ChangeItemSlot()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            //ItemPointing(!curItem);
            player.playerInput = ePlayerInput.CHANGE_ITEM_SLOT;
        }
    }
}
