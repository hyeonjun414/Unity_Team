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
        RaycastHit target;
        if(Physics.Raycast(player.transform.position + Vector3.up + transform.forward *0.5f, player.transform.forward, out target, 0.5f))
        {
            Character enemy = target.collider.gameObject.GetComponent<Character>();
            if (enemy != null)
            {
                print("Attack Enemy");
                enemy.photonView.RPC("Damaged", Photon.Pun.RpcTarget.All, player.stat.damage);
            }
        }
    }
    private void Block()
    {
        player.anim.SetTrigger("Right Punch Attack");
    }
    private void UseItem()
    {
        print("아이템 사용");
        player.playerInput = ePlayerInput.USE_ITEM;
        ItemManager.Instance.UseItem(player, ItemManager.Instance.itemList[0]);
        ItemManager.Instance.RemoveNum(ItemManager.Instance.itemList[0]);
    }
    private void ChangeItemSlot()
    {
        // Change Item Slot
    }
}
