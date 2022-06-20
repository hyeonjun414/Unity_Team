using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

using Cinemachine;



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
}

[System.Serializable]
public class CharacterStatus
{
    public int damage;
    public int hp;
    public Point curPos;
    public int currentCombo;
    public int killCount;
    public int deathCount;
}

public class Character : MonoBehaviourPun, IPunObservable
{
    [Header("Node")]
    public TileNode curNode;
    
    [Header("Player State")]
    public CharacterStatus stat;
    public ePlayerInput eCurInput = ePlayerInput.NULL;
    public PlayerState state = PlayerState.Normal;
    public int defencePoint;
    public int DP
    {
        get { return defencePoint; }
        set 
        { 
            defencePoint = value;
            if(defencePoint <= 0)
            {
                state = PlayerState.Normal;
                anim.SetBool("Defend", false);
                defencePoint = 0;
            }
        }
    }

    [Header("Command")]
    public CharacterRote roteCommand;
    public CharacterInput inputCommand;
    public CharacterMove moveCommand;
    public CharacterAction actionCommand;

    //private UnityAction OnRhythmHit;

    [HideInInspector]
    public Animator anim;
    private PlayerDir dir;
    public PlayerDir Dir
    {
        get { return dir; }
        set
        {
            dir = value;
            if (dir == PlayerDir.Start)
                dir = PlayerDir.Left;
            else if (dir == PlayerDir.End)
                dir = PlayerDir.Up;
        }
    }
    private void Awake()
    {
        anim = GetComponent<Animator>();

        inputCommand = gameObject.AddComponent<CharacterInput>();
        inputCommand.SetUp(this);
        moveCommand = gameObject.AddComponent<CharacterMove>();
        moveCommand.SetUp(this);
        roteCommand = gameObject.AddComponent<CharacterRote>();
        roteCommand.SetUp(this);
        actionCommand = gameObject.AddComponent<CharacterAction>();
        actionCommand.SetUp(this);


        Dir = PlayerDir.Right;

        float angle = 0f;
        switch (Dir)
        {
            case PlayerDir.Up:
                angle = 0f;
                break;
            case PlayerDir.Right:
                angle = 90f;
                break;
            case PlayerDir.Down:
                angle = 180f;
                break;
            case PlayerDir.Left:
                angle = 270f;
                break;
        }
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        RhythmManager.Instance.OnRhythmHit += RhythmOnChange;
        photonView.RPC("SetUp", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void SetUp()
    {
        if (photonView.IsMine)
        {
            GameObject.Find("LocalCamera").GetComponent<CinemachineVirtualCamera>().Follow = transform;
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_GEN, true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            
        }
        Map map = MapManager.Instance.map;
        
        Point vec = map.startPos[photonView.Owner.GetPlayerNumber()];
        // 자신의 최초 노드를 지정
        TileNode tile = map.GetTileNode(vec);
        curNode = tile;
        CharacterReset();

        // 현재 플레이어 위치 = 최초 노드 위치
        stat.curPos = curNode.tilePos;
        curNode.eOnTileObject = eTileOccupation.PLAYER;
        transform.position = tile.transform.position + Vector3.up * 0.5f;
        anim.speed = 2f;


    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if(RhythmHit())
        {
            inputCommand.Execute();
            roteCommand.Execute();
            moveCommand.Execute();
            actionCommand.Execute();
        }


        eCurInput = ePlayerInput.NULL;
    }
    public bool RhythmHit()
    {
        if (Input.anyKeyDown && RhythmManager.Instance.BitCheck())
        {
            RhythmManager.Instance.rhythmBox.NoteHit();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CharacterReset()
    {
        stat = new CharacterStatus();
        stat.damage = 1;
        stat.hp = 5;
        stat.curPos.y = 0;
        stat.curPos.x = 0;
        stat.currentCombo = 0;
        stat.killCount = 0;
        stat.deathCount = 0;

    }

    [PunRPC]
    public void Damaged(int damageInt)
    {
        stat.hp -= damageInt;
        anim.SetTrigger("Take Damage");
        if (stat.hp <= 0)
        {
            Die();
        }
    }
    [PunRPC]
    public void Block()
    {
        anim.SetBool("Defend", true);
        DP++;
        state = PlayerState.Defend;
    }

    public void RhythmOnChange()
    {
        print(photonView.Owner.NickName);
        DP--;
    }

    private void Die()
    {
        // anim.SetTrigger("Die");
        // MapManager_verStatic.Instance.map.GetTileNode(characterStatus.curPositionY,characterStatus.curPositionX).objectOnTile=null;
        // MapManager_verStatic.Instance.map.GetTileNode(characterStatus.curPositionY,characterStatus.curPositionX).eOnTileObject=eTileOccupation.EMPTY;
        // Destroy(gameObject);

        if (!photonView.IsMine) return;

        var builder = new StringBuilder();
        builder.Append(PhotonNetwork.LocalPlayer.NickName);
        builder.Append(" 이 사망하였습니다");
        string deadString = builder.ToString();
        photonView.RPC("SendLogToPlayers",RpcTarget.All,deadString);
    }
    
    [PunRPC]
    public void SendLogToPlayers(string msg)
    {
        GameLogManager.Instance.AddQueue(msg);
    }

    private void OnCollisionEnter(Collision other)
    {
        print("collide");
        if (other.collider.CompareTag("Player"))
        {
            moveCommand.CollidedPlayer();
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 playerPos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        Debug.DrawRay(playerPos, transform.forward, Color.green);

    }
    [PunRPC]
    public void SetCommand(ePlayerInput input, int curY, int curX)
    {
        eCurInput = input;
        stat.curPos = new Point(curY, curX);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(state);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            state = (PlayerState)stream.ReceiveNext();
        }
    }
}
