using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class RoomSettingPanel : MonoBehaviour
{
    [Header("Game Mode")]
    public TMP_Text modeText;
    public Button modeLBtn;
    public Button modeRBtn;

    [Header("Map")]
    public TMP_Text mapText;
    public Button mapLBtn;
    public Button mapRBtn;

    [Header("Lock")]
    public GameObject lockImage;
    public GameObject unlockImage;

    [Header("Propertie Value")]
    Room        curRoom;
    public int  modeNum;
    public int  mapNum;

    public void SetUp()
    {
        curRoom = PhotonNetwork.CurrentRoom;
        modeNum = (int)curRoom.CustomProperties[GameData.GAME_MODE];
        mapNum = (int)curRoom.CustomProperties[GameData.GAME_MAP];
        modeText.text = GameData.GetMode((ModeType)modeNum);
        mapText.text = GameData.GetMap((MapType)mapNum);

        if(PhotonNetwork.IsMasterClient)
        {
            lockImage.SetActive(false);
            unlockImage.SetActive(true);
            ActiveBtn(true);
        }
        else
        {
            lockImage.SetActive(true);
            unlockImage.SetActive(false);
            ActiveBtn(false);
        }
    }

    public void ActiveBtn(bool active)
    {
        modeLBtn.gameObject.SetActive(active);
        modeRBtn.gameObject.SetActive(active);
        mapLBtn.gameObject.SetActive(active);
        mapRBtn.gameObject.SetActive(active);
    }

    public void ModeLBtnClick()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (modeNum == 0) modeNum = (int)ModeType.End;

        modeNum--;

        curRoom.SetCustomProperties(new Hashtable() {{ GameData.GAME_MODE, modeNum }});
    }
    public void ModeRBtnClick()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        modeNum++;

        if (modeNum == (int)ModeType.End) modeNum = 0;
        
        curRoom.SetCustomProperties(new Hashtable() { { GameData.GAME_MODE, modeNum } });
    }
    public void MapLBtnClick()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (mapNum == 0) mapNum = (int)MapType.End;

        mapNum--;

        curRoom.SetCustomProperties(new Hashtable() { { GameData.GAME_MAP, mapNum } });
    }
    public void MapRBtnClick()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        mapNum++;

        if (mapNum == (int)MapType.End) mapNum = 0;
        
        curRoom.SetCustomProperties(new Hashtable() { { GameData.GAME_MAP, mapNum } });
    }


    public void OnRoomPropertiesUpdate(Hashtable room)
    {
        object value;
        if(room.TryGetValue(GameData.GAME_MODE, out value))
        {
            modeText.text = GameData.GetMode((ModeType)value);
        }
        else if(room.TryGetValue(GameData.GAME_MAP, out value))
        {
            mapText.text = GameData.GetMap((MapType)value);
        }
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        if(PhotonNetwork.LocalPlayer == newMasterClient)
        {
            lockImage.SetActive(false);
            unlockImage.SetActive(true);
            ActiveBtn(true);
        }
        else
        {
            lockImage.SetActive(true);
            unlockImage.SetActive(false);
            ActiveBtn(false);
        }
    }
}
