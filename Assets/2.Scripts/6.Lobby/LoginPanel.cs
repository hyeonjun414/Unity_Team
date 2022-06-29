using UnityEngine;
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
        if(DBManager.isLoginEmail)
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
        if(DBManager.isLoginEmail)
        {
            DBManager.Instance.ReadDB(DBManager.userID,"nickName",(str)=>{
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

        DBManager.Instance.NickNameDuplicateCheck(playerNameInput.text,(str)=>{
            if(str == "nullString")
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                LobbyManager.instance.ShowError("만들어진 계정에서 이미 사용중인 닉네임입니다");
            }
        });

    }
}
