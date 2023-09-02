using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFinish : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.tag=="Finish")
        {   

            if(GameManager.Instance.checkLine)
            {
                GameManager.Instance.checkLine = false;
                if (GameManager.Instance.lap > 0)
                {
                    GameManager.Instance.LapTime();

                }            
                GameManager.Instance.lap += 1;
                GameManager.Instance.finishCount += 1;
            }
            if (GameManager.Instance.lap >= 3)
            {
                GameManager.Instance.playerfinish = true; 
            }
        }

        if(other.gameObject.tag == "CheckLine")
        {
            GameManager.Instance.checkLine = true;
        }
    }
}
