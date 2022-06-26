using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Michsky.UI.ModernUIPack;

public class RoomOptionUI : MonoBehaviour
{
    [Header("Entry")]
    public GameObject pwEntry;


    [Header("Input Field")]
    public CustomInputField roomName;
    public CustomInputField roomMaxPlayer;
    public CustomInputField roomPassword;

    [Header("Toggle")]
    public CustomToggle pwToggle;


    private void OnEnable()
    {
        StartCoroutine("UpdateUIRoutine");
    }

    IEnumerator UpdateUIRoutine()
    {
        yield return null;
        Room curRoom = PhotonNetwork.CurrentRoom;
        object value;

        roomMaxPlayer.inputText.text = curRoom.MaxPlayers.ToString();

        // 방 이름
        if (GetRoomOption(GameData.ROOM_NAME, curRoom, out value))
        {
            roomName.inputText.text = (string)value;
        }

        if (GetRoomOption(GameData.ROOM_ISACTIVE_PW, curRoom, out value))
        {
            pwToggle.toggleObject.isOn = (bool)value;
        }

        if(pwToggle.toggleObject.isOn == true)
        {
            pwEntry.SetActive(true);

            if(GetRoomOption(GameData.ROOM_PW, curRoom, out value))
            {
                roomPassword.inputText.text = (string)value;
            }
        }
        else
        {
            pwEntry.SetActive(false);
        }
        UpdateStateUI();
    }

    private void UpdateStateUI()
    {
        roomName.UpdateState();
        roomMaxPlayer.UpdateState();
        if(pwToggle.toggleObject.isOn)
            roomPassword.UpdateState();
        pwToggle.UpdateState();
    }

    public void OnClickApplyButton()
    {
        UpdateRoomOption();
        gameObject.SetActive(false);
    }
    public void OnClickCancelButton()
    {
        gameObject.SetActive(false);
    }

    public void UpdateRoomOption()
    {
        Room curRoom = PhotonNetwork.CurrentRoom;

        curRoom.MaxPlayers = byte.Parse(roomMaxPlayer.inputText.text);
   

        Hashtable option =  new Hashtable {
            { GameData.ROOM_NAME, roomName.inputText.text },
            { GameData.ROOM_ISACTIVE_PW, pwToggle.toggleObject.isOn },
            { GameData.ROOM_PW, roomPassword.inputText.text },
        };

        string[] props = new string[]
        {
            GameData.ROOM_NAME,
            GameData.ROOM_ISACTIVE_PW,
            GameData.ROOM_PW,
            GameData.GAME_MODE,
            GameData.GAME_MAP,
        };

        curRoom.SetCustomProperties(option);
        curRoom.SetPropertiesListedInLobby(props);
    }

    public bool GetRoomOption(string key, Room curRoom, out object value)
    {
        return curRoom.CustomProperties.TryGetValue(key, out value);
    }

    public void ChangePwToggle()
    {
        if (pwToggle.toggleObject.isOn == true)
        {
            pwEntry.SetActive(true);
            SetDefaultPw();
        }
        else
        {
            pwEntry.SetActive(false);
        }
    }

    public void MaxPlayerLimit()
    {
        int maxPlayer = int.Parse(roomMaxPlayer.inputText.text);

        maxPlayer = Mathf.Clamp(maxPlayer, 1, 4);

        roomMaxPlayer.inputText.text = maxPlayer.ToString();
    }
    public void SetDefaultPw()
    {
        if(roomPassword.inputText.text.Length == 0)
        {
            roomPassword.inputText.text = "0000";
        }
    }
}
