using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Cinemachine;


[System.Serializable]
public class CharacterStatus
{
    public int damage;
    public int hp;
    public Point curPos;
    public int currentCombo;
    public int killCount;
    public int deathCount;
    public int destX;
    public int destY;
    public bool isMoving;
    public bool isCrashing;
}
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

public class Character : MonoBehaviourPun//, IPunObservable
{
    [Header("Node")]
    public TileNode curNode;

    public int playerNumber;
    public bool isInputAvailable = true;
    public Transform[] rayPos;
    public ePlayerInput playerInput = ePlayerInput.NULL;
    public CharacterStatus stat;
    [HideInInspector]
    public Animator anim;

    [Header("Command")]
    public ePlayerInput eCurInput;
    public CharacterRote roteCommand;
    public CharacterInput inputCommand;
    public CharacterMove moveCommand;
    public CharacterAction actionCommand;

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
        playerNumber = photonView.Owner.GetPlayerNumber();
        Map map = MapManager_verStatic.Instance.map;
        
        Point vec = map.startPos[playerNumber];
        TileNode tile = map.GetTileNode(vec);
        curNode = tile;
        CharacterReset();
        stat.curPos = tile.tilePos;
        transform.position = tile.transform.position + Vector3.up * 0.5f;

    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        inputCommand.Execute();
        roteCommand.Execute();
        moveCommand.Execute();

        eCurInput = ePlayerInput.NULL;
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
    public void Move()
    {
        moveCommand?.Execute();
    }
    public void Action()
    {
        actionCommand?.Execute();
    }

    public void Damaged(int damageInt)
    {
        stat.hp -= damageInt;
        anim.SetTrigger("Take Damage");
        if (stat.hp <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        anim.SetTrigger("Die");
        MapManager_verStatic.Instance.map.GetTileNode(stat.curPos).objectOnTile=null;
        MapManager_verStatic.Instance.map.GetTileNode(stat.curPos).eOnTileObject=eTileOccupation.EMPTY;
        Destroy(gameObject);

    }
  
    private void OnDrawGizmos()
    {
        Vector3 playerPos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        for (int i = 0; i < 4; ++i)
        {
            Debug.DrawLine(playerPos, rayPos[i].position, Color.red);
        }

    }
    [PunRPC]
    public void SetCommand(ePlayerInput input, int curY, int curX)
    {
        eCurInput = input;
        stat.curPos = new Point(curY, curX);
    }

/*    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            //stream.SendNext(transform.position);
            //stream.SendNext(transform.rotation);
            //stream.SendNext(stat.curPos);
        }
        else
        {
            //transform.position = (Vector3)stream.ReceiveNext();
            //transform.rotation = (Quaternion)stream.ReceiveNext();
            //stat.curPos = (Point)stream.ReceiveNext();
        }
    }*/
}
