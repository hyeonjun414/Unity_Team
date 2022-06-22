using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameLogManager : Singleton<GameLogManager>
{
    public Queue<string> eventQueue;
    private void Awake()
    {
        if (_instance == null)
            _instance = this;


        eventQueue = new Queue<string>();   
    }

    //무슨 일 (죽거나 죽이거나 킬스트릭이나 특수한 일)이 일어나면 rpc로 전체에 쏴
    //=> 그걸 queue에 담아서 코루틴으로 n초마다 한번씩 화면에 표시 그리고 m초후에 queue에서 삭제 후 destroy
    //큐가 0보다 크면 코루틴을 돌려서 n초마다 한번씩 표시

    public void AddQueue(string msg)
    {
        eventQueue.Enqueue(msg);
        if(eventQueue.Count == 1)
        {
            StartCoroutine(ShowLogOnScreen(msg));
        }
    }
    IEnumerator ShowLogOnScreen(string msg)
    {
        while(eventQueue.Count > 0)
        {
            yield return new WaitForSeconds(0.5f);
            GameObject obj = Instantiate(Resources.Load<GameObject>("FloatingTextCanvas"),Vector3.zero,Quaternion.identity);
            TMP_Text text = (obj.transform.GetChild(0))?.GetComponent<TMP_Text>();
            text.text = msg;
            if(eventQueue.Count > 0) eventQueue.Dequeue();
            yield return new WaitForSeconds(1f);
            Destroy(obj);
        }
        
    }

}
