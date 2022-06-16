using UnityEngine;
using Photon.Pun;

public class NewInConnectPanel : MonoBehaviour
{
    public void OnCreateRoomButtonClicked()
    {
        NewLobbyManager.instance.SetActivePanel(NewLobbyManager.PANEL.CreateRoom);
    }

    public void OnRandomMatchingButtonClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnLobbyButtonClicked()
    {
        PhotonNetwork.JoinLobby();
        NewLobbyManager.instance.SetActivePanel(NewLobbyManager.PANEL.Lobby);
    }

    public void OnLogoutButtonClicked()
    {
        PhotonNetwork.Disconnect();
        NewLobbyManager.instance.SetActivePanel(NewLobbyManager.PANEL.Login);
    }
}
