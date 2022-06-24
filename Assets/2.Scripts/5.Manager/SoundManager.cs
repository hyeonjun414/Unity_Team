using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    public AudioMixer mixer;
    public AudioSource bgSound;
    public AudioClip[] bgSoundlist;



    private void Awake()
    {
        if (_instance == null) _instance = this;
    }

    private void Start()
    {
        BGSoundPlay(bgSoundlist[0]);
    }

    public void BGSoundVolume(float volume)
    {
        mixer.SetFloat("BGSoundVolume", Mathf.Log10(volume) * 20);
    }

    public void SFXVolume(float volume)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void SFXPlay(string sfxName, AudioClip clip)
    {
        GameObject go = new GameObject(sfxName + "Sound");
        AudioSource audioSource = go.GetComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        audioSource.clip = clip;
        audioSource.Play();

        Destroy(go, clip.length);
    }

    public void BGSoundPlay(AudioClip clip)
    {
        bgSound.outputAudioMixerGroup = mixer.FindMatchingGroups("BGSound")[0];
        bgSound.clip = clip;
        bgSound.loop = true;
        bgSound.volume = 0.5f;
        bgSound.Play();
    }

}
