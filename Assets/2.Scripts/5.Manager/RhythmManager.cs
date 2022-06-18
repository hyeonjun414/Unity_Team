﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class RhythmManager : Singleton<RhythmManager>
{

    [Header("UI")]
    public Text         hitText;

    [Header("Beat")]
    public float        bpm;
    public float        hitAreaRate;
    public bool isBeat;

    [Header("Rhythm")]
    public RhythmBox    rhythmBox;
    public RhythmNote   rhythmNote;
    public Transform[]  notePos;

    [Header("Note")]
    public float        noteSpeed;


    [Header("Sound")]
    public AudioSource  audioSource;
    public AudioClip    beatsfx;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            StartCoroutine("RhythmRoutine");
        rhythmBox.SetHitArea(hitAreaRate);
    }


    public bool BitCheck()
    {
        if(rhythmBox.isBeat && isBeat)
        {
            hitText.text = "HIT";
            //print("HIT");
            isBeat = false;
            return true;
        }
        else
        {
            hitText.text = "MISS";
            //print("MISS");
            isBeat = false;
            return false;
        }
    }

    IEnumerator RhythmRoutine()
    {
        print("리듬 시작");
        yield return null;
        //yield return new WaitUntil(()=>InputCheckManager.Instance.isReadyCount >= MapManager_verStatic.Instance.playerCount);
        while (true)
        {
            if (bpm < 10) bpm = 10;
            //PhotonNetwork.Instantiate("Rhythm Note", notePos[0].position, Quaternion.identity);
            photonView.RPC("CreateNote", RpcTarget.All);
            //rhythmBox.RhythmHit();
            yield return new WaitForSeconds(60f/ bpm); 
        }
    }
    [PunRPC]
    public void CreateNote()
    {
        RhythmNote note = Instantiate(rhythmNote, notePos[0].position, Quaternion.identity, notePos[0]);
        note.SetUp(rhythmBox.gameObject, 1f / noteSpeed);
        rhythmBox.RhythmHit();
    }


    


}
