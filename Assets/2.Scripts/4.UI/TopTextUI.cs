using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TopTextUI : MonoBehaviour
{
    // 시간제는 mode 0, 남은 플레이어 기반은 mode 1
    public int mode = 0;
    public TMP_Text topText;
    public TMP_Text bottomText;


    public void SetUp(ModeType type)
    {
        switch(type)
        {
            case ModeType.BattleRoyale:
            case ModeType.OneShot:
                mode = 0;
                topText.text = "남은 플레이어";
                bottomText.text = BattleManager.Instance.alivePlayer.Count.ToString();
                break;
            case ModeType.DeathMatch:
                mode = 1;
                topText.text = "남은 시간";
                bottomText.text = TimeManager.Instance.limitTime.ToString();
                break;
        }
    }

    public void UpdateUI()
    {
        if(mode == 0)
        {
            bottomText.text = BattleManager.Instance.alivePlayer.Count.ToString();
        }
        else
        {
            bottomText.text = Mathf.Round(TimeManager.Instance.limitTime).ToString();
        }
    }
}
