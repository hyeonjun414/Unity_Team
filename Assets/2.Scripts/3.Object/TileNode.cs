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
    public GameObject objectOnTile;         //?�?�이 NULL???�닐 ???�에 ?�는 ?�브?�트
    public eTileOccupation eOnTileObject;   //?�?�의 ?�유?�태 (빈상??, ?�레?�어가 ?�에 ?�음 , ?�이?�이 ?�에 ?�음)
    public Point tilePos = new Point();


    public eTileOccupation onTileObject;
    public int posX;
    public int posY;

    RaycastHit hit;

    private void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.up, 2f, LayerMask.GetMask("Wall")))
        {
            this.eOnTileObject = eTileOccupation.OCCUPIED;
        }

        // Debug.DrawRay(transform.position, Vector3.up * 2f, Color.red);
    }
}
