using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmNote : MonoBehaviour
{

    public float time;
    public float timing;
    Vector3 destPos;

    public void SetUp(GameObject dest, float time, float timing)
    {
        destPos = dest.transform.position;
        this.time = time;
        this.timing = timing;
        StartCoroutine("MoveRoutine");
    }

    IEnumerator MoveRoutine()
    {
        yield return null;

        float curTime = 0;
        bool isTiming = false;
        float timingtemp;
        Vector3 originPos = transform.position;
        while(true)
        {
            
            if (curTime >= time)
                break;
            curTime += Time.deltaTime;
            timingtemp = curTime / time;
            if(!isTiming && timingtemp >= timing)
            {
                isTiming = true;
                RhythmManager.Instance.rhythmBox.RhythmHit();
            }
            transform.position = Vector3.Lerp(originPos, destPos, timingtemp);
            yield return null;
        }

        DestroySelf();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
