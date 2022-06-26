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
    public void SetNickName(string nickName)
    {
        nameText = GetComponent<TMP_Text>();
        nameText.text = nickName;
        cam = CamManager.Instance.mainCam;
        nameText.transform.rotation = Quaternion.LookRotation(cam.transform.forward);
        

        //플레이어 본인에게는 자신의 닉네임이 보이지 않도록 한다
       // if(PhotonView.)
        
    }
    private void Update()
    {
        nameText.transform.rotation = Quaternion.LookRotation(cam.transform.forward);
    }
}
