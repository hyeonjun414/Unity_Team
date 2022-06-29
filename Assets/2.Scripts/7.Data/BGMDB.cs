using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BGMDataBase", menuName = "Data/BGMDB")]
public class BGMDB : ScriptableObject
{
    public BGMData[] bgms;
}
