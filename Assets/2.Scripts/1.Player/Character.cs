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

public class Character : MonoBehaviour
{
    [HideInInspector]
    public bool isLocal = false;
    public Transform[] rayPos;
    
    public characterStatus characterStatus;
    [HideInInspector]
    public Animator anim;
    private MoveCommand moveCommand;
    private ActionCommand actionCommand;
    private int[,] spawnPoint;
    private float beatCoolTime;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        moveCommand = gameObject.AddComponent<CharacterMove>();
        moveCommand.SetUp(this);
        actionCommand = gameObject.AddComponent<CharacterAction>();
        actionCommand.SetUp(this);
        
        spawnPoint = new int[10,10];
        spawnPoint[0,0] = 3;//임시
        CharacterReset();
        beatCoolTime = 1f;// 임시 RhythmManager.instance.beat;

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
