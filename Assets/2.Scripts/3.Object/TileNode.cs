using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eTileOccupation
{
    EMPTY,
    PLAYER,
    ITEM,
    //DEAD_END,
}
public class TileNode : MonoBehaviour
{
    public GameObject objectOnTile;
    public eTileOccupation eOnTileObject;
    public int posX;
    public int posY;
}
