using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomEntry : MonoBehaviour
{
    public Image lockImage;
    public TMP_Text roomNumText;
    public TMP_Text roomNameText;
    public TMP_Text roomPlayersText;
    public TMP_Text roomGameMode;
    public TMP_Text roomGameMap;
    public Button joinRoomButton;
    public Button joinSecureRoomButton;
    public TMP_InputField roomPwInput;

    private string roomName;
    private string roomPw;
    private bool isLock;

    public void Initialize(string name, byte currentPlayers, byte maxPlayers)
    {
        roomName = name;

        roomNameText.text = name;
        roomPlayersText.text = currentPlayers + " / " + maxPlayers;

        joinRoomButton.enabled = currentPlayers < maxPlayers;
    }
    public void SetUp(RoomInfo room)
    {
        roomName = room.Name;
        object value;

        // 방 번호 여부
        roomNumText.text = roomName;

        // 방 패스워드 여부
        if (GetRoomOption(GameData.ROOM_ISACTIVE_PW, room, out value))
        {
            isLock = (bool)value;

            if(isLock)
            {
                lockImage.gameObject.SetActive(true);

                // 방 패스워드
                if(GetRoomOption(GameData.ROOM_PW, room, out value))
                {
                    roomPw = (string)value;
                }
            }
            else
            {
                lockImage.gameObject.SetActive(false);
            }
        }
        // 방 이름
        if (GetRoomOption(GameData.ROOM_NAME, room, out value))
        {
            roomNameText.text = (string)value;
        }

        // 방 모드
        if(GetRoomOption(GameData.GAME_MODE, room, out value))
        {
            roomGameMode.text = GameData.GetMode((ModeType)value);
        }

        // 방 맵
        if (GetRoomOption(GameData.GAME_MAP, room, out value))
        {
            roomGameMap.text = GameData.GetMap((MapType)value);
        }

        roomPlayersText.text = room.PlayerCount + " / " + room.MaxPlayers;

        joinRoomButton.enabled = room.PlayerCount < room.MaxPlayers;
    }

    private bool GetRoomOption(string key, RoomInfo curRoom, out object value)
    {
        return curRoom.CustomProperties.TryGetValue(key, out value);
    }

    public void JoinRoom()
    {
        if(isLock)
        {
            joinSecureRoomButton.gameObject.SetActive(true);
            roomPwInput.gameObject.SetActive(true);
            return;
        }

        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(roomName);
    }

    public void JoinSecureRoom()
    {
        if(roomPwInput.text == roomPw)
        {
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinRoom(roomName);
        }
        else
        {
            roomPwInput.text = "";
            joinSecureRoomButton.gameObject.SetActive(false);
            roomPwInput.gameObject.SetActive(false);
        }
    }
}
