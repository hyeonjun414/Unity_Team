using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class characterStatus
{
    public int hp;
    public int curPositionX;
    public int curPositionY;
    public int currentCombo;
    public int killCount;
    public int deathCount;
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

public class Character : MonoBehaviour
{
    [Header("Node")]
    public TileNode curNode;


    [HideInInspector]
    public bool isLocal = false;
    public Transform[] rayPos;
    
    public characterStatus characterStatus;
    [HideInInspector]
    public Animator anim;

    [Header("Command")]
    private MoveCommand moveCommand;
    private ActionCommand actionCommand;

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
        CharacterReset();

    }
    private void Update()
    {
        if(!isLocal)return;

        CheckAvailability();
        Move();
        Action();
    }

    public void CharacterReset()
    {
        characterStatus = new characterStatus();
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
