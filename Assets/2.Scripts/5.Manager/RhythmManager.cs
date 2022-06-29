using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Photon.Pun;



public class RhythmManager : Singleton<RhythmManager>
{

    [Header("Beat")]
    public float bpm;
    public float hitAreaRate;
    private double prevTime;

    [Header("Rhythm")]
    public RhythmBox    rhythmBox;
    
    public Transform[]  notePos;
    
    [Header("Note")]
    public RhythmNote rhythmNote;
    public List<RhythmNote> notePool;
    public float        noteSpeed;


    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip beatsfx;
    public int bgmIndex;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

    }

    private void Start()
    {
        CreateNotePool();
    }
    private void CreateNotePool()
    {
        notePool = new List<RhythmNote>();
        for (int i = 0; i < 50; i++)
        {
            RhythmNote note = Instantiate(rhythmNote, notePos[0].position, Quaternion.identity, notePos[0]);
            notePool.Add(note);
            note.ReturnObj();
        }
        rhythmBox.SetHitArea(hitAreaRate);
    }

    [PunRPC]
    public void RhythmStart()
    {
        BGMData bgmData = SoundManager.Instance.inGameBgms.bgms[bgmIndex];
        bpm = bgmData.bpm;
        SoundManager.Instance.BGSoundPlay(bgmData.bgm, 0);
        
        StartCoroutine("RhythmRoutine");
        
    }

    public bool BeatCheck()
    {
        if (rhythmBox.isBeat)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator RhythmRoutine()
    {
        while (true)
        {
            if (PhotonNetwork.Time >= prevTime + (60f / bpm))
            {
                prevTime = PhotonNetwork.Time;
                CreateNote();
            }
            yield return null;
        }
    }
    public void CreateNote()
    {
        RhythmNote note = notePool.Find(
            (x) => x.gameObject.activeSelf == false);
        if (note == null)
        {
            note = Instantiate(rhythmNote, notePos[0].position,
                Quaternion.identity, notePos[0]);
            notePool.Add(note);
        }
        note.SetUp(notePos[0], rhythmBox.gameObject, 1f / noteSpeed);
        rhythmBox.RhythmHit();
    }





}
