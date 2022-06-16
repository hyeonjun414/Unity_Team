using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NickNameData", menuName = "Data/NickName")]
public class NickNameData : ScriptableObject
{
    [Header("수식어")]
    public string[] prefix;

    [Header("접미어")]
    public string[] suffix;
}
