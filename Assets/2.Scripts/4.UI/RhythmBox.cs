using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmBox : MonoBehaviour
{
    public Animator anim;
    public BoxCollider2D coll;
    public bool isBeat = false;
    private RhythmNote curNote;

    public void RhythmHit()
    {
        anim.SetTrigger("Hit");
        RhythmManager.Instance.audioSource.Play();
    }

    public void SetHitArea(float value)
    {
        coll.size *= new Vector2(value, 1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Note")
        {
            //RhythmHit();
            curNote = collision.GetComponent<RhythmNote>();
            isBeat = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Note")
        {
            curNote = null;
            isBeat = false; 
        }
    }

    public void NoteHit()
    {
        if(curNote != null)
        {
            curNote.anim.SetTrigger("Hit");
            curNote = null;
        }
    }
}
