using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Option : MonoBehaviour
{
    [Header("Lobby Option")]

    public GameObject optionButton;
    public GameObject lobbyOptionWindow;
    public Button apply;
    public Button cancel;
    public Toggle musicSoundButton;
    public Toggle effectSoundButton;
    public Slider musicSoundSlider;
    public Slider effectSoundSlider;

    [Header("Game Option")]
    public GameObject goToLobby;
    public GameObject warningWindow;
    public Button warningApply;
    public Button warningCancel;

    [Header("Data")]
    public float firstMusicValue;
    public float firstEffectValue;
    public float changeMusicValue = 0;
    public float changeEffectValue = 0;

    public bool firstMusicOn = true;
    public bool firstEffectOn = true;

    private void Awake()
    {
        var obj = FindObjectsOfType<Option>();
        if (obj.Length == 1)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GotoLobbyActive();

            if (lobbyOptionWindow.activeInHierarchy == false)
            {
                lobbyOptionWindow.SetActive(true);
                firstMusicValue = musicSoundSlider.value;
                firstEffectValue = effectSoundSlider.value;
                changeMusicValue = firstMusicValue;
                changeEffectValue = firstEffectValue;

                musicSoundButton.isOn = firstMusicOn;
                effectSoundButton.isOn = firstEffectOn;
            }
            else
            {
                OnClickCancel();
            }
        }
    }

    public void GotoLobbyActive()
    {
        if (SceneManager.GetActiveScene().name == "mapTest")
        {
            optionButton.SetActive(false);
            goToLobby.SetActive(true);
        }
        else
        {
            optionButton.SetActive(true);
            goToLobby.SetActive(false);
        }
    }

    public void OnClickOptionButton()
    {
        GotoLobbyActive();

        lobbyOptionWindow.SetActive(true);
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
        lobbyOptionWindow.SetActive(false);
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
        lobbyOptionWindow.SetActive(false);
    }

    public void OnClickLobby()
    {
        warningWindow.SetActive(true);
    }

    public void OnClickWarningApply()
    {
        PhotonNetwork.LeaveRoom();

        warningWindow.SetActive(false);
        lobbyOptionWindow.SetActive(false);
    }

    public void OnClickWarningCancel()
    {
        warningWindow.SetActive(false);
        musicSoundSlider.value = firstMusicValue;
        effectSoundSlider.value = firstEffectValue;
        musicSoundButton.isOn = firstMusicOn;
        effectSoundButton.isOn = firstEffectOn;
    }
}
