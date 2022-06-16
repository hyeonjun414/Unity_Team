using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;



public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; } // 싱글톤 변수

    public Text infoText;
    public Transform[] spawnPos; // 플레이어 스폰 포지션

    private void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        // 로컬 플레이어의 로드 여부를 true로 바꿔 커스텀 프로퍼티 변경을 통해 다른 플레이어에게 알린다.
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_LOAD, true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    #region PHOTON CALLBACK

    public override void OnDisconnected(DisconnectCause cause)
    {
        // 연결이 끊겼을 때 로비 씬으로 이동한다.
        Debug.Log("Disconnected : " + cause.ToString());
        SceneManager.LoadScene("LobbyScene");
    }

    public override void OnLeftRoom()
    {
        // 방을 떠날때 연결을 끊는다.
        PhotonNetwork.Disconnect();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // 플레이어의 프로퍼티가 갱신될때 작동하는 콜백함수
        // 로드가 완료됬다고 확인될 때
        if (changedProps.ContainsKey(GameData.PLAYER_LOAD))
        {
            // 모든 플레이어가 로드가 완료되었을 때, 게임 시작 루틴을 실행시킨다.
            if (CheckAllPlayerLoadLevel())
            {
                StartCoroutine(StartCountDown());
            }
            // 로드가 완료되지 않았을때
            else
            {
                // 대기 현황을 알린다.
                PrintInfo("wait players " + PlayersLoadLevel() + " / " + PhotonNetwork.PlayerList.Length);
            }
        }
    }

    #endregion PHOTON CALLBACK

    private IEnumerator StartCountDown()
    {
        // 게임 시작 루틴
        PrintInfo("All Player Loaded, Start Count Down");
        yield return new WaitForSeconds(1.0f);

        // 카운트 다운
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
        // 모두 로드가 완료된 상태라면 true, 아니면 false를 반환
        return PlayersLoadLevel() == PhotonNetwork.PlayerList.Length;
    }

    private int PlayersLoadLevel()
    {
        // 현재 로드가 완료된 플레이어를 파악하는 함수
        int count = 0;
        // 플레이어 리스트를 가져와 각각의 플레이어에 대해 로드 프로퍼티를 확인한다.
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (p.CustomProperties.TryGetValue(GameData.PLAYER_LOAD, out playerLoadedLevel))
            {
                // 로드 프로퍼티가 완료 상태이면 카운트를 늘린다.
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
        // 정보 전달용 UI
        Debug.Log(info);
        infoText.text = info;
    }
}
