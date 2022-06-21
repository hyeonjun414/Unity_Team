using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
public class NickNameOnPlayer : MonoBehaviour
{
    TMP_Text nameText;
    public void SetNickName(string nickName , CinemachineVirtualCamera virtualCamera)
    {
        nameText = GetComponent<TMP_Text>();
        nameText.text = nickName;
        nameText.transform.rotation = Quaternion.LookRotation(virtualCamera.transform.forward);
        
    }
}
