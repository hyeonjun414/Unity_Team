using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
      private Character cplayer;


      //TODO: Collider 설치 후 함정 처리
      private void OnCollisionEnter(Collision other) {
         if(other.gameObject.name == "Player"){
            var contactedPlayer = other.gameObject;
            Debug.Log(contactedPlayer + "랑 충돌!");
            Destroy(gameObject);
        }
    }



   //가시 트랩: 해당 블럭 진입시 라이프 1개 감소
   public void ThornTrap(){
      //cplayer.hp = cplayer.hp - 1;

   }


   //얼음 트랩: 해당 블럭 진입시 한턴 동안 마비
   public void IceTrap(){



   }


}
