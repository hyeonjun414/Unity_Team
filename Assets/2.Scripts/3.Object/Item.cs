using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData data;
    public float holdingTime = 5f;

    private void OnCollisionEnter(Collision other)
    {

        //아이템과 충돌한 플레이어 알려주기
        if (other.gameObject.tag == "Player")
        {
            var contactedPlayer = other.gameObject.GetComponent<Character>();
            Debug.Log(contactedPlayer + "랑 충돌!");
            //플레이어와 충돌한 아이템 삭제
            // contactedPlayer.AddItem(itemData)
            //Destroy(gameObject);
        }
    }


























}
