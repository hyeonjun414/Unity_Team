using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class MainPanel : MonoBehaviourPunCallbacks
{
    [Header("Login")]
    public GameObject loginPanel; // �α��� �г�

    public InputField playerNameInput; // �÷��̾� ���ӽ����̽�

    [Header("Connect")]
    public GameObject inConnectPanel; // ������ �г�

    [Header("Create Room")]
    public GameObject createRoomPanel; // �� ���� �г�

    public InputField roomNameInputField; // �� �̸� ���ӽ����̽�
    public InputField maxPlayersInputField; // �ִ� �ο� ���ӽ����̽�

    [Header("Lobby")]
    public GameObject inLobbyPanel; // �κ� �г�

    public GameObject roomContent;
    public GameObject roomEntryPrefab;

    [Header("Room")]
    public GameObject inRoomPanel;

    public GameObject playerListContent;
    public Button startGameButton;
    public GameObject playerEntryPrefab;


    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;

    

    #region UNITY

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();

        playerNameInput.text = "Player " + Random.Range(1000, 10000);
    }

    public enum PANEL { Login, Connect, Lobby, Room, CreateRoom }
    private void SetActivePanel(PANEL panel)
    {
        // ������ �г��� Ȱ��ȭ �ϴ� �Լ�

        loginPanel.SetActive(panel == PANEL.Login);
        inConnectPanel.SetActive(panel == PANEL.Connect);
        createRoomPanel.SetActive(panel == PANEL.CreateRoom);
        inLobbyPanel.SetActive(panel == PANEL.Lobby);
        inRoomPanel.SetActive(panel == PANEL.Room);
    }

    public void OnLoginButtonClicked()
    {
        // �α��� �г��� ��ư�� ������ �� �۵��ϴ� �Լ�

        string playerName = playerNameInput.text;

        if (playerName == "")
        {
            Debug.LogError("Invalid Player Name");
            return;
        }
        // �Էµ� �÷��̾� ���ӽ����̽��� �ؽ�Ʈ�� �г������� �����Ѵ�.
        PhotonNetwork.LocalPlayer.NickName = playerName;
        // �ش� �������� ������ �Ѵ�.
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnCreateRoomButtonClicked()
    {
        // �� ���� ��ư�� ������ �� �۵��ϴ� �Լ�

        // �� ���� �г��� Ȱ��ȭ.
        SetActivePanel(PANEL.CreateRoom);
    }

    public void OnRandomMatchingButtonClicked()
    {
        // ���� ��Ī ��ư�� ������ �� �۵��ϴ� �Լ�

        // ������ �濡 ��.
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnLobbyButtonClicked()
    {
        // �κ� ��ư�� ����� �Լ�

        // �κ� �����ϴ� �Լ�.
        PhotonNetwork.JoinLobby();
    }

    public void OnLogoutButtonClicked()
    {
        // �α׾ƿ� ��ư�� ����� �Լ�

        // �������� ������ ������.
        PhotonNetwork.Disconnect();
    }

    public void OnCreateRoomCancelButtonClicked()
    {
        // �� ���� ��� ��ư�� ����� �Լ�

        // ���� �г� Ȱ��ȭ
        SetActivePanel(PANEL.Connect);
    }

    public void OnCreateRoomConfirmButtonClicked()
    {
        // �� ���� Ȯ�� ��ư�� ����� �Լ�

        
        string roomName = roomNameInputField.text;
        
        if (roomName == "")
            roomName = "Room" + Random.Range(1000, 10000);

        byte maxPlayer = byte.Parse(maxPlayersInputField.text);
        maxPlayer = (byte) Mathf.Clamp(maxPlayer, 1, 8);

        // �Է��� ���� ������� �� �ɼ��� ����
        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayer, PlayerTtl = 10000 };
        
        // �� ������ ��û
        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public void OnBackButtonClicked()
    {
        // �κ񿡼� �ڷ� ���� ��ư�� ������ �� �۵��ϴ� �Լ�

        // �κ񿡼� ����.
        PhotonNetwork.LeaveLobby();

    }

    public void OnLeaveRoomClicked()
    {
        // �� ������ ��ư�� �������� �۵��ϴ� �Լ�

        // �� ���� ��û
        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGameButtonClicked()
    {
        // ��ŸƮ ���� ��ư�� ����� �Լ�

        // ���� �ִ� ���� �ɼ��� ����, ������ �ʴ� ���·� ����
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        // �ΰ��� ���� �ҷ���.
        //PhotonNetwork.LoadLevel("GameScene");
        PhotonNetwork.LoadLevel("TankScene");
    }

    private void ClearRoomListView()
    {
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        roomListEntries.Clear();
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }                
            }
            else if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject entry = Instantiate(roomEntryPrefab);
            entry.transform.SetParent(roomContent.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<RoomEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

            roomListEntries.Add(info.Name, entry);
        }
    }

    #endregion UNITY

    #region PHTON CALLBACK

    public override void OnConnectedToMaster()
    {
        // ������ ���ӵǾ��� �� �߻��ϴ� �ݹ� �Լ�

        // ���� �гη� ������.
        SetActivePanel(PANEL.Connect);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        // ������ ������ �����Ǿ��� �� �߻��ϴ� �ݹ� �Լ�

        // �α��� �гη� ������.
        SetActivePanel(PANEL.Login);
        Debug.LogError(cause.ToString());
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // �� ����Ʈ�� �ֽ�ȭ �Ҷ� �۵��ϴ� �ݹ� �Լ�

        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnJoinedLobby()
    {
        SetActivePanel(PANEL.Lobby);
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnLeftLobby()
    {
        SetActivePanel(PANEL.Connect);
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(PANEL.Connect);
        Debug.LogError("Create Room Failed with Error(" + returnCode + ") : " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetActivePanel(PANEL.Connect);
        Debug.LogError("Join Room Failed with Error(" + returnCode + ") : " + message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // ���� �� ��Ī�� ���������� �۵��ϴ� �ݹ� �Լ�.

        string roomName = "Room " + Random.Range(1000, 10000);
        RoomOptions options = new RoomOptions { MaxPlayers = 8 };

        // �� ����.
        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public override void OnJoinedRoom()
    {
        // �濡 ���� ���� ���������� �۵��ϴ� �ݹ� �Լ�.

        // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
        
        // ������ �� ����� �ʱ�ȭ �����ش�.
        cachedRoomList.Clear();


        // �� �г��� Ȱ��ȭ �����ش�.
        SetActivePanel(PANEL.Room);

        // �÷��̾� ����Ʈ�� null�϶� ��ųʸ� ����
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        // ��� �÷��̾� ����Ʈ�� �޾ƿ� �ݺ����� ������.
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            // �÷��̾� ��Ʈ�� ����
            GameObject entry = Instantiate(playerEntryPrefab);
            // ��ġ ���� �� ũ�� ����
            entry.transform.SetParent(playerListContent.transform);
            entry.transform.localScale = Vector3.one;

            // ��Ʈ���� �÷��̾��� �ѹ��� �г����� ����
            entry.GetComponent<PlayerEntry>().Initialize(p.ActorNumber, p.NickName);

            object isPlayerReady;
            // �÷��̾��� ���� ���θ� �޾ƿ� �����Ѵ�.
            if (p.CustomProperties.TryGetValue(GameData.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerEntry>().SetPlayerReady((bool)isPlayerReady);

            }
            // �÷��̾� ��Ʈ�� ������Ʈ �߰�
            playerListEntries.Add(p.ActorNumber, entry);
        }

        // ���� ���� �����϶� Ȱ��ȭ
        startGameButton.gameObject.SetActive(CheckPlayersReady());

        // �� �ε� ����ȭ�� ���� ���
        Hashtable props = new Hashtable
        {
            {GameData.PLAYER_LOAD, false}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnLeftRoom()
    {
        // ���� ���� �� �۵��ϴ� �ݹ� �Լ�

        // ���� �гη� ��ȯ
        SetActivePanel(PANEL.Connect);

        // �÷��̾� ��Ʈ���� �ı� �� �ʱ�ȭ.
        foreach (GameObject entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        playerListEntries.Clear();
        playerListEntries = null;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // �÷��̾ �濡 �������� �۵��ϴ� �ݹ� �Լ�

        // �÷��̾� ��Ʈ�� ����
        GameObject entry = Instantiate(playerEntryPrefab);
        // ��ġ �� ũ�� ����
        entry.transform.SetParent(playerListContent.transform);
        entry.transform.localScale = Vector3.one;

        // ��Ʈ�� �ʱ�ȭ
        entry.GetComponent<PlayerEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        // ����Ʈ�� �߰�
        playerListEntries.Add(newPlayer.ActorNumber, entry);

        // ���� ���� �Ǿ������� ��ŸƮ ��ư �߰�
        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // �÷��̾ �濡�� �������� �۵��ϴ� �ݹ� �Լ�

        // �ش� �÷��̾� ��Ʈ�� �ı�
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);

        // �÷��̾� ��Ʈ������ ����
        playerListEntries.Remove(otherPlayer.ActorNumber);

        // ���� �÷��̾��� �غ� ���ο� ���� ��ŸƮ ��ư Ȱ��ȭ ���� Ȯ��
        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // �����Ͱ� ����Ǿ����� �۵��ϴ� �ݹ� �Լ�

        // Ŭ���̾�Ʈ�� ���� �ѹ��� ��ġ�ϴ� Ŭ���̾�Ʈ�� ��ŸƮ��ư�� Ȱ��ȭ
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            startGameButton.gameObject.SetActive(CheckPlayersReady());
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // �÷��̾��� ������Ƽ�� ����Ǿ��� �� �۵��ϴ� �ݹ� �Լ�

        // �÷��̾� ��Ʈ���� ������ ���� ����
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        GameObject entry;

        // Ÿ�� �÷��̾�� ��ġ�ϴ� �÷��̾� ��Ʈ���� �����´�.
        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object isPlayerReady;
            // ���� ���θ� Ȯ���Ѵ�.
            if (changedProps.TryGetValue(GameData.PLAYER_READY, out isPlayerReady))
            {
                // ���� ���ο� ���� �÷��̾� ��Ʈ���� �ʱ�ȭ���ش�.
                entry.GetComponent<PlayerEntry>().SetPlayerReady((bool)isPlayerReady);
            }
            
        }
        // ���������� ���� ��ŸƮ ��ư Ȱ��ȭ ���θ� Ȯ���Ѵ�.
        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    #endregion

    private bool CheckPlayersReady()
    {
        // �÷��̾� ���� ���θ� Ȯ���ϴ� �Լ�

        // ������ Ŭ���̾�Ʈ�� �ƴ϶�� ���� ����
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        // ���� ���� �÷��̾� ����Ʈ�� ������ �ϳ��ϳ� Ȯ���Ѵ�.
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            // ���𿩺θ� �����´�.
            if (p.CustomProperties.TryGetValue(GameData.PLAYER_READY, out isPlayerReady))
            {
                // ���� ���°� �ƴϸ� ����
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            // ���� ���ΰ� ���ٸ�
            else
            {
                // ����
                return false;
            }
        }

        // �׸�
        return true;
    }

    public void LocalPlayerPropertiesUpdated()
    {
        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }
}
