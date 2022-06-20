﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoteCommand : MonoBehaviour
{
    public Character player;
    public abstract void Execute();
    public void SetUp(Character player)
    {
        this.player = player;
    }
}