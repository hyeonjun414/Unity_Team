using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmBox : MonoBehaviour
{
    public Animator anim;

    public void RhythmHit()
    {
        anim.SetTrigger("Hit");
        RhythmManager.Instance.audioSource.Play();
    }

}
