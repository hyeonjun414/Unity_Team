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
        if(Physics.Raycast(player.transform.position + Vector3.up + transform.forward *0.5f, player.transform.forward, out target, 0.5f))
        {
            Character enemy = target.collider.gameObject.GetComponent<Character>();
            if (enemy != null)
            {
                // ������ ����ϰ� �ִ��� ���θ� Ȯ��
                if(EnemyDefenceCheck(enemy))
                {
                    print("����");
                    // ������ ����ϰ� �ִٸ� ������ �÷��̾� ���� ���·� ����
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
        // ���� ��밡 ��� ���¶��
        if(enemy.state == PlayerState.Defend)
        {
            // ����� ������ üũ
            return DefenceDirCheck(enemy);
        }
        // ��� ���� �ƴ϶�� ���� ����
        return false;
    }
    public bool DefenceDirCheck(Character enemy)
    {
        PlayerDir originDir = enemy.Dir;

        // �ݴ� �������� ����
        enemy.Dir++;
        enemy.Dir++;

        if (enemy.Dir == player.Dir)
        {
            // �����ϰ� �ִٸ� ���� ��ȿ
            enemy.Dir = originDir;
            return true;
        }
        else
        {
            // �����ϰ� ���� �ʴٸ� ���� ��ȿ
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
        print("������ ���");
        ItemManager.Instance.UseItem(player, ItemManager.Instance.itemList[0]);
        ItemManager.Instance.RemoveNum(ItemManager.Instance.itemList[0]);
    }
    private void ChangeItemSlot()
    {
        // Change Item Slot
    }
}
