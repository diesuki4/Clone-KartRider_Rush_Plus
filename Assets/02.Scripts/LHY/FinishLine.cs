using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public static FinishLine Instance;

    public string names;

    //int chack = 0;
    //int C = 0;
    public void OnTriggerEnter(Collider collider)
    {
       /* if(collider.gameObject)
        {
            C = C + chack;
            chack++;
            if (chack > C)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (GameManager.Instance.finishCount >= 17)
                    {
                        GameManager.Instance.names[i] = collider.gameObject.name;
                    }
                }
            }
        }*/
        
       
        if (GameManager.Instance.finishCount == 16)
        {
            GameManager.Instance.names[0] = collider.gameObject.name;                   
        }
        if (GameManager.Instance.finishCount == 17)
        {
            GameManager.Instance.names[1] = collider.gameObject.name;
        }
        if (GameManager.Instance.finishCount == 18)
        {
            GameManager.Instance.names[2] = collider.gameObject.name;
        }
        if (GameManager.Instance.finishCount == 19)
        {
            GameManager.Instance.names[3] = collider.gameObject.name;
        }
        if (GameManager.Instance.finishCount == 20)
        {
            GameManager.Instance.names[4] = collider.gameObject.name;
        }
        if (GameManager.Instance.finishCount == 21)
        {
            GameManager.Instance.names[5] = collider.gameObject.name;
        }
        if (GameManager.Instance.finishCount == 22)
        {
            GameManager.Instance.names[6] = collider.gameObject.name;
        }
        if (GameManager.Instance.finishCount == 23)
        {
            GameManager.Instance.names[7] = collider.gameObject.name;
        }


        if (CKB_Player.Instance.GetComponent<CKB_RiderRank>().currentLap == 3 && CKB_Player.Instance.state != CKB_Player.State.End)
            CKB_UIManager.Instance.GetComponent<CKB_FinishCeremony>().StartFinishCeremony();
    }
}
