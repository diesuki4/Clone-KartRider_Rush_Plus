using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCount : MonoBehaviour
{
    public FinishCount Instance; //다른 스크립트에서 접근하기 쉽도록 스테틱 영역에 올려준다.

    public int lapCount = 0;            //AI가 총 몇바퀴 돌았는지 표시해준다.

    public float currTime = 0;

    public string lapTime;
    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        if (lapCount >= 1)
        {
            currTime += Time.deltaTime;
            if (lapCount >= 3)
            {
                currTime = 0;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Finish")
        {
            GameManager.Instance.finishCount += 1;
            lapCount += 1;
        }
        if (lapCount >= 3)
        {
            lapTime = string.Format("{0:00}:{1:00.00}", (int)(currTime / 60 % 60), currTime % 60);
            // print(lapTime);
        }
    }
}
