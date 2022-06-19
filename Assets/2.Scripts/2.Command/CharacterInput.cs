using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterInput : InputCommand
{
    public override void Execute()
    {
        if (Input.anyKeyDown && RhythmManager.Instance.BitCheck())
        {
            RhythmManager.Instance.rhythmBox.NoteHit();
            PlayerInput();

        }
        
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
    }
    public void SetCommand(ePlayerInput input)
    {
        player.photonView.RPC("SetCommand", RpcTarget.All,
                new object[3] { input, player.stat.curPos.y, player.stat.curPos.x });
    }
}
