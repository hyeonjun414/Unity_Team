using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MoveCommand
{
    private Character player;
    private float spaceBetweenTiles = 2.5f;
    public CharacterMove(Character player)
    {
        this.player = player;
    }
    private void Start()
    {
        DirectionCheck();
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
        
    }
    private void RightCommand()
    {
        
    }
    private void UpCommand()
    {
        
    }
    private void DownCommand()
    {

    }
    private void RightRotateCommand()
    {
        DirectionCheck();
    }
    private void LeftRotateCommand()
    {
        DirectionCheck();
    }
    private void DirectionCheck()
    {

    }
    
}
