using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using System;

public class InRoomPanel : MonoBehaviour
{
    [HideInInspector]
    public bool isEnterKeyEnabled = false;
    [Header("RoomPanel")]
    public TMP_Text[] ChatText;
    public TMP_InputField ChatInput;


    public GameObject playerListContent;
    public Button startGameButton;
    public Button readyGameButton;
    public TMP_Text readyButtonText;
    public PlayerEntry playerEntryPrefab;

    public GameObject playerInfoPanel;

    private Dictionary<int, PlayerEntry> playerListEntries;

    private bool localPlayerIsReady = false;

    [Header("PlayerInfoUI")]
    public TMP_Text nickName;
    public TMP_Text totalPlayTimes;
    public TMP_Text winTimes;
    public TMP_Text loseTimes;
    public TMP_Text winRate;

    [Header("RoomSetting Panel")]
    public RoomSettingPanel settingPanel;

    private void Update()
    {
        if (!isEnterKeyEnabled) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            LobbyManager.instance.Send();
        }
    }
    private void OnEnable()
    {
        //ChatInput.Select();
        isEnterKeyEnabled = true;
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, PlayerEntry>();
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            PlayerEntry entry = Instantiate(playerEntryPrefab);
            entry.transform.SetParent(playerListContent.transform);
            entry.transform.SetPositionAndRotation(playerListContent.transform.position, Quaternion.identity);
            entry.transform.localScale = Vector3.one;
            entry.Initialize(p.ActorNumber, p.NickName);

            if(p.IsMasterClient)
            {
                entry.masterIcon.SetActive(true);
            }
            if(p.IsLocal)
            {
                entry.localIcon.SetActive(true);
            }

            object characterIndex;
            if (p.CustomProperties.TryGetValue(GameData.PLAYER_INDEX, out characterIndex))
            {
                entry.SetPlayerCharacter((int)characterIndex);
            }

            playerListEntries.Add(p.ActorNumber, entry);
        }

        settingPanel.SetUp();

        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트만 게임시작을 나타내고 다른 플레이어가 준비상태가 아니라면 버튼을 상호작용 불가상태로 만든다.
            // 로딩 완료의 플레이어가 마스터 클라이언트를 제외하므로 -1을 해야한다.
            LocalPlayerPropertiesUpdated();
            startGameButton.gameObject.SetActive(true);
            readyGameButton.gameObject.SetActive(false);
        }
        else
        {
            // 마스터 클라이언트가 아니라며 게임시작 버튼을 비활성화 하고 
            localPlayerIsReady = false;
            readyButtonText.text = "준비";
            startGameButton.gameObject.SetActive(false);
            readyGameButton.gameObject.SetActive(true);
        }

        startGameButton.gameObject.SetActive(CheckPlayersReady());

        Hashtable props = new Hashtable
        {
            {GameData.PLAYER_LOAD, false}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }


    private void OnDisable()
    {
        isEnterKeyEnabled = false;
        foreach (PlayerEntry entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        playerListEntries.Clear();
        playerListEntries = null;
    }

    public void OnLeaveRoomClicked()
    {
        Hashtable props = new Hashtable() { { GameData.PLAYER_READY, false } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        PhotonNetwork.LeaveRoom();
    }

    public void OnStartGameButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel("mapTest");
        SoundManager.Instance.BGSoundPlay(SoundManager.Instance.bgSoundlist[1], 3);
    }

    public void OnReadyGameButtonClicked()
    {
        localPlayerIsReady = !localPlayerIsReady;
        FindLocalPlayerEntry()?.SetPlayerReadyImage(localPlayerIsReady);

        Hashtable props = new Hashtable() { { GameData.PLAYER_READY, localPlayerIsReady } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        if (PhotonNetwork.IsMasterClient)
        {
            LocalPlayerPropertiesUpdated();
        }
    }

    public PlayerEntry FindLocalPlayerEntry()
    {
        // 자기 자신의 엔트리를 가져옴
        return playerListEntries[PhotonNetwork.LocalPlayer.ActorNumber];
    }

    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p == PhotonNetwork.MasterClient) continue;
            if (p.CustomProperties.TryGetValue(GameData.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public void OnCloseInfoPanel()
    {
        playerInfoPanel.SetActive(false);
    }

    public void LocalPlayerPropertiesUpdated()
    {
        startGameButton.interactable = CheckPlayersReady();
        //startGameButton.gameObject.SetActive(CheckPlayersReady());
    }
    public void ShowPlayerInfo(string UID)
    {
        // Hashtable props = new Hashtable() { { GameData.PLAYER_READY, localPlayerIsReady } };
        // PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        DataBaseManager.Instance.ReadPlayerInfo(UID,false,(str)=>{
            String value = str;


            string[] words = value.Split('$');
            //닉네임 $ 총판수 $ 승리수
            nickName.text = words[0];
            totalPlayTimes.text = words[1];
            winTimes.text = words[2];

            int playTimesInt = int.Parse(totalPlayTimes.text);
            int winTimesInt = int.Parse(winTimes.text);
            int loseTimesInt = playTimesInt - winTimesInt;

            loseTimes.text = loseTimesInt.ToString();
            if (winTimesInt != 0)
            {
                winRate.text = (winTimesInt / playTimesInt).ToString("F1") + " %";
            }
            else
            {
                winRate.text = "- %";
            }


            playerInfoPanel.SetActive(true);

        });


    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerEntry entry = Instantiate(playerEntryPrefab);
        entry.transform.SetPositionAndRotation(playerListContent.transform.position, Quaternion.identity);
        entry.transform.SetParent(playerListContent.transform);
        entry.transform.localScale = Vector3.one;
        entry.Initialize(newPlayer.ActorNumber, newPlayer.NickName);


        playerListEntries.Add(newPlayer.ActorNumber, entry);

        if (PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트만 게임시작을 나타내고 다른 플레이어가 준비상태가 아니라면 버튼을 상호작용 불가상태로 만든다.
            // 로딩 완료의 플레이어가 마스터 클라이언트를 제외하므로 -1을 해야한다.
            LocalPlayerPropertiesUpdated();
            readyGameButton.gameObject.SetActive(false);
        }
        else
        {
            // 마스터 클라이언트가 아니라며 게임시작 버튼을 비활성화 하고 
            localPlayerIsReady = false;
            readyButtonText.text = "준비";
            startGameButton.gameObject.SetActive(false);
        }
    }


    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);

        startGameButton.interactable = CheckPlayersReady();
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            readyGameButton.gameObject.SetActive(false);
            startGameButton.gameObject.SetActive(true);
            startGameButton.interactable = CheckPlayersReady();
            FindLocalPlayerEntry().SetPlayerReadyImage(false);
        }
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, PlayerEntry>();
        }

        PlayerEntry entry;
        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(GameData.PLAYER_READY, out isPlayerReady))
            {
                entry.SetPlayerReadyImage((bool)isPlayerReady);
            }
        }

        startGameButton.interactable = CheckPlayersReady();


        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object characterIndex;
            if (changedProps.TryGetValue(GameData.PLAYER_INDEX, out characterIndex))
            {
                entry.GetComponent<PlayerEntry>().SetPlayerCharacter((int)characterIndex);
            }
        }
    }



}
