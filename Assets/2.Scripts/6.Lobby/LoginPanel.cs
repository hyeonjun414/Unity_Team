﻿using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class LoginPanel : MonoBehaviour
{
    public TMP_InputField playerNameInput;
    public NickNameData nameData;
    public Button loginButton;
    void Start()
    {
        if(DataBaseManager.isLoginEmail)
        {
            loginButton.gameObject.SetActive(false);
            playerNameInput.gameObject.SetActive(false);
        }
        else
        {
            playerNameInput.text = nameData.prefix[Random.Range(0, nameData.prefix.Length)] +" "+
            nameData.suffix[Random.Range(0, nameData.suffix.Length)];
        }

    }
    public void NickNameSet()
    {
        if(DataBaseManager.isLoginEmail)
        {
            DataBaseManager.Instance.ReadDB(DataBaseManager.userID,"nickName",(str)=>{
                PhotonNetwork.LocalPlayer.NickName = str;

                PhotonNetwork.ConnectUsingSettings();
            });
        }
    }

    public void OnLoginButtonClicked()
    {
        string playerName = playerNameInput.text;

        if (playerName == "")
        {
            LobbyManager.instance.ShowError("Invalid Player Name");
            return;
        }

        PhotonNetwork.LocalPlayer.NickName = playerName;
        PhotonNetwork.ConnectUsingSettings();
    }
}
