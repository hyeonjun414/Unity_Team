﻿using System.Linq;
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
    
    public void DeathMatchResult(string mode)
    {
        List<Character> players = BattleManager.Instance.players;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            for(int i=0; i<players.Count;++i)
            {
                if(players[i].photonView.Owner.ActorNumber == p.ActorNumber)
                {
                    players[i].stat.killCount = p.GetScore();
                }       
            }  
        }
        List<Character> sortedList = new List<Character>();
        sortedList = players.OrderByDescending(Character => Character.stat.killCount).ToList();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if(sortedList[0].photonView.Owner.ActorNumber == p.ActorNumber)
            {
                ResultEntry winnerEntry = Instantiate(Resources.Load<ResultEntry>("WinnerEntry"),resultPanel.transform);
                winnerEntry.BattleResult(sortedList[0].nickName, p.GetScore() ,sortedList[0].stat.deathCount , 1 ,mode);
            }
            for(int i=1; i<sortedList.Count;++i)
            {
                if(sortedList[i].photonView.Owner.ActorNumber == p.ActorNumber)
                {
                    if(sortedList[i].stat.killCount == sortedList[0].stat.killCount)
                    {
                        ResultEntry winnerEntry = Instantiate(Resources.Load<ResultEntry>("WinnerEntry"),resultPanel.transform);
                        winnerEntry.BattleResult(sortedList[i].nickName, p.GetScore() ,sortedList[i].stat.deathCount , 1,mode);

                        SetCustomValue(p,sortedList[i].nickName, p.GetScore() ,sortedList[i].stat.deathCount , 1 );

                    }
                    else
                    {
                        ResultEntry loserEntry = Instantiate(Resources.Load<ResultEntry>("LoserEntry"),resultPanel.transform);
                        loserEntry.BattleResult(sortedList[i].nickName, p.GetScore() ,sortedList[i].stat.deathCount , i+1,mode);

                        SetCustomValue(p,sortedList[i].nickName, p.GetScore() ,sortedList[i].stat.deathCount , i+1 );

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
                winnerEntry.BattleResult(alives[0].nickName, p.GetScore() ,alives[0].stat.deathCount , 1,mode);
                
                SetCustomValue(p,alives[0].nickName, p.GetScore() ,alives[0].stat.deathCount , 1 );
               
            }
            for(int i=0; i<deads.Count;++i)
            {
                if(deads[i].photonView.Owner.ActorNumber == p.ActorNumber)
                {
                    ResultEntry loserEntry = Instantiate(Resources.Load<ResultEntry>("LoserEntry"),resultPanel.transform);
                    loserEntry.BattleResult(deads[i].nickName, p.GetScore() ,deads[i].stat.deathCount , 2,mode);
                    
                    SetCustomValue(p,deads[i].nickName, p.GetScore() ,deads[i].stat.deathCount , 2 );

                }
            }
            
        }
    }

    private void SetCustomValue(Player p, string nickName, int _kill, int _death, int _rank)
    {
            ExitGames.Client.Photon.Hashtable name = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_NAME, nickName } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(name);
            ExitGames.Client.Photon.Hashtable kill = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_KILL, _kill } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(kill);
            ExitGames.Client.Photon.Hashtable death = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_DEAD, _death } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(death);
            ExitGames.Client.Photon.Hashtable rank = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_RANK, _rank } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(rank);
    }
}