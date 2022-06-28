using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;

public class PlayerResultInfo
{
    public PlayerResultInfo(int actorNumber, string name, int kill, int death, int score, int index)
    {
        this.actorNumber = actorNumber;
        this.name = name;
        this.kill = kill;
        this.death = death;
        this.score = score;
        this.index = index;
    }
    public int actorNumber;
    public string name;
    public int kill;
    public int death;
    public int score;
    public int index;
}
public class ResultSceneManager : MonoBehaviour
{
    private int _syncFinished = 0;
    public int syncFinished
    {
        get
        {
            return _syncFinished;
        }
        set
        {
            _syncFinished = value;
            Debug.Log(_syncFinished + "명의 플레이어 정보 동기화 완료");
        }
    }
    public Transform[] WinnersSpawnPoint;
    public Transform[] LosersSpawnPoint;
    public GameObject[] skeletons;

    public BattleResultPanel battleResultPanel;


    List<PlayerResultInfo> resultInfoList;
    private void Awake()
    {
        resultInfoList = new List<PlayerResultInfo>();
        InitPlayers();
        SetInformation();
        SetPlayer();

    }


    // 이전 플레이어 정보 갱신
    private void InitPlayers()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object kill;
            p.CustomProperties.TryGetValue(GameData.PLAYER_KILL, out kill);

            object death;
            p.CustomProperties.TryGetValue(GameData.PLAYER_DEAD, out death);

            object score;
            p.CustomProperties.TryGetValue(GameData.PLAYER_SCORE, out score);

            object index;
            p.CustomProperties.TryGetValue(GameData.PLAYER_INDEX, out index);

            int _kill = (int)kill;
            int _death = (int)death;
            int _score = (int)score;
            int _index = (int)index;
            resultInfoList.Add(new PlayerResultInfo(p.ActorNumber, p.NickName, _kill, _death, _score, _index));
        }
    }
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        StartCoroutine(LeaveAndReturnToRoom());

        StartCoroutine(WaitForReadyToWrite());

    }

    private void SetInformation()
    {
        //battleResultPanel.SetFinalResult(resultInfoList);
    }
    private void SetPlayer()
    {
        List<PlayerResultInfo> infos = resultInfoList;

        //List<PlayerResultInfo> sortedList = new List<PlayerResultInfo>();
        //sortedList = infos.OrderByDescending(PlayerResultInfo => PlayerResultInfo.rank).ToList();

        for (int i = 0; i < resultInfoList.Count; ++i)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("CharactersEmpty/Character");
            int index = resultInfoList[i].index + 1;
            if (resultInfoList[i].index + 1 == 19) index = 1;
            builder.Append((index).ToString("D2"));
            string dummyPlayer = builder.ToString();

            DummyPlayer player = null;
            if (resultInfoList[i].score == 1)
            {
                if (WinnersSpawnPoint[0].transform.childCount == 0)
                {
                    player = Instantiate(Resources.Load<DummyPlayer>(dummyPlayer), WinnersSpawnPoint[0]);

                }
                else if (WinnersSpawnPoint[0].transform.childCount >= 1)
                {
                    if (WinnersSpawnPoint[1].transform.childCount == 0)
                    {
                        player = Instantiate(Resources.Load<DummyPlayer>(dummyPlayer), WinnersSpawnPoint[1]);
                    }
                    else if (WinnersSpawnPoint[1].transform.childCount == 1)
                    {
                        if (WinnersSpawnPoint[2].transform.childCount == 0)
                        {
                            player = Instantiate(Resources.Load<DummyPlayer>(dummyPlayer), WinnersSpawnPoint[2]);
                        }
                        else if (WinnersSpawnPoint[2].transform.childCount == 1)
                        {
                            if (WinnersSpawnPoint[3].transform.childCount == 0)
                            {
                                player = Instantiate(Resources.Load<DummyPlayer>(dummyPlayer), WinnersSpawnPoint[3]);
                            }
                        }
                    }
                }


                player.gameObject.SetActive(true);
                StartCoroutine(PlayerAnimPlay(player, "Victory"));
            }
            else
            {
                if (LosersSpawnPoint[0].transform.childCount == 0)
                {
                    player = Instantiate(Resources.Load<DummyPlayer>(dummyPlayer), LosersSpawnPoint[0]);
                    skeletons[0].SetActive(true);
                }
                else if (LosersSpawnPoint[0].transform.childCount >= 1)
                {
                    if (LosersSpawnPoint[1].transform.childCount == 0)
                    {
                        player = Instantiate(Resources.Load<DummyPlayer>(dummyPlayer), LosersSpawnPoint[1]);
                        skeletons[1].SetActive(true);
                    }
                    else if (LosersSpawnPoint[1].transform.childCount == 1)
                    {
                        if (LosersSpawnPoint[2].transform.childCount == 0)
                        {
                            player = Instantiate(Resources.Load<DummyPlayer>(dummyPlayer), LosersSpawnPoint[2]);
                            skeletons[2].SetActive(true);
                        }
                    }
                }
                player.gameObject.SetActive(true);
                StartCoroutine(PlayerAnimPlay(player, "Crying"));
            }


        }

    }
    private void SyncUpdatedInformation(string nickName)
    {

        Debug.Log("0번성공");
        DataBaseManager.Instance.GetUserIDbyNickName(nickName, (str) =>
        {
            Debug.Log("1번성공");
            DataBaseManager.Instance.ReadPlayerInfo(str, true, (str2) =>
            {
                Debug.Log("2번성공 : " + str + " str2: " + str2);

                String value = str2;

                string[] words = value.Split('$');
                //이메일 $ 닉네임 $ 총판수 $ 승리수
                for (int i = 0; i < resultInfoList.Count; ++i)
                {
                    Debug.Log("resultInfoList.count: " + resultInfoList.Count + " i: " + i);
                    Debug.Log(resultInfoList[i].name + " :: " + words[1]);
                    if (resultInfoList[i].name == words[1])
                    {
                        Debug.Log("resultInfoList[i].rank: " + resultInfoList[i].score);
                        if (resultInfoList[i].score == 1)
                        {
                            Debug.Log("2.5번성공; 승리카운트 올라가야함");
                            int wins = int.Parse(words[3]);
                            ++wins;
                            words[3] = wins.ToString();
                        }
                        int totals = int.Parse(words[2]);
                        Debug.Log("totals: " + totals + " words[2]: " + words[2]);
                        ++totals;
                        words[2] = totals.ToString();
                        Debug.Log("3번성공");
                        DataBaseManager.Instance.WriteExistingPlayerDB(str, words[0], words[1], words[2], words[3]);
                        Debug.Log("4번성공");
                    }
                }

                Debug.Log("5번성공");
                ++syncFinished;
                Debug.Log("6번성공");
            });
        });
    }
    private void WriteDB()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isMail;
            if(p.CustomProperties.TryGetValue(GameData.IS_EMAIL, out isMail))
            {
                if (p.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber) continue;
                //자기 데이터는 자기가 쓰도록 수정
                if ((bool)isMail)
                {
                    SyncUpdatedInformation(p.NickName);
                }
            }
        }
        Debug.Log("resultInfoList.Count: " + resultInfoList.Count);
        for (int i = 0; i < resultInfoList.Count; ++i)
        {
            Debug.Log(resultInfoList[i].name + " : " + resultInfoList[i].kill + " : " + resultInfoList[i].death + " : " + resultInfoList[i].score);
        }
    }


    private void ResetCustomProperties()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {

            ExitGames.Client.Photon.Hashtable prop1 =
                new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_READY, false } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(prop1);
            ExitGames.Client.Photon.Hashtable prop2 =
                new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_KILL, 0 } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(prop1);
            ExitGames.Client.Photon.Hashtable prop3 =
                new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_DEAD, 0 } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(prop1);
            ExitGames.Client.Photon.Hashtable prop4 =
                new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_SCORE, 0 } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(prop1);
            ExitGames.Client.Photon.Hashtable prop5 = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_LOAD, false } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(prop5);



        }

    }
    IEnumerator WaitForReadyToWrite()
    {
        yield return new WaitForSeconds(4f);
        WriteDB();

    }
    // 플레이어 승리, 패배 애니메이션 재생
    IEnumerator PlayerAnimPlay(DummyPlayer player, string whatToPlay)
    {
        yield return new WaitForSeconds(0.5f);
        player.anim.Play(whatToPlay);
    }
    IEnumerator LeaveAndReturnToRoom()
    {
        yield return new WaitForSeconds(10f);
        ResetCustomProperties();
        yield return new WaitForSeconds(5f);

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("NewLobbyScene");



    }





}
