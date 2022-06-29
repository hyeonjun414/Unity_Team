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
    public CustomInputField roomMaxPlayer;
    public CustomInputField roomPassword;
    public CustomDropdown dropdownMode;
    public CustomDropdown dropdownMap;


    [Header("Toggle")]
    public CustomToggle pwToggle;

    [Header("Dropdown Icon")]
    public Sprite dropdownItemIcon;

    private void Start()
    {
        dropdownMode.dropdownItems.Clear();
        dropdownMap.dropdownItems.Clear();
        for (int i = 0; i<(int)ModeType.End; i++)
        {
            dropdownMode.CreateNewItemFast(GameData.GetMode((ModeType)i), dropdownItemIcon);
        }
        for (int i = 0; i < (int)MapType.End; i++)
        {
            dropdownMap.CreateNewItemFast(GameData.GetMap((MapType)i), dropdownItemIcon);
        }
    }
    IEnumerator InitRoutine()
    {
        yield return null;

        roomName.inputText.text = $"{PhotonNetwork.LocalPlayer.NickName}의 방";
        roomMaxPlayer.inputText.text = "4";
        pwToggle.toggleObject.isOn = false;
        dropdownMode.selectedItemIndex = 0;
        dropdownMap.selectedItemIndex = 0;

        UpdateStateUI();
    }

    private void UpdateStateUI()
    {
        roomName.UpdateState();
        roomMaxPlayer.UpdateState();
        pwToggle.UpdateState();
        dropdownMode.SetupDropdown();
        dropdownMap.SetupDropdown();
    }

    private void OnEnable()
    {
        // 기본 값 셋팅
        StartCoroutine("InitRoutine");

    }
    


    public void OnCreateRoomCancelButtonClicked()
    {
        LobbyManager.instance.SetActivePanel(LobbyManager.PANEL.Connect);
    }

    public void OnCreateRoomConfirmButtonClicked()
    {
        int roomNum = Random.Range(1000, 10000);

        byte maxPlayer = byte.Parse(roomMaxPlayer.inputText.text);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayer };
        
        options.CustomRoomProperties = new Hashtable {
            {GameData.ROOM_NAME, roomName.inputText.text },
            { GameData.ROOM_ISACTIVE_PW, pwToggle.toggleObject.isOn },
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
        if (pwToggle.toggleObject.isOn == true)
        {
            pwEntry.SetActive(true);
            SetDefaultPw();
            roomPassword.UpdateState();
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
        if (roomPassword.inputText.text.Length == 0)
        {
            roomPassword.inputText.text = "0000";
        }
    }
}
