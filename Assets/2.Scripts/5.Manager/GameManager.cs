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
    
    public static GameManager Instance { get; private set; } // �̱��� ����

    public Text infoText;
    public Transform[] spawnPos; // �÷��̾� ���� ������

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        // ���� �÷��̾��� �ε� ���θ� true�� �ٲ� Ŀ���� ������Ƽ ������ ���� �ٸ� �÷��̾�� �˸���.
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_LOAD, true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    #region PHOTON CALLBACK

    public override void OnDisconnected(DisconnectCause cause)
    {
        // ������ ������ �� �κ� ������ �̵��Ѵ�.
        Debug.Log("Disconnected : " + cause.ToString());
        SceneManager.LoadScene("LobbyScene");
    }

    public override void OnLeftRoom()
    {
        // ���� ������ ������ ���´�.
        PhotonNetwork.Disconnect();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // �÷��̾��� ������Ƽ�� ���ŵɶ� �۵��ϴ� �ݹ��Լ�
        // �ε尡 �Ϸ��ٰ� Ȯ�ε� ��
        if (changedProps.ContainsKey(GameData.PLAYER_LOAD))
        {
            // ��� �÷��̾ �ε尡 �Ϸ�Ǿ��� ��, ���� ���� ��ƾ�� �����Ų��.
            if (CheckAllPlayerLoadLevel())
            {
                StartCoroutine(StartCountDown());
            }
            // �ε尡 �Ϸ���� �ʾ�����
            else
            {
                // ��� ��Ȳ�� �˸���.
                PrintInfo("wait players " + PlayersLoadLevel() + " / " + PhotonNetwork.PlayerList.Length);
            }
        }
    }

    #endregion PHOTON CALLBACK

    private IEnumerator StartCountDown()
    {
        //        yield return new WaitUntil(()=>InputCheckManager.Instance.isReadyCount >= MapManager_verStatic.Instance.playerCount);
        // ���� ���� ��ƾ
        PrintInfo("All Player Loaded, Start Count Down");
        yield return new WaitForSeconds(1.0f);

        // ī��Ʈ �ٿ�
        for (int i = GameData.COUNTDOWN; i > 0; i--)
        {
            PrintInfo("Count Down " + i);
            yield return new WaitForSeconds(1.0f);
        }

        PrintInfo("Start Game!");

        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0);
    }

    private bool CheckAllPlayerLoadLevel()
    {
        // ��� �ε尡 �Ϸ�� ���¶�� true, �ƴϸ� false�� ��ȯ
        return PlayersLoadLevel() == PhotonNetwork.PlayerList.Length;
    }

    private int PlayersLoadLevel()
    {
        // ���� �ε尡 �Ϸ�� �÷��̾ �ľ��ϴ� �Լ�
        int count = 0;
        // �÷��̾� ����Ʈ�� ������ ������ �÷��̾ ���� �ε� ������Ƽ�� Ȯ���Ѵ�.
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(GameData.PLAYER_LOAD, out playerLoadedLevel))
            {
                // �ε� ������Ƽ�� �Ϸ� �����̸� ī��Ʈ�� �ø���.
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
        // ���� ���޿� UI
        Debug.Log(info);
        infoText.text = info;
    }
}
