using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;
using TMPro;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerResultInfo
{
    public PlayerResultInfo(int actorNumber, string name, int kill, int death, int rank)
    {
        this.actorNumber = actorNumber;
        this.name = name;
        this.kill = kill;
        this.death = death;
        this.rank = rank;
    }
    public int actorNumber;
    public string name;
    public int kill;
    public int death;
    public int rank;
}
public class ResultSceneManager : MonoBehaviourPun
{
    private int _sycFinished=0;
    public int syncFinished
    {
        get{
            return _sycFinished;
        }
        set{
            _sycFinished = value;
            Debug.Log(_sycFinished+ "명의 플레이어 정보 동기화 완료");
        }
    }
    public GameObject resultWindow;

    // 승리한 플레이어 정보
    string[] winnerNic;
    // 패배한 플레이어 정보
    string[] loserNic;

    public CharacterData characterData;
    public GameObject characterSet;
    public int characterIndex;


    [Header("WINNER")]
    // 승리한 플레이어 닉네임 띄울 텍스트
    public Text[] winnerNicUIText;

    [Header("LOSER")]
    // 패배한 플레이어 닉네임 띄울 텍스트
    public Text[] loserNicUIText;


    List<PlayerResultInfo> resultInfoList; 
    private void Awake()
    {
        resultInfoList = new List<PlayerResultInfo>();
        Invoke("Test", 1f);
        InitPlayers();
    }

    public void Test()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("NewLobbyScene");
    }
    private void InitPlayers()
    {
        foreach( Player p in PhotonNetwork.PlayerList)
        {
            // object name;

            // p.CustomProperties.TryGetValue(GameData.PLAYER_NAME,out name);

            object kill;
            p.CustomProperties.TryGetValue(GameData.PLAYER_KILL,out kill);

            object death;
            p.CustomProperties.TryGetValue(GameData.PLAYER_DEAD,out death);

            object rank;
            p.CustomProperties.TryGetValue(GameData.PLAYER_RANK,out rank);
            
            
            string _name = (string)name;
            int _kill = (int)kill;
            int _death = (int)death;
            int _rank = (int)rank;
            resultInfoList.Add(new PlayerResultInfo(p.ActorNumber,p.NickName,_kill,_death,_rank));
        }
    }
    private void Start()
    {
       // WinnerInfomation();
       // LoserInformation();

        //PlayerAnimPlay();
        foreach( Player p in PhotonNetwork.PlayerList)
        {
            object value = null;
            
            if (p.CustomProperties.TryGetValue(GameData.IS_EMAIL, out value))
            {
                bool isMail = (bool)value; 
                if(isMail)
                {
                    SyncUpdatedInformation(p.NickName);
                }
            }       
        }
    }
    private void SyncUpdatedInformation(string nickName)
    {

        Debug.Log("0번성공");
        DataBaseManager.Instance.GetUserIDbyNickName(nickName,(str)=>{
            Debug.Log("1번성공");
            DataBaseManager.Instance.ReadPlayerInfo(str,true,(str2)=>{
                Debug.Log("2번성공 : " + str + " str2: " + str2);

                String value = str2;

                string[] words = value.Split('$');
                //이메일 $ 닉네임 $ 총판수 $ 승리수
                for(int i=0; i<resultInfoList.Count;++i)
                {
                    Debug.Log("resultInfoList.count: "+resultInfoList.Count + " i: "+i);
                    Debug.Log(resultInfoList[i].name + " :: "+words[1]);
                    if(resultInfoList[i].name == words[1])
                    {
                        if(resultInfoList[i].rank == 1)
                        {
                            int wins = int.Parse(words[3]);
                            ++wins;
                            words[3] = wins.ToString();
                        }
                        int totals = int.Parse(words[2]);
                        ++totals;
                        words[2] = totals.ToString();
                        Debug.Log("3번성공");
                        DataBaseManager.Instance.WriteExistingPlayerDB(str,words[0],words[1],words[2],words[3]);
                        Debug.Log("4번성공");
                    }
                }
                
                
                ++_sycFinished;
            });
         });
    }

    // // 승리자 닉네임 결과창 UI에 뜨도록 설정
    // public void WinnerInfomation()
    // {
    //     for (int i = 0; i < BattleManager.Instance.alivePlayer.Count; i++)
    //     {
    //         winnerNic[i] = BattleManager.Instance.alivePlayer[i].nickName;
    //         winnerNicUIText[i].text = winnerNic[i];
    //         // PhotonNetwork.Instantiate(BattleManager.Instance.alivePlayer[i].);
    //     }
    // }

    // // 패배자 닉네임 결과창 UI에 뜨도록 설정
    // public void LoserInformation()
    // {
    //     for (int i = 0; i < BattleManager.Instance.deadPlayer.Count; i++)
    //     {
    //         loserNic[i] = BattleManager.Instance.deadPlayer[i].nickName;
    //         loserNicUIText[i].text = loserNic[i];
    //         // PhotonNetwork.Instantiate(BattleManager.Instance.deadPlayer[i].);
    //     }
    // }

    // 플레이어 승리, 패배 애니메이션 재생
    public void PlayerAnimPlay()
    {

            //BattleManager.Instance.alivePlayer[i].anim.SetTrigger("Crying");
    }





}
