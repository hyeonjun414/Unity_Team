using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmManager : Singleton<RhythmManager>
{
    [Header("UI")]
    public Text hitText;

    [Header("Beat")]
    public float bpm;
    public float timing;
    public float accuBeat;
    public float accuBeatInterval;

    [Header("Rhythm")]
    public RhythmBox  rhythmBox;
    public RhythmNote rhythmNote;
    public Transform[] notePos;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip beatsfx;


    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }


    private void Start()
    {
        StartCoroutine("RhythmRoutine");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            BitCheck(timing);
        }
    }

    public void BitCheck(float time)
    {
        if(accuBeat - accuBeatInterval < time && time < accuBeat + accuBeatInterval)
        {
            hitText.text = "HIT";
        }
        else
        {
            hitText.text = "MISS";
        }
    }

    IEnumerator RhythmRoutine()
    {
        yield return null;

        while (true)
        {
            if (bpm < 10) bpm = 10;
            RhythmNote note = Instantiate(rhythmNote, notePos[0].position, Quaternion.identity, notePos[0]);
            note.SetUp(rhythmBox.gameObject, 60 / bpm, accuBeat);
            
            yield return StartCoroutine(TimeCheckRoutine(60f/bpm));
        }
    }
    IEnumerator TimeCheckRoutine(float time)
    {
        float curTime = 0f;
        while(true)
        {
            if (curTime > time)
                break;
            curTime += Time.deltaTime;
            timing = curTime / time;
            yield return null;
        }
        
    }


    


}
