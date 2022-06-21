//아이템 매니저: 플레이어와 아이템이 충돌했을 때 아이템 데이터에 맞는 행동을 보내줌.

//아이템은 1번 자리에 먼저, 그 다음 아이템을 먹었을 때 1번 자리에 아이템이 있다면 2번 자리에 넣어줌
//플레이어가 아이템을 사용하면 1번 먼저 사용되고, 사용하는 순간 2번 자리에 있던 아이템이 1번 자리로 감.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemManager : Singleton<ItemManager>
{
    public List<ItemData> itemList = new List<ItemData>();
    public ItemSlotUI itemSlotUI;
    public int maxCount = 2;

    private void Awake() {
        if (_instance == null){
            _instance = this;
        }
    }
    private void Update() {
        itemSlotUI.UpdateUI();
    }

    public void UseItem(Character player, ItemData data)
    {

        switch (data.itemType)
        {
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

    public void HealPotion(Character player)
    {

        //생명이 2개 늘어남
            player.stat.hp = 2 + player.stat.hp;

            //테스트용 디버그 로그
            Debug.Log(player.name + "의 체력이 2 증가합니다.");

            //늘어난 라이프가 5개 이상일 경우 5개로 고정해줌
            if (player.stat.hp > 5)
            {
                player.stat.hp = 5;

                //테스트용 디버그 로그
                Debug.Log(player.name + "의 체력이 이미 최대입니다!");
            }
            Debug.Log(player.stat.hp);
    }


    public void PowerUpPotion(Character player)
    {
        //공격력이 2배로 증가
        Debug.Log(player.name + "의 공격력이 두 배로 증가합니다.");
    }

    public void DashItem(Character player)
    {
        //같은 이동을 한 턴에 연속 두번 처리
        //이동 종류: 오른쪽 회전, 왼쪽 회전, 앞, 뒤, 좌, 우
        //앞뒤좌우 이동 시 같은 방향으로 2칸 이동, Vector로는 2씩 이동

        //현 아이템이 사용 되면
        Debug.Log(player.name + "의 이동이 두 배로 증가합니다.");
    }

    public void SeeingThrough(Character player)
    {
        //벽 투시
        //오브젝트 알파값
        Debug.Log(player.name + "가 벽을 투시합니다.");
    }


    public bool AddNum(ItemData item){
        if (itemList.Count >= maxCount){
            return false;
        }
        else{
            itemList.Add(item);
            return true;
        }
    }
    public void RemoveNum(ItemData item){
        itemList.Remove(item);
        itemSlotUI.UpdateUI();
    }

    public void ChangeItems(){
        //itemSlotUI.anim.SetTrigger("ChangeSlot");

        // 코루틴
        itemList.Reverse();
        Debug.Log("아이템 순서를 바꿉니다.");
    }

}
