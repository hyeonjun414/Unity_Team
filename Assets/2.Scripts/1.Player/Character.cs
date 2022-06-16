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
}
public enum ePlayerInput
{
    NULL,
    MOVE,
    ROTATE,
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


    public bool isInputAvailable = true;
    public Transform[] rayPos;
    public ePlayerInput playerInput = ePlayerInput.NULL;
    public CharacterStatus characterStatus;
    [HideInInspector]
    public Animator anim;

    [Header("Command")]
    public MoveCommand moveCommand;
    public ActionCommand actionCommand;

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
        if(photonView.IsMine)
            GameObject.Find("LocalCamera").GetComponent<CinemachineVirtualCamera>().Follow = transform;

        Map map = MapManager_verStatic.Instance.map;
        Vector2 vec = map.startPos[PhotonNetwork.LocalPlayer.GetPlayerNumber()];
        TileNode tile = map.GetTileNode(vec);
        curNode = tile;
        CharacterReset();
        characterStatus.curPositionX = tile.posX;
        characterStatus.curPositionY = tile.posY;
        transform.position = tile.transform.position + Vector3.up * 0.5f;

    }

    private void Update()
    {
        if(!photonView.IsMine) return;
        //if(!isInputAvailable)return;
        CheckAvailability();
        Move();
        Action();
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
        switch(Dir)
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
        while(true)
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
        if(characterStatus.hp <=0)
        {
            Die();
        }
    }
    private void Die()
    {
        anim.SetTrigger("Die");
        MapManager.Instance.grid[characterStatus.curPositionX,characterStatus.curPositionY].objectOnTile=null;
        MapManager.Instance.grid[characterStatus.curPositionX,characterStatus.curPositionY].eOnTileObject=eTileOccupation.EMPTY;
        Destroy(gameObject);

    }
    public void CheckAvailability()
    {
        //적이 사방에 있으면 해당 방향으로는 move 를 할 수 없게 예외처리
        //사방에 벽이 있으면 해당 방향으로는 move를 할 수 없게 예외처리
        //사방에 노드가 없는 큐브가 있으면 그 방향으로는 move를 할 수 없게 예외처리

       // MapManager.Instance.mapSizeX
       
    }
    private void OnDrawGizmos()
    {
        Vector3 playerPos = new Vector3(transform.position.x,transform.position.y+1f,transform.position.z);
        for(int i=0; i<4; ++i)
        {
            Debug.DrawLine(playerPos,rayPos[i].position,Color.red);
        }
        
        //Debug.DrawRay(playerPos,rayPos.position,Color.red);
    }

}
