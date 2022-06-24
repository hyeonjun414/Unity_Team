using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    public GameObject optionButton;
    public GameObject optionWindow;
    public Button apply;
    public Button cancel;
    public Toggle musicSoundButton;
    public Toggle effectSoundButton;
    public Slider musicSoundSlider;
    public Slider effectSoundSlider;

    float firstMusicValue;
    float firstEffectValue;
    float changeMusicValue = 0;
    float changeEffectValue = 0;

    bool firstMusicOn = true;
    bool firstEffectOn = true;

    private void Start()
    {
        optionWindow.SetActive(false);
    }

    public void OnClickOptionButton()
    {
        optionWindow.SetActive(true);
        firstMusicValue = musicSoundSlider.value;
        firstEffectValue = effectSoundSlider.value;
        changeMusicValue = firstMusicValue;
        changeEffectValue = firstEffectValue;

        musicSoundButton.isOn = firstMusicOn;
        effectSoundButton.isOn = firstEffectOn;
    }

    public void OnClickApply()
    {
        firstMusicValue = changeMusicValue;
        firstEffectValue = changeEffectValue;
        firstMusicOn = musicSoundButton.isOn;
        firstEffectOn = effectSoundButton.isOn;
        optionWindow.SetActive(false);
    }

    public void OnClickMusicSoundButton()
    {
        if (musicSoundButton.isOn == true)
        {
            SoundManager.Instance.BGSoundVolume(changeMusicValue);
        }
        else
        {
            SoundManager.Instance.BGSoundVolume(musicSoundSlider.minValue);
        }
    }

    public void OnClickEffectSoundButton()
    {
        if (effectSoundButton.isOn == true)
        {
            SoundManager.Instance.SFXVolume(changeEffectValue);
        }
        else
        {
            SoundManager.Instance.SFXVolume(musicSoundSlider.minValue);
        }
    }

    public void MusicSoundSlider()
    {
        changeMusicValue = musicSoundSlider.value;
        if (musicSoundButton.isOn)
        {
            SoundManager.Instance.BGSoundVolume(changeMusicValue);
        }
    }

    public void EffectSoundSlider()
    {
        changeEffectValue = effectSoundSlider.value;
        if (musicSoundButton.isOn)
        {
            SoundManager.Instance.SFXVolume(changeEffectValue);
        }

    }


    public void OnClickCancel()
    {
        musicSoundSlider.value = firstMusicValue;
        effectSoundSlider.value = firstEffectValue;
        musicSoundButton.isOn = firstMusicOn;
        effectSoundButton.isOn = firstEffectOn;
        optionWindow.SetActive(false);
    }

}
