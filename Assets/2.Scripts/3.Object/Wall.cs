using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    public Material opaqueMat;
    public Material transparentMat;
    private TileNode curTile;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

    }
    private void Start()
    {
//        curTile = MapManager.Instance.map.GetTileNode(transform.position);
//        curTile.eOnTileObject = eTileOccupation.OCCUPIED;
    }
    public void UpdateMaterial(bool isTransparent)
    {
        if (isTransparent)
        {
            meshRenderer.material = transparentMat;
        }
        else
        {
            meshRenderer.material = opaqueMat;
        }
    }

}
