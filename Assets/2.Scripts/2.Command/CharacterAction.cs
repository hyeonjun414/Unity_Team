using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAction : ActionCommand
{
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

        RaycastHit target;
        if (Physics.Raycast(player.transform.position + Vector3.up + transform.forward * 0.5f, player.transform.forward, out target, 0.5f))
        {
            Character enemy = target.collider.gameObject.GetComponent<Character>();
            if (enemy != null)
            {
                if (EnemyDefenceCheck(enemy))
                {
                    player.photonView.RPC("Stunned", Photon.Pun.RpcTarget.All);
                    return;
                }
                player.anim.SetTrigger("Right Punch Attack");
                print("Attack Enemy");
                enemy.photonView.RPC("Damaged", Photon.Pun.RpcTarget.All, player.stat.damage);
            }
        }
    }

    public bool EnemyDefenceCheck(Character enemy)
    {
        if (enemy.state == PlayerState.Defend)
        {
            return DefenceDirCheck(enemy);
        }
        return false;
    }
    public bool DefenceDirCheck(Character enemy)
    {
        PlayerDir originDir = enemy.Dir;

        enemy.Dir++;
        enemy.Dir++;

        if (enemy.Dir == player.Dir)
        {
            enemy.Dir = originDir;
            return true;
        }
        else
        {
            enemy.Dir = originDir;
            return false;
        }
    }

    private void Block()
    {
        player.photonView.RPC("Block", Photon.Pun.RpcTarget.All);

    }
    private void UseItem()
    {
        if (ItemManager.Instance.itemList.Count == 0)
        {
            Debug.Log("사용할 아이템이 없습니다!");
        }
        else
        {
            player.eCurInput = ePlayerInput.USE_ITEM;
            ItemManager.Instance.UseItem(player, ItemManager.Instance.itemList[0]);
            ItemManager.Instance.RemoveNum(ItemManager.Instance.itemList[0]);

        }
    }
    private void ChangeItemSlot()
    {
        player.eCurInput = ePlayerInput.CHANGE_ITEM_SLOT;
        ItemManager.Instance.ChangeItems();
    }
}
