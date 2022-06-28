using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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
            photonView.RPC("Attack", RpcTarget.All);
        }
        else if (player.eCurInput == ePlayerInput.BLOCK)
        {
            photonView.RPC("Block", RpcTarget.All);
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
    [PunRPC]
    public void Attack()
    {
        // 공격 애니메이션과 효과음을 재생.
        player.anim.SetTrigger("Attack");
        player.audioSource.PlayOneShot(player.attackMissSound);

        // 전방으로 레이캐스트를 실행하여 다른 캐릭터가 검출되면 해당 캐릭터에게 공격 로직을 실행
        RaycastHit target;
        if (Physics.Raycast(player.transform.position + Vector3.up + transform.forward * 0.5f, player.transform.forward, out target, 2f))
        {
            Character enemy = target.collider.gameObject.GetComponent<Character>();
            // 공격 로직
            if (enemy != null)
            {
                // 적이 방어 상태인지 체크
                if (EnemyDefenseCheck(enemy))
                {
                    // 방어가 유효하면 공격한 플레이어 스턴
                    Stunned(2f);
                }
                else
                {
                    // 방어가 유효하지 않다면 대상은 공격을 받는다.
                    enemy.Damaged(player.stat.damage);
                    player.stat.score += 5;
                    if (enemy.state == PlayerState.Dead)
                    {
                        player.stat.killCount++;
                        
                    }
                    player.UpdateStatus();
                }
            }
        }
    }
    public void ActionStop()
    {
        StopAllCoroutines();
    }

    [PunRPC]
    public void Stunned(float time)
    {
        StartCoroutine("StunRoutine", time);
    }
    IEnumerator StunRoutine(float time)
    {
        player.stunEffect.gameObject.SetActive(true);
        player.anim.SetBool("Stunned", true);
        player.state = PlayerState.Stun;

        yield return new WaitForSeconds(time);

        player.stunEffect.gameObject.SetActive(false);
        player.anim.SetBool("Stunned", false);
        player.state = PlayerState.Normal;
    }

    public bool EnemyDefenseCheck(Character enemy)
    {
        if (enemy.state == PlayerState.Defend)
        {
            return DefenseDirCheck(enemy);
        }
        return false;
    }
    public bool DefenseDirCheck(Character enemy)
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

    [PunRPC]
    public void Block()
    {
        player.shieldEffect.gameObject.SetActive(true);
        player.audioSource.PlayOneShot(player.shieldSound);

        StartCoroutine("BlockRoutine", 1f);
    }
    IEnumerator BlockRoutine(float time)
    {
        player.shieldEffect.gameObject.SetActive(true);
        player.state = PlayerState.Defend;
        player.anim.SetBool("Defend", true);

        yield return new WaitForSeconds(time);

        player.shieldEffect.gameObject.SetActive(false);
        player.state = PlayerState.Normal;
        player.anim.SetBool("Defend", false);
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
        ItemManager.Instance.SwitchItems();
    }
}
