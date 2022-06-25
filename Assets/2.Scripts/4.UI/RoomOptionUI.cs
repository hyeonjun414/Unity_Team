
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class RoomOptionUI : MonoBehaviour
{
    [Header("Entry")]
    public GameObject pwEntry;


    [Header("Input Field")]
    public TMP_InputField roomName;
    public TMP_InputField roomMaxPeople;
    public TMP_InputField roomPassword;

    [Header("Toggle")]
    public Toggle pwToggle;


    private void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        Room curRoom = PhotonNetwork.CurrentRoom;
        object value;

        roomMaxPeople.text = curRoom.MaxPlayers.ToString();

        // 방 이름
        if (GetRoomOption(GameData.ROOM_NAME, curRoom, out value))
        {
            roomName.text = (string)value;
        }

        if (GetRoomOption(GameData.ROOM_ISACTIVE_PW, curRoom, out value))
        {
            pwToggle.isOn = (bool)value;
        }

        if(pwToggle.isOn == true)
        {
            pwEntry.SetActive(true);

            if(GetRoomOption(GameData.ROOM_PW, curRoom, out value))
            {
                roomPassword.text = (string)value;
            }
        }
        else
        {
            pwEntry.SetActive(false);
        }
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

        curRoom.MaxPlayers = (byte)int.Parse(roomMaxPeople.text);
   

        Hashtable option =  new Hashtable {
            { GameData.ROOM_NAME, roomName.text },
            { GameData.ROOM_ISACTIVE_PW, pwToggle.isOn },
            { GameData.ROOM_PW, roomPassword.text },
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
        if (pwToggle.isOn == true)
        {
            pwEntry.SetActive(true);
        }
        else
        {
            pwEntry.SetActive(false);
        }
    }
}
