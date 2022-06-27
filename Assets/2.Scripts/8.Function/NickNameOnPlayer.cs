using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
public class NickNameOnPlayer : MonoBehaviour
{
    TMP_Text nameText;
    private Camera cam;
    PhotonView photonView;
    
    public void SetNickName(string nickName)
    {
        nameText = GetComponent<TMP_Text>();
        nameText.text = nickName;
        cam = CamManager.Instance.mainCam;
        nameText.transform.rotation = Quaternion.LookRotation(cam.transform.forward);        
    }
    private void Update()
    {
        nameText.transform.rotation = Quaternion.LookRotation(cam.transform.forward);
    }

    



    
}
