using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmBox : MonoBehaviour
{
    public Animator anim;
    public BoxCollider2D coll;
    public bool isBeat = false;

    public void RhythmHit()
    {
        anim.SetTrigger("Hit");
        RhythmManager.Instance.audioSource.Play();
    }

    public void SetHitArea(float value)
    {
        coll.size *= value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Note")
        {
            RhythmHit();
            isBeat = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Note")
        {
            isBeat = false; 
        }
    }
}
