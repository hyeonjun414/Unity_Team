using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAction : ActionCommand
{
    public TileNode tileFront;
    public override void Execute()
    {
        Action();
    }
    public void Action()
    {
        if (player.eCurInput == ePlayerInput.ATTACK)
        {
            Attack();
        }
        else if (player.eCurInput == ePlayerInput.BLOCK)
        {
            Block();
        }
        else if (player.eCurInput == ePlayerInput.USE_ITEM)
        {
            UseItem();
        }
        else if (player.eCurInput == ePlayerInput.CHANGE_ITEM_SLOT)
        {
            ChangeItemSlot();
        }
    }
    private void Attack()
    {
        player.anim.SetTrigger("Right Punch Attack");
    }
    private void Block()
    {
        player.anim.SetTrigger("Right Punch Attack");
    }
    private void UseItem()
    {
        print("������ ���");
        player.playerInput = ePlayerInput.USE_ITEM;
        ItemManager.Instance.UseItem(player, ItemManager.Instance.itemList[0]);
        ItemManager.Instance.RemoveNum(ItemManager.Instance.itemList[0]);
    }
    private void ChangeItemSlot()
    {
        // Change Item Slot
    }
}
