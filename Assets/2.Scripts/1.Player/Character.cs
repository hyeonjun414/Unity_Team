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
    public Transform rayPos;
    private int[,] grid;
    public characterStatus characterStatus;
    private Animator anim;
    private MoveCommand moveCommand;
    private ActionCommand actionCommand;
    private Vector3 spawnPoint;
    private float beatCoolTime;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        
        CharacterReset();
        beatCoolTime = 1f;// 임시 RhythmManager.instance.beat;
      //  int mapSizeX=0;//임시로 맵사이즈용 변수 지정
      //  int mapSizeY=0;//임시로 맵사이즈용 변수 지정
      //  grid = new int[mapSizeX,mapSizeY];
    }
    private void Update()
    {
        Move();
        Action();
    }

    public void CharacterReset()
    {
        characterStatus = new characterStatus();
        characterStatus.hp = 5;
        characterStatus.curPositionX = (int)spawnPoint.x;
        characterStatus.curPositionY = (int)spawnPoint.y;
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
    private void OnDrawGizmos()
    {
        Vector3 playerPos = new Vector3(transform.position.x,transform.position.y+1f,transform.position.z);
        Debug.DrawLine(playerPos,rayPos.position,Color.red);
        //Debug.DrawRay(playerPos,rayPos.position,Color.red);
    }

}
