using System.Collections;
using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using UnityEngine;

public class CharacterMove : MoveCommand
{

    public override void Execute()
    {
        MoveToNode();
        player.eCurInput = ePlayerInput.NULL;
    }
    public void MoveToNode()
    {
        print("moveToNode");
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
    public TileNode NodeDetect()
    {
        print("NodeDetect");
        if (player.eCurInput == ePlayerInput.MOVE_LEFT)
        {
            return PreExcuteNextNode(new Point(0, -1));
        }
        else if (player.eCurInput == ePlayerInput.MOVE_RIGHT)
        {
            return PreExcuteNextNode(new Point(0, 1));
        }
        else if (player.eCurInput == ePlayerInput.MOVE_UP)
        {
            return PreExcuteNextNode(new Point(-1, 0));
        }
        else if (player.eCurInput == ePlayerInput.MOVE_DOWN)
        {
            return PreExcuteNextNode(new Point(1, 0));
        }

        return PreExcuteNextNode(new Point(0, 0));
    }
    private IEnumerator MoveRoutine(Point point)
    {
        yield return null;
        //player.anim.SetTrigger("Jump");
        TileNode originNode = MapManager_verStatic.Instance.map.GetTileNode(player.stat.curPos);

        print(player.stat.curPos.ToString());
        print(point.ToString());
        if (MapManager_verStatic.Instance.BoundaryCheck(player.stat.curPos, point))
        {
            player.stat.curPos += point;
        }

        print(player.stat.curPos.ToString());
        TileNode destNode = MapManager_verStatic.Instance.map.GetTileNode(player.stat.curPos);
        

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
    }
    public TileNode PreExcuteNextNode(Point movePoint)
    {
        Point resultDir = GetResultDir(movePoint);

        if (MapManager_verStatic.Instance.BoundaryCheck(player.stat.curPos, resultDir))
        {
            return GetPlayerDest(resultDir);
        }
        else
        {
            return GetPlayerDest(new Point(0,0));
        }
    }

    public void MoveNextNode(Point movePoint)
    {
        Point resultDir = GetResultDir(movePoint);
        StartCoroutine(MoveRoutine(resultDir));
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
        return MapManager_verStatic.Instance.map.GetTileNode(destPoint); 
    }

    private Vector3 GetBezierPos(Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 q1 = Vector3.Lerp(p1, p2, t);
        Vector3 q2 = Vector3.Lerp(p2, p3, t);

        return Vector3.Lerp(q1, q2, t);
    }
    

}
