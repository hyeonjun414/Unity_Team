using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;


public class SoundManager : Singleton<SoundManager>
{

    public AudioMixer mixer;
    public AudioSource bgSound;
    public AudioClip[] bgSoundlist;

    private void Awake()
    {
        if (_instance == null) _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        BGSoundPlay(bgSoundlist[0], 1f);
    }

    public void BGSoundVolume(float volume)
    {
        mixer.SetFloat("BGSoundVolume", Mathf.Log10(volume) * 20);
    }

    public void SFXVolume(float volume)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void BGSoundPlay(AudioClip clip, float delay)
    {
        bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BGSound")[0];
        bgSound.clip = clip;
        bgSound.loop = true;
        bgSound.volume = 0.2f;
        Invoke("Play", delay);
    }

    public void Play()
    {
        bgSound.Play();
    }
}
