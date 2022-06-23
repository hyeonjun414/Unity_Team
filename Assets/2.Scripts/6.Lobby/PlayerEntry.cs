using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;
using TMPro;

public class PlayerEntry : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text playerNameText;
    public Image characterPanel;
    [Header("Character")]
    public CharacterData characterData;
    public GameObject characterSet;
    [HideInInspector]
    public int characterIndex;
    [HideInInspector]
    public int characterDataSize;
    public Button rightClickButton;
    public Button leftClickButton;

    [Header("Ready")]
    public GameObject readyImage;

    private int ownerId;
    private bool isPlayerReady;

    public void Start()
    {
        characterDataSize = characterData.players.Length;
        if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
        {
            rightClickButton.gameObject.SetActive(false);
            leftClickButton.gameObject.SetActive(false);
        }
        else
        {
            characterIndex = Random.Range(0, characterDataSize);
            SetPlayerCharacter(characterIndex);
            Hashtable props = new Hashtable() { { GameData.PLAYER_INDEX, characterIndex } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

    }
    public void OnRightButtonClicked()
    {
        if (isPlayerReady) return;
        //characterData.players[++characterIndex];

        ++characterIndex;
        if (characterIndex >= characterDataSize) characterIndex = 0;
        SetPlayerCharacter(characterIndex);
        Hashtable props = new Hashtable() { { GameData.PLAYER_INDEX, characterIndex } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

    }
    public void OnLeftButtonClicked()
    {
        if (isPlayerReady) return;

        --characterIndex;
        if (characterIndex <= 0) characterIndex = characterDataSize - 1;
        SetPlayerCharacter(characterIndex);
        Hashtable props = new Hashtable() { { GameData.PLAYER_INDEX, characterIndex } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void Initialize(int playerId, string playerName)
    {
        ownerId = playerId;
        playerNameText.text = playerName;
    }

    public void SetPlayerReadyImage(bool playerReady)
    {
        isPlayerReady = playerReady;
        readyImage.SetActive(playerReady);
    }

    public void SetPlayerCharacter(int index)
    {
        for (int i = 0; i < characterDataSize; ++i)
        {
            characterSet.transform.GetChild(i).gameObject.SetActive(false);
        }
        GameObject charac = characterSet.transform.GetChild(index).gameObject;
        charac.SetActive(true);
        DummyPlayer dummy = charac.GetComponent<DummyPlayer>();
        int randomAnim = Random.Range(1, 4);
        switch (randomAnim)
        {
            case 1: dummy.anim.SetTrigger("Wave Hand"); break;
            case 2: dummy.anim.SetTrigger("Clapping"); break;
            case 3: dummy.anim.SetTrigger("Victory"); break;
        }

        characterIndex = index;


    }
}
