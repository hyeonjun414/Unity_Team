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
                // 상대방이 방어하고 있는지 여부를 확인
                if(EnemyDefenceCheck(enemy))
                {
                    print("스턴");
                    // 상대방이 방어하고 있다면 공격한 플레이어 스턴 상태로 변경
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
        // 만약 상대가 방어 상태라면
        if(enemy.state == PlayerState.Defend)
        {
            // 방어의 방향을 체크
            return DefenceDirCheck(enemy);
        }
        // 방어 상태 아니라면 공격 진행
        return false;
    }
    public bool DefenceDirCheck(Character enemy)
    {
        PlayerDir originDir = enemy.Dir;

        // 반대 방향으로 돌림
        enemy.Dir++;
        enemy.Dir++;

        if (enemy.Dir == player.Dir)
        {
            // 마주하고 있다면 공격 무효
            enemy.Dir = originDir;
            return true;
        }
        else
        {
            // 마주하고 있지 않다면 공격 유효
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
        print("아이템 사용");
        ItemManager.Instance.UseItem(player, ItemManager.Instance.itemList[0]);
        ItemManager.Instance.RemoveNum(ItemManager.Instance.itemList[0]);
    }
    private void ChangeItemSlot()
    {
        // Change Item Slot
    }
}
