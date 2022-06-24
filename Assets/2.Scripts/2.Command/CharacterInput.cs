using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterInput : InputCommand
{
    public override void Execute()
    {
        PlayerInput();
    }
    public void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetCommand(ePlayerInput.MOVE_LEFT);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            SetCommand(ePlayerInput.MOVE_RIGHT);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            SetCommand(ePlayerInput.MOVE_UP);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            SetCommand(ePlayerInput.MOVE_DOWN);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            SetCommand(ePlayerInput.ROTATE_LEFT);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            SetCommand(ePlayerInput.ROTATE_RIGHT);
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            SetCommand(ePlayerInput.ATTACK);
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            SetCommand(ePlayerInput.BLOCK);
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            SetCommand(ePlayerInput.USE_ITEM);
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            SetCommand(ePlayerInput.CHANGE_ITEM_SLOT);
        }
    }

    public void SetCommand(ePlayerInput input)
    {
        player.eCurInput = input;
    }
}
