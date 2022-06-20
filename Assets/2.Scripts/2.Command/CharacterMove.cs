using System.Collections;
using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using UnityEngine;

public class CharacterMove : MoveCommand
{
    public bool isMoving = false;
    public override void Execute()
    {
        MoveToNode();
    }
    public void MoveToNode()
    {
        if (player.eCurInput == ePlayerInput.MOVE_LEFT)
        {
            MoveNextNode(new Point(0, -1));
        }
        else if (player.eCurInput == ePlayerInput.MOVE_RIGHT)
        {
            MoveNextNode(new Point(0, 1));
        }
        else if (player.eCurInput == ePlayerInput.MOVE_UP)
        {
            MoveNextNode(new Point(-1, 0));
        }
        else if (player.eCurInput == ePlayerInput.MOVE_DOWN)
        {
            MoveNextNode(new Point(1, 0));
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
            destNode = MapManager.Instance.map.GetTileNode(player.stat.curPos);
        }

        Vector3 middlePos = (originNode.transform.position + destNode.transform.position) * 0.5f + Vector3.up;
        Vector3 offset = Vector3.up * 0.5f;
        float curTime = 0;
        while (true)
        {
            if (curTime > 0.2f)
                break;
            curTime += Time.deltaTime;
            transform.position = GetBezierPos(
                originNode.transform.position + offset,
                middlePos + offset,
                destNode.transform.position + offset,
                curTime / 0.2f);

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
    public void MoveNextNode(Point movePoint)
    {
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

    public void CollidedPlayer()
    {
        if (!isMoving)
            return;

        StopCoroutine("MoveRoutine");
        StartCoroutine("ReturnPosRoutine");
    }

    IEnumerator ReturnPosRoutine()
    {
        yield return null;


        TileNode destNode = player.curNode;


        Vector3 middlePos = (transform.position + destNode.transform.position) * 0.5f + Vector3.up;
        Vector3 offset = Vector3.up * 0.5f;
        float curTime = 0;
        while (true)
        {
            if (curTime > 0.2f)
                break;
            curTime += Time.deltaTime;
            transform.position = GetBezierPos(
                transform.position + offset,
                middlePos + offset,
                destNode.transform.position + offset,
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
