﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
//using UnityEngine.Events;

public class CameraLock : MonoBehaviour
{


    public void EnableCamera(Character player)
    {
        if (player.photonView.IsMine)
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 20f, player.transform.position.z);
            transform.SetParent(player.transform);
        }
    }
}
