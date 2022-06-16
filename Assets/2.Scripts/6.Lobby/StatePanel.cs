using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class StatePanel : MonoBehaviour
{
    public Text stateText;

    private ClientState state;
    
    void Update()
    {
        if (state == PhotonNetwork.NetworkClientState)
            return;

        state = PhotonNetwork.NetworkClientState;
        stateText.text = state.ToString();
        Debug.Log("PhotonNetwork State : " + state.ToString());
    }
}
