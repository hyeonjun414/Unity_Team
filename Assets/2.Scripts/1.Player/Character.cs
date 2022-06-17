using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Cinemachine;



public class CharacterStatus
{
    public int damage;
    public int hp;
    public int curPositionX;
    public int curPositionY;
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

public class Character : MonoBehaviourPun
{
    [Header("Node")]
    public TileNode curNode;

    public int playerNumber;
    public bool isInputAvailable = true;
    public Transform[] rayPos;
    public ePlayerInput playerInput = ePlayerInput.NULL;
    public CharacterStatus characterStatus;
    [HideInInspector]
    public Animator anim;

    [Header("Command")]
    public ePlayerInput eCurInput;
    public CharacterInput inputCommand;
    public CharacterMove moveCommand;
    public CharacterAction actionCommand;
    public Vector2 playerHeadingPos = Vector2.zero;
    public bool isMoving = true; //임시
    public bool isCrashing = false; //임시
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

            SetDirection();
        }
    }
    private void Awake()
    {
        anim = GetComponent<Animator>();

        inputCommand = gameObject.AddComponent<CharacterInput>();
        inputCommand.SetUp(this);
        moveCommand = gameObject.AddComponent<CharacterMove>();
        moveCommand.SetUp(this);
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
        
        Vector2 vec = map.startPos[playerNumber];
        TileNode tile = map.GetTileNode(vec);
        curNode = tile;
        CharacterReset();
        characterStatus.curPositionX = tile.posX;
        characterStatus.curPositionY = tile.posY;
        transform.position = tile.transform.position + Vector3.up * 0.5f;

    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        inputCommand.Execute();
    }

    public void CharacterReset()
    {
        characterStatus = new CharacterStatus();
        characterStatus.damage = 1;
        characterStatus.hp = 5;
        characterStatus.curPositionX = 0;//(int)spawnPoint.x;
        characterStatus.curPositionY = 0;//(int)spawnPoint.y;
        characterStatus.currentCombo = 0;
        characterStatus.killCount = 0;
        characterStatus.deathCount = 0;

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

    public void SetDirection()
    {
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
        StartCoroutine(RotateRoutine(Quaternion.AngleAxis(angle, Vector3.up)));
    }
    IEnumerator RotateRoutine(Quaternion destRot)
    {
        Quaternion originRot = transform.rotation;
        float curTime = 0;
        while (true)
        {
            if (curTime > 0.2f)
                break;
            curTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(originRot, destRot, curTime / 0.2f);
            yield return null;
        }
    }
    
    public void Damaged(int damageInt)
    {
        characterStatus.hp -= damageInt;
        anim.SetTrigger("Take Damage");
        if (characterStatus.hp <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        anim.SetTrigger("Die");
        MapManager_verStatic.Instance.map.GetTileNode(characterStatus.curPositionY,characterStatus.curPositionX).objectOnTile=null;
        MapManager_verStatic.Instance.map.GetTileNode(characterStatus.curPositionY,characterStatus.curPositionX).eOnTileObject=eTileOccupation.EMPTY;
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
    public void SetCommand(ePlayerInput input)
    {
        eCurInput = input;
    }


}
