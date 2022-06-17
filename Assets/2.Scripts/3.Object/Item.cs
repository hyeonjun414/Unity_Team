using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class Item : MonoBehaviour
{
    public int posX;
    public int posY;

    public ItemData data;
    public int holdingTime = 18;


    private void Start()
    {
        Invoke("ItemDestroy", holdingTime);
    }

    private void OnTriggerEnter(Collider other)
    {

        //아이템과 충돌한 플레이어 알려주기
        if (other.gameObject.tag == "Player")
        {
            var contactedPlayer = other.gameObject.GetComponent<Character>();
            Debug.Log(contactedPlayer + "랑 충돌!");
            // contactedPlayer.AddItem(itemData)

            //플레이어와 충돌한 아이템 삭제
            ItemDestroy();
            ItemManager.Instance.UseItem(contactedPlayer, data);
        }
    }

    private void ItemDestroy()
    {
        Destroy(gameObject);
        MapManager_verStatic.Instance.map.grid[MapManager_verStatic.Instance.map.mapSize * posX + posY].eOnTileObject = eTileOccupation.EMPTY;
        ItemSpawnManger_verStatic.Instance.curItemCount--;
    }


























}
