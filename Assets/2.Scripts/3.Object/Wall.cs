using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private TileNode curTile;

    private void Start()
    {
        curTile = MapManager.Instance.map.GetTileNode(transform.position);
        curTile.eOnTileObject = eTileOccupation.ITEM;
    }

}
