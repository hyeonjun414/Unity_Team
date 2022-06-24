using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RegenUI : MonoBehaviour
{
    public TMP_Text regenText;
    public TMP_Text regenCountText;
    public int curTime;

    public void RegenStart(Character player)
    {
        gameObject.SetActive(true);
        if(player.photonView.IsMine)
            CamManager.Instance.ActiveCam(CamType.Map);
        curTime = 5;
        regenCountText.text = curTime.ToString();
        StartCoroutine("RegenRoutine", player);
    }

    IEnumerator RegenRoutine(Character player)
    {
        yield return null;

        while(curTime > 0)
        {
            --curTime;
            regenCountText.text = curTime.ToString();
            yield return new WaitForSeconds(1f);
        }
        gameObject.SetActive(false);
        TileNode emptyNode = MapManager.Instance.GetEmptyNode();
        player.photonView.RPC("Revive", Photon.Pun.RpcTarget.All, emptyNode.tilePos.y, emptyNode.tilePos.x);
    }

}
