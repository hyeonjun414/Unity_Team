using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class ActionCommand : MonoBehaviourPun
{
    public Character player;
    public abstract void Execute();
    public void SetUp(Character player)
    {
        this.player = player;
    }
}
