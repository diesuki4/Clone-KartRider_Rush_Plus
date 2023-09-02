using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKB_CheckPoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        CKB_RiderRank ckbRdRank = null;

        if (CKB_Player.Instance.state != CKB_Player.State.Ready && (ckbRdRank = other.GetComponent<CKB_RiderRank>()))
        {
            int checkPointNum = int.Parse(transform.name.Substring(12, transform.name.Length - 13));

            if (checkPointNum == 56)
                ckbRdRank.is1stLapChkPtpassed = true;
            else if (ckbRdRank.is1stLapChkPtpassed && checkPointNum == 0)
                ckbRdRank.is2ndLapChkPtpassed = true;

            if (ckbRdRank.is1stLapChkPtpassed && ckbRdRank.is2ndLapChkPtpassed)
                ckbRdRank.IncreaseLap();

            ckbRdRank.lastTouched = (ckbRdRank.currentLap-1) * transform.parent.childCount + checkPointNum;

            if (other.gameObject == CKB_Player.Instance.gameObject &&
                ckbRdRank.currentLap == 2 &&
                checkPointNum == 0)
                CKB_AudioManager.Instance.PlayAudio(CKB_AudioManager.AudioType.LapChange);
        }
    }
}
