using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BattleManager : MonoBehaviourPun
{
    [HideInInspector]
    public int isReadyCount = 0;
    public bool isResultButtonClicked = false;
    public BattleResultPanel battleResultPanel;

    

    [Header("Player")]
    public List<Character> players;
    //살아있는 플레이어 
    public List<Character> alivePlayer;
    //사망한 플레이어
    public List<Character> deadPlayer;

    [Header("Text")]
    public Text resultText;
    public GameObject resultTextObj;

    [Header("UI")]
    public RegenUI regenUI;

    [Header("Mode")]
    public ModeType mode;
  
    public static BattleManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void Start()
    {
        battleResultPanel = UIManager.Instance.battleResultPanel;
    }

    public void SetUpMode()
    {
        object modeData;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameData.GAME_MODE, out modeData))
        {
            mode = (ModeType)modeData;
            switch (mode)
            {
                case ModeType.BattleRoyale:
                    SetUpBattleRoyale();
                    break;
                case ModeType.OneShot:
                    SetUpOneShotMode();
                    break;
                case ModeType.DeathMatch:
                    SetUpDeathMatch();
                    break;
            }
        }
        UIManager.Instance.topTextUI.SetUp(mode);
    }
    
    private void Update() 
    {
        ShowBattleStatus();

    }
    public void ShowBattleStatus()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            battleResultPanel.gameObject.SetActive(!battleResultPanel.gameObject.activeSelf);
        }
    }

    public void SetUpBattleRoyale()
    {
        foreach (Character p in players)
        {
            p.isRegen = false;
            p.stat.hp = 5;
            p.statusUI?.UpdateStatusUI();
        }
    }
    
    public void SetUpOneShotMode()
    {

        // 한대 맞으면 죽는 데스매치
        foreach(Character p in players)
        {
            p.isRegen = false;
            p.stat.hp = 1;
            p.statusUI?.UpdateStatusUI();
        }
    }
    public void SetUpDeathMatch()
    {
        TimeManager.Instance.limitTime = 60f;

        foreach (Character p in players)
        {
            p.isRegen = true;
        }
    }

    public void RegisterAllPlayer()
    {
        players = FindObjectsOfType<Character>().ToList();

        alivePlayer = FindObjectsOfType<Character>().ToList();

        // 등록이 완료되었다면 모드 설정에 따라 설정을 진행한다.
        SetUpMode();

    }

    public void PlayerAddScore(int playerId)
    {
        Character player = players.Find((x) => x.playerId == playerId);
        player.stat.score += 5;
        player.SendUpdateUI();
    }
    public void PlayerAddKill(int playerId)
    {
        Character player = players.Find((x) => x.playerId == playerId);
        player.stat.killCount++;
        player.SendUpdateUI();
    }

    //플레이어가 죽었을 때 판정 || 플레이어가 disconnect되었을 때 호출
    public void PlayerOut(Character deadPL)
    {
        // 시간제 게임일 경우 계산안함.
        if (mode == ModeType.DeathMatch) return;
        //alivePlayer 리스트에서 죽은 플레이어를 뺀다.
        alivePlayer.Remove(deadPL);

        Debug.Log("playerOut: "+deadPL.playerId);
        
        //deadPlayer 리스트에 죽은 플레이어를 더해준다.
        deadPlayer.Add(deadPL);

        // 남은 플레이어 갱신
        UIManager.Instance.topTextUI.UpdateUI();

        if(alivePlayer.Count ==1)
        {
            GameOver();
        }
        
    }

    [PunRPC]
    public void BattleOverMessage()
    {
        resultTextObj.SetActive(true);

        string myNickName = PhotonNetwork.LocalPlayer.NickName;

        //플레이어가 죽지 않았을 때
        if (alivePlayer.Exists(player => player.nickName == myNickName))
        {
            resultText.text = "YOU WIN!";
            return;
        }
        //플레이어가 죽었을 때
        else if (deadPlayer.Exists(player => player.nickName == myNickName))
        {
            resultText.text = "YOU LOSE!";
            return;
        }
    }
    private void SetBattleResult()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            Character player = players.Find((x) => x.nickName == p.NickName);
            if (player != null)
            {
                SetCustomValue(p, player.stat.kill, player.stat.death, player.stat.score);
            }
        }

    }

    public void GameOver()
    {
        photonView.RPC("BattleOverMessage", RpcTarget.All);
        StartCoroutine("GameOverRoutine");
    }

    IEnumerator GameOverRoutine(){
        yield return new WaitForSeconds(3f);  
        
        SetBattleResult();

        yield return new WaitForSeconds(5f);
        
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
            PhotonNetwork.LoadLevel("Result");
        }
        

    }
    private void SetCustomValue(Player p, int _kill, int _death, int _score)
    {
        Hashtable kill = new Hashtable() { { GameData.PLAYER_KILL, _kill } };
        p.SetCustomProperties(kill);
        Hashtable death = new Hashtable() { { GameData.PLAYER_DEAD, _death } };
        p.SetCustomProperties(death);
        Hashtable score = new Hashtable() { { GameData.PLAYER_SCORE, _score } };
        p.SetCustomProperties(score);
    }

}
