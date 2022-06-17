﻿using System.Collections;
using System.Collections.Generic;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using UnityEngine;

public class CharacterMove : MoveCommand
{

    public override void Execute()
    {
        if(Input.anyKeyDown && RhythmManager.Instance.BitCheck())
        {
            RhythmManager.Instance.rhythmBox.NoteHit();
            InputCommand();
         //   player.photonView.RPC()
            
        }
    }
    [PunRPC]
    public void SendCommandToMaster(int playerNumber , int command, int playerDestX, int playerDestY, int curPosX , int curPosY)
    {
        //command   1 == move
        //          2 == rotate
        //          3 == attack
        //          4 == useItem
        //          5 == changeSlot
        player.playerNumber = playerNumber;
        if(PhotonNetwork.IsMasterClient)
        {
            InputCheckManager.Instance.CachePlayersCommand(playerNumber,command,playerDestX,playerDestY,curPosX,curPosY);
        }
    }
    
    private void InputCommand()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            MoveCalculate(player.Dir, new Vector2(-1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            MoveCalculate(player.Dir, new Vector2(1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            MoveCalculate(player.Dir, new Vector2(0, -1));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            MoveCalculate(player.Dir, new Vector2(0, 1));
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            player.Dir++;
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            player.Dir--;
        }

    }
    private IEnumerator MoveRoutine(Vector2 resultDir)
    {
        //yield return new WaitUntil(()=>player.isInputAvailable);
        //if(adsasd)break;
        yield return null;
        //player.anim.SetTrigger("Jump");
        TileNode originNode = MapManager.Instance.grid[
            player.characterStatus.curPositionY,
            player.characterStatus.curPositionX];

        if(MapManager.Instance.BoundaryCheck(player.characterStatus.curPositionY,
            player.characterStatus.curPositionX, resultDir))
        {
            player.characterStatus.curPositionY += (int)resultDir.y;
            player.characterStatus.curPositionX += (int)resultDir.x;
        }

        TileNode destNode = MapManager.Instance.grid[
            player.characterStatus.curPositionY,
            player.characterStatus.curPositionX];

        print($"{player.characterStatus.curPositionY}, {player.characterStatus.curPositionX}");

        Vector3 middlePos = (originNode.transform.position + destNode.transform.position) * 0.5f + Vector3.up;
        
        float curTime = 0;
        yield return new WaitUntil(()=>player.isInputAvailable);
        while (true)
        {
            if (curTime > 0.2f)
                break;
            curTime += Time.deltaTime;
            transform.position = GetBezierPos(originNode.transform.position, middlePos, destNode.transform.position, curTime / 0.2f);

            yield return null;
        }
    }
    private IEnumerator MoveRoutine2(Vector2 resultDir)
    {
        yield return null;
        yield return new WaitUntil(()=>player.isInputAvailable);
        if(!player.isMoving)yield break;
        //player.anim.SetTrigger("Jump");
        TileNode originNode = MapManager_verStatic.Instance.map.GetTileNode(player.characterStatus.curPositionY,
            player.characterStatus.curPositionX);


        if (MapManager_verStatic.Instance.BoundaryCheck(player.characterStatus.curPositionY,
            player.characterStatus.curPositionX, resultDir))
        {
            player.characterStatus.curPositionY += (int)resultDir.y;
            player.characterStatus.curPositionX += (int)resultDir.x;
        }

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
    private void MoveCalculate(PlayerDir dir, Vector2 moveVec)
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
        //player.playerHeadingPos = resultDir;

        Vector2 dest = GetPlayerDest(resultDir);
        int playerNumber = PhotonNetwork.LocalPlayer.GetPlayerNumber();
        player.photonView.RPC(
            "SendCommandToMaster",RpcTarget.AllBuffered,new object[6]{playerNumber, 1,dest.x,dest.y,player.characterStatus.curPositionX,player.characterStatus.curPositionY});

        StartCoroutine(MoveRoutine2(resultDir));
    }
    public Vector2 GetPlayerDest(Vector2 result)
    {
        Vector2 destVec = new Vector2(player.characterStatus.curPositionX + result.x
                                        ,player.characterStatus.curPositionY + result.y);
        return destVec; 
    }
    private Vector3 GetBezierPos(Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 q1 = Vector3.Lerp(p1, p2, t);
        Vector3 q2 = Vector3.Lerp(p2, p3, t);

        return Vector3.Lerp(q1, q2, t);
    }
    

}
