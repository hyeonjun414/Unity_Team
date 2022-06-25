using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class InLobbyPanel : MonoBehaviour
{
    public GameObject roomContent;
    public GameObject roomEntryPrefab;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;

    private void Start()
    {
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();
    }

    private void OnDisable()
    {
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    public void OnBackButtonClicked()
    {
        PhotonNetwork.LeaveLobby();
        LobbyManager.instance.SetActivePanel(LobbyManager.PANEL.Connect);
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {

        // 방리스트가 업데이트될때 방리스트의 초기화한다.
        ClearRoomListView();

        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public void ClearRoomList()
    {
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject entry = Instantiate(roomEntryPrefab);
            entry.transform.SetParent(roomContent.transform);
            entry.transform.localScale = Vector3.one; 
            entry.GetComponent<RoomEntry>().SetUp(info);
            roomListEntries.Add(info.Name, entry);
        }
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

    private void ClearRoomListView()
    {
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        roomListEntries.Clear();
    }
}
