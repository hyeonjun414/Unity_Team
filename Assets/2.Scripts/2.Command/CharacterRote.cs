using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRote : RoteCommand
{

    public override void Execute()
    {
        PlayerRotation();
        
    }

    public void PlayerRotation()
    {
        if (player.eCurInput == ePlayerInput.ROTATE_RIGHT)
        {
            player.Dir++;
            SetDirection();
        }
        else if (player.eCurInput == ePlayerInput.ROTATE_LEFT)
        {
            player.Dir--;
            SetDirection();
        }

        
    }

    public void SetDirection()
    {
        float angle = 0f;
        switch (player.Dir)
        {
            case PlayerDir.Up:
                angle = 0f;
                break;
            case PlayerDir.Right:
                angle = 90f;
                break;
            case PlayerDir.Down:
                angle = 180f;
                break;
            case PlayerDir.Left:
                angle = 270f;
                break;
        }
        StartCoroutine(RotateRoutine(Quaternion.AngleAxis(angle, Vector3.up)));
    }
    IEnumerator RotateRoutine(Quaternion destRot)
    {
        Quaternion originRot = player.transform.rotation;
        float curTime = 0;
        while (true)
        {
            if (curTime > 0.2f)
                break;
            curTime += Time.deltaTime;
            player.transform.rotation = Quaternion.Slerp(originRot, destRot, curTime / 0.2f);
            yield return null;
        }
    }
}
