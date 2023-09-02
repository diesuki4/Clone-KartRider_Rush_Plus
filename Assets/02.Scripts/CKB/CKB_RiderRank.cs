using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKB_RiderRank : MonoBehaviour
{
    public int lastTouched;
    public int currentLap;
    public bool is1stLapChkPtpassed;
    public bool is2ndLapChkPtpassed;

    public float bestLapTime;
    public float totalLapTime;

    float lapTime;

    IEnumerator Start()
    {
        //PlayerPrefs.DeleteKey("Best Total Lap Time");

        currentLap = 1;
        is1stLapChkPtpassed = false;
        is2ndLapChkPtpassed = false;

        bestLapTime = (float)(new TimeSpan(2, 0, 0, 0, 0).TotalSeconds);

        while (CKB_Player.Instance.state == CKB_Player.State.Ready)
            yield return null;

        while (currentLap < 3)
        {
            totalLapTime += Time.deltaTime;
            yield return null;
        }
    }

    void Update()
    {
        lapTime += Time.deltaTime;
    }

    public void IncreaseLap()
    {
        ++currentLap;
        is1stLapChkPtpassed = false;
        is2ndLapChkPtpassed = false;

        if (lapTime < bestLapTime)
            bestLapTime = lapTime;
        
        lapTime = 0;
    }

    public bool IsBestTotalLapTime()
    {
        bool isBestTotalLapTime = totalLapTime <= PlayerPrefs.GetFloat("Best Total Lap Time", (float)(new TimeSpan(2, 0, 0, 0, 0).TotalSeconds));

        if (isBestTotalLapTime)
        {
            PlayerPrefs.SetFloat("Best Total Lap Time", totalLapTime);
            PlayerPrefs.SetInt("Is Best Total Lap Time", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Is Best Total Lap Time", 0);
        }

        return isBestTotalLapTime;
    }
}
