using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Michsky.UI.ModernUIPack;

public class CreateRoomPanel : MonoBehaviour
{

    [Header("Entry")]
    public GameObject pwEntry;


    [Header("Input Field")]
    public CustomInputField roomName;
    public CustomInputField roomMaxPeople;
    public CustomInputField roomPassword;
    public CustomDropdown dropdownMode;
    public CustomDropdown dropdownMap;

    public Sprite dropdownItemIcon;

    private void Start()
    {
        dropdownMode.dropdownItems.Clear();
        dropdownMap.dropdownItems.Clear();
        for (int i = 0; i<(int)ModeType.End; i++)
        {
            dropdownMode.CreateNewItemFast(GameData.GetMode((ModeType)i), dropdownItemIcon);
        }
        dropdownMode.SetupDropdown();
        for (int i = 0; i < (int)MapType.End; i++)
        {
            dropdownMap.CreateNewItemFast(GameData.GetMap((MapType)i), dropdownItemIcon);
        }
        dropdownMap.SetupDropdown();
    }
    IEnumerator InitRoutine()
    {
        yield return null;
        roomName.inputText.text = $"{PhotonNetwork.LocalPlayer.NickName}의 방";
        roomName.UpdateState();
        roomMaxPeople.inputText.text = "4";
        roomMaxPeople.UpdateState();

        pwToggle.isOn = false;
        dropdownMode.selectedItemIndex = 0;
        dropdownMap.selectedItemIndex = 0;
    }

    private void OnEnable()
    {
        // 기본 값 셋팅
        StartCoroutine("InitRoutine");

    }
    

    [Header("Toggle")]
    public Toggle pwToggle;

    public void OnCreateRoomCancelButtonClicked()
    {
        LobbyManager.instance.SetActivePanel(LobbyManager.PANEL.Connect);
    }

    public void OnCreateRoomConfirmButtonClicked()
    {
        int roomNum = PhotonNetwork.CountOfRooms;
        roomNum++;

        RoomOptions options = new RoomOptions { MaxPlayers = (byte)int.Parse(roomMaxPeople.inputText.text) };
        options.CustomRoomProperties = new Hashtable {
            {GameData.ROOM_NAME, roomName.inputText.text },
            { GameData.ROOM_ISACTIVE_PW, pwToggle.isOn },
            { GameData.ROOM_PW, pwToggle ? roomPassword.inputText.text : "" },
            { GameData.GAME_MODE, dropdownMode.selectedItemIndex },
            { GameData.GAME_MAP, dropdownMap.selectedItemIndex }
        };
        options.CustomRoomPropertiesForLobby = new string[]
        {
            GameData.ROOM_NAME,
            GameData.ROOM_ISACTIVE_PW,
            GameData.ROOM_PW,
            GameData.GAME_MODE,
            GameData.GAME_MAP,
        };

        PhotonNetwork.CreateRoom(roomNum.ToString(), options, null);
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
