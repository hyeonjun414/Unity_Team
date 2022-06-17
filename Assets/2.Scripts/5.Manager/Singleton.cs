using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Singleton<T> : MonoBehaviourPun where T : MonoBehaviourPun
{

    protected static T _instance;
    public static T Instance
    {
        get { return _instance; }
    }

}
