using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
//using UnityEngine.Events;

public class CameraLock : MonoBehaviour
{


    public void EnableCamera(Character player)
    {
/*        GameObject[] objs = (GameObject.FindGameObjectsWithTag("Player"));
        for(int i=0; i<objs.Length;++i)
        {
            Character player = objs[i].GetComponent<Character>();
            if(player.photonView.IsMine)
            {
                transform.position = new Vector3(player.transform.position.x,player.transform.position.y+5f,player.transform.position.z);
                transform.SetParent(player.transform);
            }
        }*/

        if (player.photonView.IsMine)
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 5f, player.transform.position.z);
            transform.SetParent(player.transform);
        }

    }
}
