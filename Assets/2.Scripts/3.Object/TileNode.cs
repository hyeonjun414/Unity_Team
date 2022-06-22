using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eTileOccupation
{
    EMPTY,
    PLAYER,
    OCCUPIED,
    //DEAD_END,
}

public class TileNode : MonoBehaviour
{
    public GameObject objectOnTile;         //?€?¼ì´ NULL???„ë‹ ???„ì— ?ˆëŠ” ?¤ë¸Œ?íŠ¸
    public eTileOccupation eOnTileObject;   //?€?¼ì˜ ?ìœ ?íƒœ (ë¹ˆìƒ??, ?Œë ˆ?´ì–´ê°€ ?„ì— ?ˆìŒ , ?„ì´?œì´ ?„ì— ?ˆìŒ)
    public Point tilePos = new Point();


    public eTileOccupation onTileObject;
    public int posX;
    public int posY;

    RaycastHit hit;

    private void Start()
    {
        if (Physics.Raycast(transform.position, Vector3.up, out hit, 10f))
        {
            eOnTileObject = eTileOccupation.OCCUPIED;
        }

    }
}
