using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAction : ActionCommand
{
    public TileNode tileFront;
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
            //애니메이션추가
            player.playerInput = ePlayerInput.BLOCK;
            

        }
        
   
    }
    private void UseItem()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            player.playerInput = ePlayerInput.USE_ITEM;
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
