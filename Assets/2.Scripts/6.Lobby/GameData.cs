using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public const int COUNTDOWN = 0;

    public const string PLAYER_READY = "Ready";
    public const string PLAYER_LOAD = "Load";
    public const string PLAYER_GEN = "Character";

    public const string PLAYER_INDEX = "PlayerIndex";

    public const string GAME_MODE = "GameMode";

    public const string PLAYER_OWNERID = "OwnerID";


    [Header("게임결과정보 저장")]
    public const string PLAYER_NAME = "PlayerName";
    public const string PLAYER_KILL = "PlayerKill";
    public const string PLAYER_DEAD = "PlayerDead";
    public const string PLAYER_RANK = "PlayerRank";
    public static Color GetColor(int playerNumber)
    {
        switch(playerNumber)
        {
            case 0: return Color.red;
            case 1: return Color.green;
            case 2: return Color.blue;
            case 3: return Color.yellow;
            case 4: return Color.cyan;
            case 5: return Color.magenta;
            case 6: return Color.white;
            case 7: return Color.black;
            default: return Color.grey;
        }
    }
}
