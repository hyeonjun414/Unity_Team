using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ItemManager : Singleton<ItemManager>
{
    private Character character;
   // private CharacterMove characterMove;
    

    private void Awake() {
        
    }

        public void HealPotion(){
        //라이프가 2개 늘어남
        character.characterStatus.hp = 2 + character.characterStatus.hp;
        //늘어난 라이프가 5개 이상일 경우 5개로 고정해줌
        if(character.characterStatus.hp > 5){
            character.characterStatus.hp = 5;

        }
    }


    public void PowerUpPotion(){
        //공격력이 2배로 증가




    }

    public void DashItem(){
        //같은 이동을 한 턴에 연속 두번 처리
        //이동 종류: 오른쪽 회전, 왼쪽 회전, 앞, 뒤, 좌, 우
        //회전 시 방향과 상관 없이 180도 회전
        //앞뒤좌우 이동 시 같은 방향으로 2칸 이동, Vector로는 2씩 이동

        //현 아이템이 사용 되면
/*
        //이동
        if(왼쪽일 때){
            //왼쪽 2번
        }
        else if(오른쪽일 때){
            //오른쪽 2번
        }
        else if(앞쪽일 때){
            //앞으로 2번
        }
        else if(뒤쪽일 때){
            //뒤로 2번
        }
        else if(왼쪽 회전이거나 || 오른쪽 회전일 때){
            //180도 회전
        }        

*/

    }

    public void SeeingThrough(){
        //벽 투시
        //오브젝트 알파값
    }


    







   
}
