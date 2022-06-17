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
    }
    public void MoveToNode()
    {
        print("moveToNode");
        if (player.eCurInput == ePlayerInput.MOVE_LEFT)
        {
            MoveNextNode(player.Dir, new Vector2(-1, 0));
        }
        else if (player.eCurInput == ePlayerInput.MOVE_RIGHT)
        {
            MoveNextNode(player.Dir, new Vector2(1, 0));
        }
        else if (player.eCurInput == ePlayerInput.MOVE_UP)
        {
            MoveNextNode(player.Dir, new Vector2(0, -1));
        }
        else if (player.eCurInput == ePlayerInput.MOVE_DOWN)
        {
            MoveNextNode(player.Dir, new Vector2(0, 1));
        }
    }
    public TileNode NodeDetect()
    {
        print("NodeDetect");
        if (player.eCurInput == ePlayerInput.MOVE_LEFT)
        {
            return PreExcuteNextNode(player.Dir, new Vector2(-1, 0));
        }
        else if (player.eCurInput == ePlayerInput.MOVE_RIGHT)
        {
            return PreExcuteNextNode(player.Dir, new Vector2(1, 0));
        }
        else if (player.eCurInput == ePlayerInput.MOVE_UP)
        {
            return PreExcuteNextNode(player.Dir, new Vector2(0, -1));
        }
        else if (player.eCurInput == ePlayerInput.MOVE_DOWN)
        {
            return PreExcuteNextNode(player.Dir, new Vector2(0, 1));
        }

        return PreExcuteNextNode(player.Dir, new Vector2(0, 0));
    }
    private IEnumerator MoveRoutine2(Vector2 vec)
    {
        yield return null;
        //player.anim.SetTrigger("Jump");
        TileNode originNode = MapManager_verStatic.Instance.map.GetTileNode(player.characterStatus.curPositionY,
            player.characterStatus.curPositionX);


        player.characterStatus.curPositionX += (int)vec.x;
        player.characterStatus.curPositionY += (int)vec.y;

        TileNode destNode = MapManager_verStatic.Instance.map.GetTileNode(player.characterStatus.curPositionY,
            player.characterStatus.curPositionX);
        print($"{player.characterStatus.curPositionY}, {player.characterStatus.curPositionX}");

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
    public TileNode PreExcuteNextNode(PlayerDir dir, Vector2 moveVec)
    {
        Vector2 resultDir = Vector2.zero;
        switch (dir)
        {
            case PlayerDir.Up:
                resultDir = new Vector2(moveVec.x, moveVec.y);
                break;
            case PlayerDir.Right:
                resultDir = new Vector2(-moveVec.y, moveVec.x);
                break;
            case PlayerDir.Down:
                resultDir = new Vector2(-moveVec.x, -moveVec.y);
                break;
            case PlayerDir.Left:
                resultDir = new Vector2(moveVec.y, -moveVec.x);

                break;
        }
        if (MapManager_verStatic.Instance.BoundaryCheck(player.characterStatus.curPositionY,
             player.characterStatus.curPositionX, resultDir))
        {
            return GetPlayerDest(new Vector2(player.characterStatus.curPositionX + resultDir.x
                                        , player.characterStatus.curPositionY + resultDir.y));
        }
        else
        {
            return GetPlayerDest(new Vector2(player.characterStatus.curPositionX,
                player.characterStatus.curPositionY));
        }
    }

    public void MoveNextNode(PlayerDir dir, Vector2 moveVec)
    {
        Vector2 resultDir = Vector2.zero;
        switch (dir)
        {
            case PlayerDir.Up:
                resultDir = new Vector2(moveVec.x, moveVec.y);
                break;
            case PlayerDir.Right:
                resultDir = new Vector2(-moveVec.y, moveVec.x);
                break;
            case PlayerDir.Down:
                resultDir = new Vector2(-moveVec.x, -moveVec.y);
                break;
            case PlayerDir.Left:
                resultDir = new Vector2(moveVec.y, -moveVec.x);

                break;
        }
        StartCoroutine(MoveRoutine2(resultDir));
    }
    public TileNode GetPlayerDest(Vector2 result)
    {
        Vector2 destVec = new Vector2(player.characterStatus.curPositionX + result.x
                                        ,player.characterStatus.curPositionY + result.y);
        return MapManager_verStatic.Instance.map.GetTileNode(result); 
    }
    public TileNode GetPlayerMoveNode(Vector2 result)
    {

        Vector2 destVec = new Vector2(player.characterStatus.curPositionX
                                , player.characterStatus.curPositionY);
        return MapManager_verStatic.Instance.map.GetTileNode(destVec);
    }
    private Vector3 GetBezierPos(Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 q1 = Vector3.Lerp(p1, p2, t);
        Vector3 q2 = Vector3.Lerp(p2, p3, t);

        return Vector3.Lerp(q1, q2, t);
    }
    

}
