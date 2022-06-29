using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePlayerInput
{
    NULL,
    MOVE_UP,
    MOVE_RIGHT,
    MOVE_DOWN,
    MOVE_LEFT,
    ROTATE_RIGHT,
    ROTATE_LEFT,
    ATTACK,
    BLOCK,
    USE_ITEM,
    CHANGE_ITEM_SLOT,
}

public enum PlayerDir
{
    Start,
    Up,
    Right,
    Down,
    Left,
    End
}

public enum PlayerState
{
    Normal,
    Move,
    Defend,
    Attack,
    Stun,
    Dead
}

public enum ModeType
{
    BattleRoyale,
    OneShot,
    DeathMatch,
    End,
}

public enum MapType
{
    Normal,
    Forest,
    Snow,
    End
}

public class GameData : MonoBehaviour
{
    [Header("Room Option")]
    public const string ROOM_NAME = "RoomName";
    public const string ROOM_ISACTIVE_PW = "IsActivePw";
    public const string ROOM_PW = "RoomPw";

    public const int COUNTDOWN = 0;

    public const string PLAYER_READY = "Ready";
    public const string PLAYER_LOAD = "Load";
    public const string PLAYER_GEN = "Character";

    public const string PLAYER_INDEX = "PlayerIndex";

    public const string GAME_MODE = "GameMode";
    public const string GAME_MAP = "GameMap";
  
    public const string IS_EMAIL = "IsEmail";

    [Header("게임결과정보 저장")]
    public const string PLAYER_NAME = "PlayerName";
    public const string PLAYER_KILL = "PlayerKill";
    public const string PLAYER_DEAD = "PlayerDead";
    public const string PLAYER_SCORE = "PlayerScore";
    public static string GetMode(ModeType type)
    {
        switch (type)
        {
            case ModeType.BattleRoyale:
                return "Battle Royale";
            case ModeType.OneShot:
                return "One Shot";
            case ModeType.DeathMatch:
                return "Death Match";
            default: return "";
        }

    }
    public static string GetMap(MapType type)
    {
        switch (type)
        {
            case MapType.Normal:
                return "The Ground";
            case MapType.Forest:
                return "The Forest";
            case MapType.Snow:
                return "The Snow";
            default: return "";
        }
    }
    public static bool InputGetCheck()
    {
        if (Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.D) ||
            Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.Q) ||
            Input.GetKeyDown(KeyCode.E) ||
            Input.GetKeyDown(KeyCode.H) ||
            Input.GetKeyDown(KeyCode.J) ||
            Input.GetKeyDown(KeyCode.N) ||
            Input.GetKeyDown(KeyCode.M))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    
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
