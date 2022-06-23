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
    
    public static GameManager Instance { get; private set; }
    public Text serverText;
    public Text pingText;
    public Text infoText;
    public CharacterData data;
    public Transform[] spawnPos;

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_LOAD, true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    private void FixedUpdate()
    {
        pingText.text = PhotonNetwork.GetPing().ToString();
        serverText.text = PhotonNetwork.Time.ToString();
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
                
                object characterIndex=0;
                PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(GameData.PLAYER_INDEX,out characterIndex);
                StartCoroutine(StartCountDown((int)characterIndex));

            }
            else
            {
                PrintInfo("wait players " + PlayersLoadLevel() + " / " + PhotonNetwork.PlayerList.Length);
            }
        }
        if (changedProps.ContainsKey(GameData.PLAYER_GEN))
        {
            if (CheckAllCharacter())
            {
                StartCoroutine(RegisterPlayer());

            }
            else
            {
                PrintInfo("wait players " + PlayersCharacter() + " / " + PhotonNetwork.PlayerList.Length);
            }
        }
    }


    #endregion PHOTON CALLBACK

    private IEnumerator StartCountDown(int index)
    {

        // TODO : 선택된 맵을 생성해야함.

        PrintInfo("All Player Loaded, Start Count Down");
        yield return new WaitForSeconds(1.0f);

        for (int i = GameData.COUNTDOWN; i > 0; i--)
        {
            PrintInfo("Count Down " + i);
            yield return new WaitForSeconds(1.0f);
        }

        PrintInfo("Start Game!");

        PhotonNetwork.Instantiate("PlayerCharacter", Vector3.zero, Quaternion.identity, 0);

        yield return new WaitForSeconds(1.0f);
        infoText.gameObject.SetActive(false);
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

    private void PrintInfo(string info)
    {
        Debug.Log(info);
        infoText.text = info;
    }


    IEnumerator RegisterPlayer(){
        yield return new WaitForSeconds(1f);
        BattleManager.Instance.RegisterAllPlayer();
    }


    IEnumerator GoToEndingScene(){
        yield return new WaitForSeconds(3f);
        //PhotonNetwork
    }

    public void GotoEnding(){
        if(!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to load a level but we are not the master Client");
        }
       // PhotonNetwork.LoadLevel(PhotonNetwork.CurrentRoom.PlayerCount);
    }


    public void LeaveRoom(){
        PhotonNetwork.LeaveRoom();
    }

    private void OnPlayerConnected(Player player) {
        
    }

}
