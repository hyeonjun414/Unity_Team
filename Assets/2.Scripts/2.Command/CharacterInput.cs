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
            player.photonView.RPC("SetCommand", RpcTarget.All, ePlayerInput.MOVE_LEFT);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            player.photonView.RPC("SetCommand", RpcTarget.All, ePlayerInput.MOVE_RIGHT);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            player.photonView.RPC("SetCommand", RpcTarget.All, ePlayerInput.MOVE_UP);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            player.photonView.RPC("SetCommand", RpcTarget.All, ePlayerInput.MOVE_DOWN);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            player.photonView.RPC("SetCommand", RpcTarget.All, ePlayerInput.ROTATE_LEFT);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            player.photonView.RPC("SetCommand", RpcTarget.All, ePlayerInput.ROTATE_RIGHT);
        }
    }
}
