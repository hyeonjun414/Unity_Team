using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NewCreateRoomPanel : MonoBehaviour
{
    public TMP_InputField roomNameInputField;
    public TMP_InputField maxPlayersInputField;

    public void OnCreateRoomCancelButtonClicked()
    {
        NewLobbyManager.instance.SetActivePanel(NewLobbyManager.PANEL.Connect);
    }

    public void OnCreateRoomConfirmButtonClicked()
    {
        string roomName = roomNameInputField.text;

        if (roomName == "")
            roomName = "Room" + Random.Range(1000, 10000);

        byte maxPlayer = byte.Parse(maxPlayersInputField.text);
        maxPlayer = (byte)Mathf.Clamp(maxPlayer, 1, 15);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayer };
        PhotonNetwork.CreateRoom(roomName, options, null);
    }
}