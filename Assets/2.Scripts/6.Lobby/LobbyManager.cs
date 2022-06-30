using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static LobbyManager instance { get; private set; }

    [Header("Panel")]
    public LoginPanel loginPanel;
    public InConnectPanel inConnectPanel;
    public CreateRoomPanel createRoomPanel;
    public InLobbyPanel inLobbyPanel;
    public InRoomPanel inRoomPanel;
    public InfoPanel infoPanel;


    #region UNITY

    private void Awake()
    {
        instance = this;
    }

private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.UseRpcMonoBehaviourCache = true;
        
        // 다시 로비씬으로 돌아왔을때 룸에 들어와있는 상태라면 룸을 켜준다.
        if(PhotonNetwork.CurrentRoom != null)
        {
            SetActivePanel(PANEL.Room);
            SoundManager.Instance.PlayerRoomBGM();
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }
    }


    public enum PANEL { Login, Connect, Lobby, Room, CreateRoom }
    public void SetActivePanel(PANEL panel)
    {
        loginPanel.gameObject.SetActive(panel == PANEL.Login);
        inConnectPanel.gameObject.SetActive(panel == PANEL.Connect);
        createRoomPanel.gameObject.SetActive(panel == PANEL.CreateRoom);
        inLobbyPanel.gameObject.SetActive(panel == PANEL.Lobby);
        inRoomPanel.gameObject.SetActive(panel == PANEL.Room);
    }

    public void ShowError(string error)
    {
        infoPanel.ShowError(error);
    }

    #endregion UNITY

    #region PHTON CALLBACK

    public override void OnConnectedToMaster()
    {
        SetActivePanel(PANEL.Connect);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        inLobbyPanel.OnRoomListUpdate(roomList);
        
    }

    public override void OnJoinedLobby()
    {
        inLobbyPanel.ClearRoomList();
    }

    public override void OnLeftLobby()
    {
        inLobbyPanel.ClearRoomList();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(PANEL.Connect);
        infoPanel.ShowError("Create Room Failed with Error(" + returnCode + ") : " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetActivePanel(PANEL.Connect);
        infoPanel.ShowError("Join Room Failed with Error(" + returnCode + ") : " + message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        int roomNum = PhotonNetwork.CountOfRooms;
        roomNum++;

        string roomName = $"{PhotonNetwork.LocalPlayer.NickName}의 방";
        RoomOptions options = new RoomOptions { MaxPlayers = 4 };
        options.CustomRoomProperties = new Hashtable {
            {GameData.ROOM_NAME, roomName },
            { GameData.ROOM_ISACTIVE_PW, false },
            { GameData.ROOM_PW, "" },
            { GameData.GAME_MODE, 0 },
            { GameData.GAME_MAP, 0 }
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

    public override void OnJoinedRoom()
    {
        SoundManager.Instance.PlayerRoomBGM();
        SetActivePanel(PANEL.Room);


    }

    public override void OnLeftRoom()
    {
        SoundManager.Instance.TitleRoomBGM();
        SetActivePanel(PANEL.Connect);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        inRoomPanel.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        inRoomPanel.OnPlayerLeftRoom(otherPlayer);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        inRoomPanel.OnMasterClientSwitched(newMasterClient);
        inRoomPanel.settingPanel.OnMasterClientSwitched(newMasterClient);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        inRoomPanel.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        inRoomPanel.settingPanel.OnRoomPropertiesUpdate(propertiesThatChanged);
    }


    public void LocalPlayerPropertiesUpdated()
    {
        inRoomPanel.LocalPlayerPropertiesUpdated();
    }
    public void ShowPlayerInfo(string UID)
    {
        inRoomPanel.ShowPlayerInfo(UID);
    }

    #endregion


    public void Send()
    {
        if (inRoomPanel.ChatInput.text == "") return;

        photonView.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + inRoomPanel.ChatInput.text);
        inRoomPanel.ChatInput.text = "";
        //inRoomPanel.ChatInput.Select();
    }

    [PunRPC]
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < inRoomPanel.ChatText.Length; i++)
            if (inRoomPanel.ChatText[i].text == "")
            {
                isInput = true;
                inRoomPanel.ChatText[i].text = msg;
                break;
            }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < inRoomPanel.ChatText.Length; i++) inRoomPanel.ChatText[i - 1].text = inRoomPanel.ChatText[i].text;
            inRoomPanel.ChatText[inRoomPanel.ChatText.Length - 1].text = msg;
        }
    }


}
