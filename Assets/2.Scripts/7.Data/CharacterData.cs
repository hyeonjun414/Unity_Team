using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Data/CharacterDB")]
public class CharacterData : ScriptableObject
{
    [Header("캐릭터")]
    public GameObject[] players;
}

