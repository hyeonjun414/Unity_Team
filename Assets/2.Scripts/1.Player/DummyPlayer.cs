using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class DummyPlayer : MonoBehaviourPun
{
    public Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
}
