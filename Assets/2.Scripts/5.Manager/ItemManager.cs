//아이템 매니저: 플레이어와 아이템이 충돌했을 때 아이템 데이터에 맞는 행동을 보내줌.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ItemManager : Singleton<ItemManager>
{
    private void Awake() {
        if (_instance == null){
            _instance = this;
        } 
    }

public void UseItem(Character player, ItemData data){
    switch(data.itemType){
        case ItemType.HEAL:
            HealPotion(player);
            break; 
        case ItemType.POWERUP:
            PowerUpPotion(player);
            break;
        case ItemType.DASH:
            DashItem(player);
            break;
        case ItemType.SEEINGTHORUGH:
            SeeingThrough(player);
            break;
    }
}

        public void HealPotion(Character player){
       
        //생명이 2개 늘어남
        player.characterStatus.hp = 2 + player.characterStatus.hp;

        //테스트용 디버그 로그
        Debug.Log("플레이어의 체력이 2 증가합니다.");

        //늘어난 라이프가 5개 이상일 경우 5개로 고정해줌
        if (player.characterStatus.hp > 5)
        {
            player.characterStatus.hp = 5;

            //테스트용 디버그 로그
            Debug.Log("플레이어의 체력이 이미 최대입니다!");
        }
        Debug.Log(player.characterStatus.hp);
    }


    public void PowerUpPotion(Character player){
        //공격력이 2배로 증가
        Debug.Log("공격력이 두 배로 증가합니다.");
    }

    public void DashItem(Character player){
        //같은 이동을 한 턴에 연속 두번 처리
        //이동 종류: 오른쪽 회전, 왼쪽 회전, 앞, 뒤, 좌, 우
        //앞뒤좌우 이동 시 같은 방향으로 2칸 이동, Vector로는 2씩 이동

        //현 아이템이 사용 되면
        Debug.Log("이동이 두 배로 증가합니다.");
    }

    public void SeeingThrough(Character player){
        //벽 투시
        //오브젝트 알파값
        Debug.Log("벽을 투시합니다.");
    }











}
