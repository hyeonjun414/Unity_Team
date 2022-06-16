using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class NewRoomEntry : MonoBehaviour
{
    public TMP_Text roomNameText;
    public TMP_Text roomPlayersText;
    public Button joinRoomButton;

    private string roomName;

    public void Initialize(string name, byte currentPlayers, byte maxPlayers)
    {
        roomName = name;

        roomNameText.text = name;
        roomPlayersText.text = currentPlayers + " / " + maxPlayers;

        joinRoomButton.enabled = currentPlayers < maxPlayers;
    }

    public void JoinRoom()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(roomName);
    }
}
