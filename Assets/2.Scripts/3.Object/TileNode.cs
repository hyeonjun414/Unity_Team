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
    public GameObject objectOnTile;         //타일이 NULL이 아닐 시 위에 있는 오브젝트
    public eTileOccupation eOnTileObject;   //타일의 점유상태 (빈상태 , 플레이어가 위에 있음 , 아이템이 위에 있음)
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
