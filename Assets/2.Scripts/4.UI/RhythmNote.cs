using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmNote : MonoBehaviour
{

    public float time;
    public float timing;
    public Animator anim;
    Vector3 destPos;

    public void SetUp(GameObject dest, float time)
    {
        destPos = dest.transform.position;
        this.time = time;
        //StartCoroutine("MoveRoutine");
    }

    private void Update()
    {
        transform.Translate(Vector3.right * 200 * Time.deltaTime);
    }

    IEnumerator MoveRoutine()
    {
        yield return null;

        float curTime = 0;
        Vector3 originPos = transform.position;
        while(true)
        {
            if (curTime >= time)
                break;
            curTime += Time.deltaTime;
            transform.position = Vector3.Lerp(originPos, destPos, curTime/time);
            yield return null;
        }

        DestroySelf();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "NoteInit")
        {
            DestroySelf();
        }
    }

    public void DestroySelf()
    {
        
        BattleManager.Instance.Judge();
        Destroy(gameObject);
    }


}
