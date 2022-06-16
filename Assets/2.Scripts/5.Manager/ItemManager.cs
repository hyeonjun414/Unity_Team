using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ItemManager : Singleton<ItemManager>
{
    private Character character;
    // private CharacterMove characterMove;


    private void Awake()
    {

    }

    public void UseItem(Character player, ItemData data)
    {
        switch (data.itemType)
        {
            case ItemType.HEAL:
                HealPotion(player);
                break;

        }

    }

    public void HealPotion(Character player)
    {

        //라이프가 2개 늘어남
        player.characterStatus.hp = 2 + player.characterStatus.hp;
        //늘어난 라이프가 5개 이상일 경우 5개로 고정해줌
        if (player.characterStatus.hp > 5)
        {
            player.characterStatus.hp = 5;

        }
    }


    public void PowerUpPotion()
    {
        //공격력이 2배로 증가




    }

    public void DashItem()
    {
        //같은 이동을 한 턴에 연속 두번 처리
        //이동 종류: 오른쪽 회전, 왼쪽 회전, 앞, 뒤, 좌, 우
        //앞뒤좌우 이동 시 같은 방향으로 2칸 이동, Vector로는 2씩 이동

        //현 아이템이 사용 되면


    }

    public void SeeingThrough()
    {
        //벽 투시
        //오브젝트 알파값
    }











}
