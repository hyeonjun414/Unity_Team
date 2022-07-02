using System.Collections;
using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using UnityEngine;

public class CharacterMove : MoveCommand
{
    public bool isMoving = false;
    private ePlayerInput curMoveDir;
    public override void Execute()
    {
        MoveToNode();
        
    }
    public void MoveToNode()
    {

        if (player.eCurInput == ePlayerInput.MOVE_LEFT)
        {
            MoveNextNode(new Point(0, -player.stat.playerMoveDistance));
        }
        else if (player.eCurInput == ePlayerInput.MOVE_RIGHT)
        {
            MoveNextNode(new Point(0, player.stat.playerMoveDistance));
        }
        else if (player.eCurInput == ePlayerInput.MOVE_UP)
        {
            MoveNextNode(new Point(-player.stat.playerMoveDistance, 0));
        }
        else if (player.eCurInput == ePlayerInput.MOVE_DOWN)
        {
            MoveNextNode(new Point(player.stat.playerMoveDistance, 0));
        }
    }
    private IEnumerator MoveRoutine(Point point)
    {
        yield return null;
        isMoving = true;
        TileNode originNode = player.curNode;
        TileNode destNode = null;
        if (MapManager.Instance.BoundaryCheck(player.stat.curPos, point))
        {
            destNode = MapManager.Instance.map.GetTileNode(player.stat.curPos + point);
        }
        else
        {
            point /= 2;
            if (player.stat.playerMoveDistance == 2 && MapManager.Instance.BoundaryCheck(player.stat.curPos, point))
            {
                destNode = MapManager.Instance.map.GetTileNode(player.stat.curPos + point);
            }
            else
            {
                destNode = MapManager.Instance.map.GetTileNode(player.stat.curPos);

            }
        }

        Vector3 middlePos = (originNode.transform.position + destNode.transform.position) * 0.5f + Vector3.up;

        float curTime = 0;
        while (true)
        {
            if (curTime > 0.2f)
                break;
            curTime += Time.deltaTime;
            transform.position = GetBezierPos(
                originNode.transform.position + Vector3.up,
                middlePos + Vector3.up,
                destNode.transform.position + Vector3.up,
                curTime / 0.2f);

            if(CollisionRayPlayer())
            {
                print($"충돌해서 돌아감 tilePos : {player.curNode.tilePos.y}, {player.curNode.tilePos.x}");
                photonView.RPC("CollidedPlayer", RpcTarget.All, player.curNode.tilePos.y, player.curNode.tilePos.x);
            }

            yield return null;
        }

        ChangeNode(destNode);

    }
    public void ChangeNode(TileNode nextNode)
    {
        player.curNode.eOnTileObject = eTileOccupation.EMPTY;
        player.curNode = nextNode;
        player.curNode.eOnTileObject = eTileOccupation.PLAYER;
        player.stat.curPos = nextNode.tilePos;
        isMoving = false;
    }

    public bool CollisionRayPlayer()
    {
        Vector3 dirVec = Vector3.zero;
        switch (curMoveDir)
        {
            case ePlayerInput.MOVE_UP:
                dirVec = transform.forward;
                break;
            case ePlayerInput.MOVE_DOWN:
                dirVec = -transform.forward;
                break;
            case ePlayerInput.MOVE_RIGHT:
                dirVec = transform.right;
                break;
            case ePlayerInput.MOVE_LEFT:
                dirVec = -transform.right;
                break;
        }

        Vector3 dirVecUp = Quaternion.Euler(0, 70, 0) * dirVec;
        Vector3 dirVecDown = Quaternion.Euler(0, -70, 0) * dirVec;

        RaycastHit hit;
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + dirVec, Color.red, 10f);
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + dirVecUp, Color.red, 10f);
        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + dirVecDown, Color.red, 10f);
        
        if (Physics.Raycast(transform.position + Vector3.up,
            dirVec, out hit, 1f,LayerMask.GetMask("Player")))
        {
            return true;
        }
        if (Physics.Raycast(transform.position + Vector3.up,
            dirVecUp, out hit, 1f, LayerMask.GetMask("Player")))
        {
            return true;
        }
        if (Physics.Raycast(transform.position + Vector3.up,
            dirVecDown, out hit, 1f, LayerMask.GetMask("Player")))
        {
            return true;
        }

        return false;
    }

    public void MoveNextNode(Point movePoint)
    {
        curMoveDir = player.eCurInput;
        Point resultDir = GetResultDir(movePoint);
        StartCoroutine("MoveRoutine", resultDir);
    }

    public Point GetResultDir(Point movePoint)
    {
        // 플레이어의 방향에 따른 이동 위치 차이를 계산하는 함수
        Point resultDir = new Point();
        switch (player.Dir)
        {
            case PlayerDir.Up:
                resultDir = new Point(movePoint.y, movePoint.x);
                break;
            case PlayerDir.Right:
                resultDir = new Point(movePoint.x, -movePoint.y);
                break;
            case PlayerDir.Down:
                resultDir = new Point(-movePoint.y, -movePoint.x);
                break;
            case PlayerDir.Left:
                resultDir = new Point(-movePoint.x, movePoint.y);
                break;
        }
        return resultDir;
    }
    public TileNode GetPlayerDest(Point result)
    {
        Point destPoint = player.stat.curPos + result;
        return MapManager.Instance.map.GetTileNode(destPoint); 
    }
    [PunRPC]
    public void CollidedPlayer(int y, int x)
    {
/*        if (!isMoving)
            return;*/
        
        StopCoroutine("MoveRoutine");
        StartCoroutine(ReturnPosRoutine(y, x));
    }

    IEnumerator ReturnPosRoutine(int y, int x)
    {
        yield return null;


        TileNode destNode = MapManager.Instance.map.GetTileNode(new Point(y, x));


        Vector3 middlePos = (transform.position + destNode.transform.position) * 0.5f;

        float curTime = 0;
        while (true)
        {
            if (curTime > 0.2f)
                break;
            curTime += Time.deltaTime;
            transform.position = GetBezierPos(
                transform.position,
                middlePos+ Vector3.up * 0.5f,
                destNode.transform.position + Vector3.up,
                curTime / 0.2f);

            yield return null;
        }
        isMoving = false;
    }



    private Vector3 GetBezierPos(Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 q1 = Vector3.Lerp(p1, p2, t);
        Vector3 q2 = Vector3.Lerp(p2, p3, t);

        return Vector3.Lerp(q1, q2, t);
    }
    

}
