using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RhythmNote : MonoBehaviourPun
{

    public float time;
    public float velocity;
    public float timing;
    public Animator anim;
    public BoxCollider2D coll;
    Vector3 destPos;

    private void Start()
    {
        transform.SetParent(RhythmManager.Instance.notePos[0], true);
    }
    public void SetUp(Transform start,GameObject dest, float time)
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.one;
        transform.position = start.position;
        destPos = dest.transform.position;
        this.time = time;
        coll.enabled = true;
        velocity = Vector3.Distance(destPos, transform.position) / time;
    }

    private void Update()
    {
        transform.Translate(Vector3.right * velocity * Time.deltaTime);
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
        //RhythmManager.Instance.isBeat = true;
        //BattleManager.Instance.Judge();
        ReturnObj();
    }

    public void ReturnObj()
    {
        gameObject.SetActive(false);
    }


}
