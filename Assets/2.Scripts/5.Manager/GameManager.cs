using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;

public class GameManager : MonoBehaviourPunCallbacks
{
    private bool isGameStart;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        isGameStart = false;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_LOAD, true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    #region PHOTON CALLBACK

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected : " + cause.ToString());
        SceneManager.LoadScene("NewLobbyScene");
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey(GameData.PLAYER_LOAD))
        {
            if (CheckAllPlayerLoadLevel())
            {
                StartCoroutine(StartCountDown());
            }
        }
        else if (changedProps.ContainsKey(GameData.PLAYER_GEN))
        {
            if (CheckAllCharacter() && !isGameStart)
            {
                isGameStart = true;
                print(targetPlayer.NickName);
                BattleManager.Instance.RegisterAllPlayer();
                RhythmManager.Instance.RhythmStart();
            }
        }
    }


    #endregion PHOTON CALLBACK

    private IEnumerator StartCountDown()
    {
        yield return null;
        // TODO : 선택된 맵을 생성해야함.

        PhotonNetwork.Instantiate("PlayerCharacter", Vector3.zero, Quaternion.identity, 0);
    }

    private bool CheckAllPlayerLoadLevel()
    {
        return PlayersLoadLevel() == PhotonNetwork.PlayerList.Length;
    }

    private int PlayersLoadLevel()
    {
        int count = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(GameData.PLAYER_LOAD, out playerLoadedLevel))
            {
                if ((bool)playerLoadedLevel)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private bool CheckAllCharacter()
    {
        return PlayersCharacter() == PhotonNetwork.PlayerList.Length;
    }

    private int PlayersCharacter()
    {
        int count = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(GameData.PLAYER_GEN, out playerLoadedLevel))
            {
                if ((bool)playerLoadedLevel)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public void LeaveRoom(){
        PhotonNetwork.LeaveRoom();
    }

}
