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

    public float tempMusicValue;
    public float tempEffectValue;
    public float curMusicValue;
    public float curEffectValue;

    private void Start()
    {
        optionWindow.SetActive(false);
    }

    public void OnClickOptionButton()
    {
        optionWindow.SetActive(true);
    }

    public void OnClickApply()
    {
        optionWindow.SetActive(false);
        // TODO : 현재 슬라이드의 값을 음악,효과음 사운드로 설정
    }

    public void OnClickMusicSoundButton()
    {
        if (musicSoundButton.isOn == true)
        {
            musicSoundSlider.value = tempMusicValue;
        }
        else
        {
            musicSoundSlider.value = 0;
        }
        // TODO : 음악 사운드 0으로 설정
    }

    public void OnClickEffectSoundButton()
    {
        if (effectSoundButton.isOn == true)
        {
            effectSoundSlider.value = tempEffectValue;
        }
        else
        {
            effectSoundSlider.value = 0;
        }
        // TODO : 효과음 사운드 0으로 설정
    }

    public void MusicSoundSlider()
    {
        tempMusicValue = curMusicValue;

        curMusicValue = musicSoundSlider.value;
        // TODO : 음악 슬라이드 크기에 따라 사운드 크기 변경
    }

    public void EffectSoundSlider()
    {
        tempEffectValue = curEffectValue;

        curEffectValue = effectSoundSlider.value;
        // TODO : 효과음 슬라이드 크기에 따라 사운드 크기 변경
    }


    public void OnClickCancel()
    {

        optionWindow.SetActive(false);
    }

}
