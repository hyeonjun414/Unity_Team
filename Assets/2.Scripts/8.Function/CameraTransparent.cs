using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransparent : MonoBehaviour
{
    Character player = null;
    List<Wall> cacheWalls= new List<Wall>();
    
    private void FixedUpdate()
    {
        CheckWall();

    }
    private void CheckWall()
    {
        if(player == null) return;
        Wall[] wall= new Wall[2];
        
       // Vector3 fixedPos = new Vector3(player.transform.position.x,player.transform.position.y+1f,transform.position.z);
        Vector3 dir = (player.transform.position - transform.position).normalized;
        RaycastHit[] target = Physics.RaycastAll(transform.position, dir,10f,LayerMask.GetMask("Wall"));
        if(target.Length>0)
        {
            for(int i=0; i<target.Length;++i)
            {
                wall[i]= target[i].collider.gameObject.GetComponent<Wall>();
                wall[i].UpdateMaterial(true);
                cacheWalls.Add(wall[i]);
            }
            
        }
        else
        {
            if(cacheWalls.Count!=0)
            {
                for(int i=0; i<cacheWalls.Count;++i)
                {
                    cacheWalls[i].UpdateMaterial(false);
                }
                cacheWalls.Clear();
            }


            
        }


    }
    public void SetPlayer(Character player)
    {
        this.player = player;
    }

    private void OnDrawGizmos()
    {
        if(player!=null) Debug.DrawLine(transform.position,player.transform.position,Color.red);
    }
}
