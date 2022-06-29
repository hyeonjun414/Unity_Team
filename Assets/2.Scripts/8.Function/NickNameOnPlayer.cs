using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
    }
    private void Update()
    {
        nameText.transform.rotation = Quaternion.LookRotation(cam.transform.forward);
    }

    



    
}
