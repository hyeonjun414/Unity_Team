using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MoveCommand
{
    private float spaceBetweenTiles = 2.5f;
    public string direction;

    private void Start()
    {
        //DirectionCheck();
    }
    public override void Execute()
    {
        LeftCommand();
        RightCommand();
        UpCommand();
        DownCommand();
        RightRotateCommand();
        LeftRotateCommand();     
    }
    private void LeftCommand()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            player.characterStatus.curPositionX-=1;
            Move();
        }
        
    }
    private void RightCommand()
    {
        if(Input.GetKeyDown(KeyCode.D))
        {
            player.characterStatus.curPositionX+=1;
            Move();
        }
        
    }
    private void UpCommand()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            player.characterStatus.curPositionY+=1;
            Move();
        }
        
    }
    private void DownCommand()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            player.characterStatus.curPositionY-=1;
            Move();
        }
        
    }
    private void RightRotateCommand()
    {

        SetPlayerDirection();
    }
    private void LeftRotateCommand()
    {

    }
    private void Move()
    {
        player.anim.SetTrigger("Jump");
        StartCoroutine(WaitForNextInput());
        
    }

    private void SetPlayerDirection()
    {

    }
    public IEnumerator WaitForNextInput()
    {
        yield return new WaitForSeconds(0.3f);
        transform.position = new Vector3
            ((player.characterStatus.curPositionX*MapManager.spaceBetweenTiles),
            transform.position.y,
             (player.characterStatus.curPositionY*MapManager.spaceBetweenTiles));
    }
    
}
