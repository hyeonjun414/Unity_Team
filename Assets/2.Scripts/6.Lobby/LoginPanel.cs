using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class LoginPanel : MonoBehaviour
{
    public TMP_InputField playerNameInput;
    public NickNameData nameData;
    void Start()
    {
        //playerNameInput.text = "Player " + Random.Range(1000, 10000);
        playerNameInput.text = nameData.prefix[Random.Range(0, nameData.prefix.Length)] +" "+
            nameData.suffix[Random.Range(0, nameData.suffix.Length)];
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
