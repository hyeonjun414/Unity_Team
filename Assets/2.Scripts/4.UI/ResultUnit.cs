using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultUnit : MonoBehaviour
{
    [Header("Rank Image")]
    public Sprite win;
    public Sprite lose;

    [Header("Rank")]
    public Image rankImage;
    public TMP_Text rankText;

    [Header("Name")]
    public TMP_Text nameText;
    [Header("Kill")]
    public TMP_Text killText;
    [Header("Death")]
    public TMP_Text deathText;
    [Header("Score")]
    public TMP_Text scoreText;

    public void SetUp(PlayerResultInfo info, int rank)
    {
        rankImage.sprite = rank == 0 ? win : lose;

        rankText.text = rank.ToString();
        nameText.text = info.name;
        killText.text = info.kill.ToString();
        deathText.text = info.death.ToString();
        scoreText.text = info.score.ToString();
    }

}
