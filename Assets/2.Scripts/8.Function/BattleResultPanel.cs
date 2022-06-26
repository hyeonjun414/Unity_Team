using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class BattleResultPanel : MonoBehaviour
{
    public bool isBattleFinished=false;
    public GameObject battleResultPanel;
    private GameObject resultPanel;
    public void SetBattleResult()
    {
        //object mode;
        //if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameData.GAME_MODE, out mode))
        //{
        //    int modeNum = (int)mode;

            resultPanel = battleResultPanel.transform.GetChild(0).gameObject;

            //테스트
            int modeNum = 1;
            //테스트끝
            switch (modeNum)
            {
                case 0:
                    //한사람이 남을때까지
                    
                    LastManResult("LastMan");
                    break;
                case 1:
                    DeathMatchResult("DeathMatch");
                    //시간매치 => 누가 가장 많이 죽였는가
                    break;
                case 2:
                    //원펀 => 사실상 lastman이랑 같음
                    LastManResult("OnePun");
                    break;
            }
        //}
        
        battleResultPanel.SetActive(true);
    }
    public void SetFinalResult(List<PlayerResultInfo> resultInfo)
    {
        resultPanel = battleResultPanel.transform.GetChild(0).gameObject;
        
        for(int i=0; i<resultInfo.Count;++i)
        {
            if(resultInfo[i].rank==1)
            {
                ResultEntry winnerEntry = Instantiate(Resources.Load<ResultEntry>("WinnerEntry"),resultPanel.transform);
                winnerEntry.BattleResult(resultInfo[i].name,resultInfo[i].kill,resultInfo[i].death,resultInfo[i].rank,"NULL");
            }
            else
            {
                ResultEntry loserEntry = Instantiate(Resources.Load<ResultEntry>("LoserEntry"),resultPanel.transform);
                loserEntry.BattleResult(resultInfo[i].name,resultInfo[i].kill,resultInfo[i].death,resultInfo[i].rank,"NULL");
            }   
        }
         battleResultPanel.SetActive(true);
    }
    public void ClearPanel()
    {
        ResultEntry[] resultEntries = resultPanel.transform.GetComponentsInChildren<ResultEntry>();
        for(int i=0; i<resultEntries.Length;++i)
        {
            Destroy(resultEntries[i].gameObject);
        }
    }
    
    public void DeathMatchResult(string mode)
    {
        List<Character> players = BattleManager.Instance.players;

        List<Character> sortedList = new List<Character>();
        sortedList = players.OrderByDescending(Character => Character.stat.killCount).ToList();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if(sortedList[0].photonView.Owner.ActorNumber == p.ActorNumber)
            {
                ResultEntry winnerEntry = Instantiate(Resources.Load<ResultEntry>("WinnerEntry"),resultPanel.transform);
                winnerEntry.BattleResult(sortedList[0].nickName, sortedList[0].stat.killCount ,sortedList[0].stat.deathCount , 1 ,mode);
            
                SetCustomValue(p,sortedList[0].nickName, sortedList[0].stat.killCount ,sortedList[0].stat.deathCount , 1 );
            }
            for(int i=1; i<sortedList.Count;++i)
            {
                if(sortedList[i].photonView.Owner.ActorNumber == p.ActorNumber)
                {
                    if(sortedList[i].stat.killCount == sortedList[0].stat.killCount)
                    {
                        ResultEntry winnerEntry = Instantiate(Resources.Load<ResultEntry>("WinnerEntry"),resultPanel.transform);
                        winnerEntry.BattleResult(sortedList[i].nickName, sortedList[i].stat.killCount ,sortedList[i].stat.deathCount , 1,mode);

                        SetCustomValue(p,sortedList[i].nickName, sortedList[i].stat.killCount ,sortedList[i].stat.deathCount , 1 );

                    }
                    else
                    {
                        ResultEntry loserEntry = Instantiate(Resources.Load<ResultEntry>("LoserEntry"),resultPanel.transform);
                        loserEntry.BattleResult(sortedList[i].nickName, sortedList[i].stat.killCount ,sortedList[i].stat.deathCount , i+1,mode);

                        SetCustomValue(p,sortedList[i].nickName, sortedList[i].stat.killCount ,sortedList[i].stat.deathCount , i+1 );

                    }

                }
            }
        }
    }
    public void LastManResult(string mode)
    {
        
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            List<Character> alives = BattleManager.Instance.alivePlayer;
            List<Character> deads = BattleManager.Instance.deadPlayer;

            if(alives[0].photonView.Owner.ActorNumber == p.ActorNumber)
            {
                ResultEntry winnerEntry = Instantiate(Resources.Load<ResultEntry>("WinnerEntry"),resultPanel.transform);
                winnerEntry.BattleResult(alives[0].nickName, alives[0].stat.killCount ,alives[0].stat.deathCount , 1,mode);
                
                SetCustomValue(p,alives[0].nickName, alives[0].stat.killCount ,alives[0].stat.deathCount , 1 );
               
            }
            for(int i=0; i<deads.Count;++i)
            {
                if(deads[i].photonView.Owner.ActorNumber == p.ActorNumber)
                {
                    ResultEntry loserEntry = Instantiate(Resources.Load<ResultEntry>("LoserEntry"),resultPanel.transform);
                    loserEntry.BattleResult(deads[i].nickName, deads[i].stat.killCount ,deads[i].stat.deathCount , 2,mode);
                    
                    SetCustomValue(p,deads[i].nickName, deads[i].stat.killCount ,deads[i].stat.deathCount , 2 );

                }
            }
            
        }
    }

    private void SetCustomValue(Player p, string nickName, int _kill, int _death, int _rank)
    {
            ExitGames.Client.Photon.Hashtable name = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_NAME, nickName } };
            p.SetCustomProperties(name);
            ExitGames.Client.Photon.Hashtable kill = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_KILL, _kill } };
            p.SetCustomProperties(kill);
            ExitGames.Client.Photon.Hashtable death = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_DEAD, _death } };
            p.SetCustomProperties(death);
            ExitGames.Client.Photon.Hashtable rank = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_RANK, _rank } };
            p.SetCustomProperties(rank);
    }
}
