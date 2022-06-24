using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="MapData", menuName = "Data/Map")]
public class MapData : ScriptableObject
{
    [Header("Map")]
    public Map[] maps;
}
