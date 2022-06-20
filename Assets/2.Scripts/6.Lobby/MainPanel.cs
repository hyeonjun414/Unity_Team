using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class MainPanel : MonoBehaviourPunCallbacks
{
    [Header("Login")]
    public GameObject loginPanel; // 로그인 패널

    public InputField playerNameInput; // 플레이어 네임스페이스

    [Header("Connect")]
    public GameObject inConnectPanel; // 연결중 패널

    [Header("Create Room")]
    public GameObject createRoomPanel; // 방 생성 패널

    public InputField roomNameInputField; // 방 이름 네임스페이스
    public InputField maxPlayersInputField; // 최대 인원 네임스페이스

    [Header("Lobby")]
    public GameObject inLobbyPanel; // 로비 패널

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
        // 적절한 패널을 활성화 하는 함수

        loginPanel.SetActive(panel == PANEL.Login);
        inConnectPanel.SetActive(panel == PANEL.Connect);
        createRoomPanel.SetActive(panel == PANEL.CreateRoom);
        inLobbyPanel.SetActive(panel == PANEL.Lobby);
        inRoomPanel.SetActive(panel == PANEL.Room);
    }

    public void OnLoginButtonClicked()
    {
        // 로그인 패널의 버튼을 눌렀을 때 작동하는 함수

        string playerName = playerNameInput.text;

        if (playerName == "")
        {
            Debug.LogError("Invalid Player Name");
            return;
        }
        // 입력된 플레이어 네임스페이스의 텍스트를 닉네임으로 설정한다.
        PhotonNetwork.LocalPlayer.NickName = playerName;
        // 해당 설정으로 연결을 한다.
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnCreateRoomButtonClicked()
    {
        // 방 생성 버튼을 눌렀을 때 작동하는 함수

        // 방 생성 패널을 활성화.
        SetActivePanel(PANEL.CreateRoom);
    }

    public void OnRandomMatchingButtonClicked()
    {
        // 랜덤 매칭 버튼을 눌렀을 때 작동하는 함수

        // 랜덤한 방에 들어감.
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnLobbyButtonClicked()
    {
        // 로비 버튼에 연결된 함수

        // 로비에 접속하는 함수.
        PhotonNetwork.JoinLobby();
    }

    public void OnLogoutButtonClicked()
    {
        // 로그아웃 버튼에 연결된 함수

        // 서버와의 연결을 해제함.
        PhotonNetwork.Disconnect();
    }

    public void OnCreateRoomCancelButtonClicked()
    {
        // 방 생성 취소 버튼에 연결된 함수

        // 연결 패널 활성화
        SetActivePanel(PANEL.Connect);
    }

    public void OnCreateRoomConfirmButtonClicked()
    {
        // 방 생성 확인 버튼에 연결된 함수

        
        string roomName = roomNameInputField.text;
        
        if (roomName == "")
            roomName = "Room" + Random.Range(1000, 10000);

        byte maxPlayer = byte.Parse(maxPlayersInputField.text);
        maxPlayer = (byte) Mathf.Clamp(maxPlayer, 1, 8);

        // 입력한 값을 기반으로 방 옵션을 설정
        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayer, PlayerTtl = 10000 };
        
        // 방 생성을 요청
        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public void OnBackButtonClicked()
    {
        // 로비에서 뒤로 가기 버튼을 눌렀을 때 작동하는 함수

        // 로비에서 나감.
        PhotonNetwork.LeaveLobby();

    }

    public void OnLeaveRoomClicked()
    {
        // 방 떠나기 버튼을 눌렀을때 작동하는 함수

        // 방 떠남 요청
        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGameButtonClicked()
    {
        // 스타트 게임 버튼에 연결됨 함수

        // 현재 있는 방의 옵션을 닫힌, 보이지 않는 상태로 변경
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        // 인게임 씬을 불러옴.
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
        // 서버에 접속되었을 때 발생하는 콜백 함수

        // 연결 패널로 변경함.
        SetActivePanel(PANEL.Connect);
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        // 서버에 접속이 해제되었을 때 발생하는 콜백 함수

        // 로그인 패널로 변경함.
        SetActivePanel(PANEL.Login);
        Debug.LogError(cause.ToString());
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 룸 리스트를 최신화 할때 작동하는 콜백 함수

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
        // 랜덤 방 매칭에 실패했을때 작동하는 콜백 함수.

        string roomName = "Room " + Random.Range(1000, 10000);
        RoomOptions options = new RoomOptions { MaxPlayers = 8 };

        // 방 생성.
        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public override void OnJoinedRoom()
    {
        // 방에 들어가는 것을 성공했을때 작동하는 콜백 함수.

        // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
        
        // 기존의 방 목록을 초기화 시켜준다.
        cachedRoomList.Clear();


        // 방 패널을 활성화 시켜준다.
        SetActivePanel(PANEL.Room);

        // 플레이어 리스트가 null일때 딕셔너리 생성
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        // 모든 플레이어 리스트를 받아와 반복문을 돌린다.
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            // 플레이어 엔트리 생성
            GameObject entry = Instantiate(playerEntryPrefab);
            // 위치 설정 및 크기 설정
            entry.transform.SetParent(playerListContent.transform);
            entry.transform.localScale = Vector3.one;

            // 엔트리에 플레이어의 넘버와 닉네임을 설정
            entry.GetComponent<PlayerEntry>().Initialize(p.ActorNumber, p.NickName);

            object isPlayerReady;
            // 플레이어의 레디 여부를 받아와 적용한다.
            if (p.CustomProperties.TryGetValue(GameData.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerEntry>().SetPlayerReady((bool)isPlayerReady);

            }
            // 플레이어 엔트리 오브젝트 추가
            playerListEntries.Add(p.ActorNumber, entry);
        }

        // 전부 레디 상태일때 활성화
        startGameButton.gameObject.SetActive(CheckPlayersReady());

        // 씬 로드 동기화를 위해 사용
        Hashtable props = new Hashtable
        {
            {GameData.PLAYER_LOAD, false}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnLeftRoom()
    {
        // 방을 떠날 때 작동하는 콜백 함수

        // 연결 패널로 전환
        SetActivePanel(PANEL.Connect);

        // 플레이어 엔트리를 파괴 및 초기화.
        foreach (GameObject entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        playerListEntries.Clear();
        playerListEntries = null;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // 플레이어가 방에 들어왔을때 작동하는 콜백 함수

        // 플레이어 엔트리 생성
        GameObject entry = Instantiate(playerEntryPrefab);
        // 위치 및 크기 설정
        entry.transform.SetParent(playerListContent.transform);
        entry.transform.localScale = Vector3.one;

        // 엔트리 초기화
        entry.GetComponent<PlayerEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        // 리스트에 추가
        playerListEntries.Add(newPlayer.ActorNumber, entry);

        // 레디가 전부 되어있으면 스타트 버튼 추가
        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 플레이어가 방에서 떠났을때 작동하는 콜백 함수

        // 해당 플레이어 엔트리 파괴
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);

        // 플레이어 엔트리에서 제외
        playerListEntries.Remove(otherPlayer.ActorNumber);

        // 남은 플레이어의 준비 여부에 따라 스타트 버튼 활성화 여부 확인
        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // 마스터가 변경되었을때 작동하는 콜백 함수

        // 클라이언트중 액터 넘버가 일치하는 클라이언트의 스타트버튼을 활성화
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            startGameButton.gameObject.SetActive(CheckPlayersReady());
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // 플레이어의 프로퍼티가 변경되었을 때 작동하는 콜백 함수

        // 플레이어 엔트리가 없을때 동적 생성
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        GameObject entry;

        // 타겟 플레이어와 일치하는 플레이어 엔트리를 가져온다.
        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object isPlayerReady;
            // 레디 여부를 확인한다.
            if (changedProps.TryGetValue(GameData.PLAYER_READY, out isPlayerReady))
            {
                // 레디 여부에 따라 플레이어 엔트리를 초기화해준다.
                entry.GetComponent<PlayerEntry>().SetPlayerReady((bool)isPlayerReady);
            }
            
        }
        // 변경점으로 인한 스타트 버튼 활성화 여부를 확인한다.
        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    #endregion

    private bool CheckPlayersReady()
    {
        // 플레이어 레디 여부를 확인하는 함수

        // 마스터 클라이언트가 아니라면 실행 안함
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        // 현재 방의 플레이어 리스트를 가져와 하나하나 확인한다.
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            // 레디여부를 가져온다.
            if (p.CustomProperties.TryGetValue(GameData.PLAYER_READY, out isPlayerReady))
            {
                // 레디 상태가 아니면 빨강
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            // 레디 여부가 없다면
            else
            {
                // 빨강
                return false;
            }
        }

        // 그린
        return true;
    }

    public void LocalPlayerPropertiesUpdated()
    {
        startGameButton.gameObject.SetActive(CheckPlayersReady());
    }
}
