using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum CamType
{
    Player,
    Dead,
}

public class CamManager : MonoBehaviour
{
    public static CamManager Instance = null;

    [Header("Virtual Cam")]
    public CinemachineVirtualCamera[] cams;

    [Header("Camera")]
    public Camera mainCam;
    public Camera miniMapCam;

    private void Awake()
    {
        Instance = this;
    }

    public void ActiveCam(CamType cam)
    {
        for(int i = 0; i < cams.Length; i++)
        {
            if(i == (int)cam)
            {
                cams[i].Priority = 1;
            }
            else
            {
                cams[i].Priority = 0;
            }
        }
    }

    public void FollowPlayerCam(Character player)
    {
        cams[(int)CamType.Player].Follow = player.camPos;
        cams[(int)CamType.Player].GetComponent<CameraTransparent>().SetPlayer(player);
    }

}
