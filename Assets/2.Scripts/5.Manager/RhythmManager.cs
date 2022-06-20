using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Photon.Pun;



public class RhythmManager : Singleton<RhythmManager>
{

    [Header("UI")]
    public Text         hitText;

    [Header("Beat")]
    public float        bpm;
    public float        hitAreaRate;
    public bool isBeat;
    public double prevTime;

    [Header("Rhythm")]
    public RhythmBox    rhythmBox;
    public RhythmNote   rhythmNote;
    public Transform[]  notePos;

    [Header("Note")]
    public float        noteSpeed;


    [Header("Sound")]
    public AudioSource  audioSource;
    public AudioClip    beatsfx;

    public UnityAction OnRhythmHit;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        
    }

    private void Start()
    {
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
        while (true)
        {
            if (bpm < 10) bpm = 10;
            if(PhotonNetwork.Time >= prevTime + (double)(60f / bpm))
            {
                prevTime = PhotonNetwork.Time;
                CreateNote();
            }
            yield return null;
            //PhotonNetwork.Instantiate("Rhythm Note", notePos[0].position, Quaternion.identity);
            //photonView.RPC("CreateNote", RpcTarget.AllBuffered);
            //rhythmBox.RhythmHit();
            //yield return new WaitForSeconds(60f/ bpm); 
        }
    }
    [PunRPC]
    public void CreateNote()
    {
        RhythmNote note = Instantiate(rhythmNote, notePos[0].position, Quaternion.identity, notePos[0]);
        note.SetUp(rhythmBox.gameObject, 1f / noteSpeed);
        rhythmBox.RhythmHit();
        OnRhythmHit?.Invoke();
        isBeat = true;
    }


    


}
