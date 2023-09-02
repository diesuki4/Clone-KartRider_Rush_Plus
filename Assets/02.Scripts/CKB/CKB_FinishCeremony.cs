using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class CKB_FinishCeremony : MonoBehaviour
{
    public CKB_RankManager ckbRnkMgr;
    public PlayableDirector pd;
    public PlayableDirector pd2;
    public Transform vcams;
    public float[] timeChkPts;

    public Transform finishCeremony;
    public GameObject finCeremonyBG1Factory;
    public GameObject finCeremonyBG2Factory;
    public GameObject finCeremonyUI;
    public GameObject lapTimeUI;
    public Transform finalStatsUI;
    public Transform raceFailUI;
    public GameObject tenCount;

    CKB_Boost ckbBoost;
    CKB_PlayerDrive ckbPD;
    CKB_RiderRank ckbRdRank;

    bool isRaceFailed;

    void Start()
    {
        ckbBoost = CKB_Player.Instance.GetComponent<CKB_Boost>();
        ckbPD = CKB_Player.Instance.GetComponent<CKB_PlayerDrive>();
        ckbRdRank =CKB_Player.Instance.GetComponent<CKB_RiderRank>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
            StartRaceFailedCut();
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            StartFinishCeremony();
    }

    public void StartRaceFailedCut()
    {
        CKB_UIManager.Instance.DisableControlUI();
        CKB_Player.Instance.state = CKB_Player.State.End;
        CKB_AudioManager.Instance.StopAllAudio();
        CKB_AudioManager.Instance.StartAudio(CKB_AudioManager.AudioType.RaceFailed);

        isRaceFailed = true;

        StartCoroutine(IEStartRaceFailedCut());
        StartCoroutine(IEShowFinalStatsUI());
    }

    IEnumerator IEStartRaceFailedCut()
    {
        vcams.Rotate(0, 90, 0);
        pd2.Play();

        raceFailUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.1f);

        raceFailUI.gameObject.SetActive(false);

        yield return new WaitForSeconds(9.9f);

        CKB_UIManager.Instance.NextButtonSetActive(true);
    }

    public void StartFinishCeremony()
    {
        GameManager.Instance.StopCoroutine("StopCount");
        tenCount.SetActive(false);
        CKB_UIManager.Instance.DisableControlUI();
        CKB_Player.Instance.state = CKB_Player.State.End;
        CKB_AudioManager.Instance.StopAllAudio();
        CKB_AudioManager.Instance.PlayAudio(CKB_AudioManager.AudioType.FinishLinePass);
        
        pd.Play();

        StartCoroutine(IEGenerateBackgrounds(0));
        StartCoroutine(IEShowFinCeremonyUI(0));

        StartCoroutine(IEGenerateBackgrounds(2));
        StartCoroutine(IEShowFinCeremonyUI(2));
        
        StartCoroutine(IEShowFinalStatsUI());
    }

    IEnumerator IEGenerateBackgrounds(int type)
    {
        if (type == 2 && !ckbRdRank.IsBestTotalLapTime())
            yield break;

        float startTime = timeChkPts[0 + type];
        float endTime = timeChkPts[1 + type];
        float term = 0.02f;

        while (pd.time < endTime)
        {
            if (startTime <= pd.time)
            {
                Vector2 localscale = new Vector2(UnityEngine.Random.Range(16, 30), UnityEngine.Random.Range(2, 4));
                Vector2 startOffset = new Vector2(2000, UnityEngine.Random.Range(-100, 300));
                float speed = UnityEngine.Random.Range(12000, 24000);
                Color color = new Color(UnityEngine.Random.Range(200, 255), UnityEngine.Random.Range(170, 255),
                                        UnityEngine.Random.Range(0, 50), UnityEngine.Random.Range(130, 220)) / 255f;

                CKB_FinCeremonyBGEffect bgEff = Instantiate((type==0 ? finCeremonyBG1Factory : finCeremonyBG2Factory), finishCeremony).GetComponent<CKB_FinCeremonyBGEffect>();
                bgEff.InitializeParams(localscale, startOffset, speed, color);
            }
            
            yield return new WaitForSeconds(term);
        }
    }

    IEnumerator IEShowFinCeremonyUI(int type)
    {
        if (type == 2 && !ckbRdRank.IsBestTotalLapTime())
            yield break;

        float startTime = timeChkPts[0 + type];
        float endTime = timeChkPts[1 + type];
        float duration = endTime - startTime;

        RectTransform crmyTransform = finCeremonyUI.GetComponent<RectTransform>();
        RectTransform daoTrasform = crmyTransform.Find("Dao").GetComponent<RectTransform>();
        RectTransform trophyTrasform = crmyTransform.Find("Trophy").GetComponent<RectTransform>();
        string text = (type == 0 ? GetPlayerRankString() : "NEW RECORD");

        float speed = 18000;
        Vector2 ceremonyUIStartOffset = new Vector2(2000, 100);
        Vector2 ceremonyUIDestination = new Vector2((type == 0 ? -400 : 0), 100);

        RectTransform lapTransform = lapTimeUI.GetComponent<RectTransform>();
        Vector2 lapTimeUIStartOffset = new Vector2(-2000, 100);
        Vector2 lapTimeUIDestination = ceremonyUIDestination + Vector2.right * 165 + Vector2.up * -122;

        while (pd.time < endTime)
        {
            if (startTime <= pd.time)
            {
                if (!finCeremonyUI.activeSelf)
                {
                    crmyTransform.anchoredPosition = ceremonyUIStartOffset;
                    lapTransform.anchoredPosition = lapTimeUIStartOffset;

                    crmyTransform.Find("Text").GetComponent<TMP_Text>().text = text;
                    lapTransform.GetComponent<TMP_Text>().text = (type == 0 ? "" : FloatToTimeString(ckbRdRank.totalLapTime));

                    daoTrasform.anchoredPosition = Vector2.zero;
                    trophyTrasform.anchoredPosition = new Vector2(151, 58);

                    finCeremonyUI.SetActive(true);
                    lapTimeUI.SetActive(true);
                }

                if (endTime - duration * 0.2 < pd.time)
                {
                    crmyTransform.anchoredPosition += Vector2.left * speed * Time.deltaTime;
                    lapTransform.anchoredPosition += Vector2.right * speed * Time.deltaTime;
                }
                else if (startTime + duration * 0.2 <= pd.time && pd.time <= endTime - duration * 0.2 ||
                            crmyTransform.anchoredPosition.x <= ceremonyUIDestination.x)
                {
                    crmyTransform.anchoredPosition = ceremonyUIDestination;
                    lapTransform.anchoredPosition = lapTimeUIDestination;

                    Vector2 dir = Vector2.left * 1.3f;

                    daoTrasform.anchoredPosition += dir;
                    trophyTrasform.anchoredPosition += dir;
                }
            }

            yield return null;
        }
        
        finCeremonyUI.SetActive(false);
        lapTimeUI.SetActive(false);
    }

    IEnumerator IEShowFinalStatsUI()
    {
        float firstChkTime = (isRaceFailed ? 2.3f : ckbRdRank.IsBestTotalLapTime() ? timeChkPts[4] : 2.3f);
        float secondChkTime = (isRaceFailed ? 3.1f : ckbRdRank.IsBestTotalLapTime() ? timeChkPts[5] : 3.1f);
        float speed = 18000;
        ckbRdRank.IsBestTotalLapTime();

        RectTransform flag = finalStatsUI.Find("Racing Flag").GetComponent<RectTransform>();
        RectTransform rank = finalStatsUI.Find("Rank").GetComponent<RectTransform>();
        RectTransform totalLap = finalStatsUI.Find("Total Lap Time").GetComponent<RectTransform>();
        RectTransform lap = finalStatsUI.Find("Lap Time").GetComponent<RectTransform>();
        RectTransform avrSpeed = finalStatsUI.Find("Average Speed").GetComponent<RectTransform>();
        RectTransform drift = finalStatsUI.Find("Drift Count").GetComponent<RectTransform>();
        RectTransform booster = finalStatsUI.Find("Booster Count").GetComponent<RectTransform>();

        Vector2 flagDestination = new Vector2(1850, -90);
        Vector2 rankDestination = new Vector2(625, 350);
        Vector2 totalLapDestination = new Vector2(610, 215);
        Vector2 lapDestination = new Vector2(590, 130);
        Vector2 avrSpeedDestination = new Vector2(650, -35);
        Vector2 driftDestination = new Vector2(630, -140);
        Vector2 boosterDestination = new Vector2(560, -245);

        foreach (TMP_Text text in rank.GetComponentsInChildren<TMP_Text>())
        {
            text.text = (isRaceFailed ? "Fail" : GetPlayerRankString());
            text.color = (isRaceFailed ? Color.grey : text.color);
        }
        totalLap.GetComponent<TMP_Text>().text = (isRaceFailed ? "" : FloatToTimeString(ckbRdRank.totalLapTime));
        lap.GetComponent<TMP_Text>().text = (isRaceFailed ? "" : "1Lap " + FloatToTimeString(ckbRdRank.bestLapTime));
        avrSpeed.GetComponent<TMP_Text>().text = "AVR Speed   " + (ckbPD.sumSpeed / ckbPD.countSpeed * 3.6f * 0.5f) + " km/h";
        drift.GetComponent<TMP_Text>().text = "Drift   " + ckbPD.driftCount;
        booster.GetComponent<TMP_Text>().text = "Booster   " + ckbBoost.boostCount;

        yield return StartCoroutine(IEMoveStatUI(firstChkTime, null, Vector2.zero, speed, true));

        yield return StartCoroutine(IEMoveStatUI(firstChkTime + 0.1f, flag, flagDestination, speed, false));

        rank.anchoredPosition = rankDestination;

        while ((isRaceFailed ? pd2 : pd).time < firstChkTime + 0.4f)
        {
            float scale = Mathf.Lerp(20, 1, ((float)pd.time - firstChkTime + 0.1f) / 0.3f);

            rank.localScale = Vector3.Scale(Vector3.one, new Vector3(scale, scale, 1));

            yield return null;
        }
        
        rank.localScale = Vector3.one;

        yield return StartCoroutine(IEMoveStatUI(firstChkTime + 0.5f, totalLap, totalLapDestination, speed, false));
        yield return StartCoroutine(IEMoveStatUI(firstChkTime + 0.6f, lap, lapDestination, speed, false));

        yield return StartCoroutine(IEMoveStatUI(secondChkTime, null, Vector2.zero, speed, true));

        yield return StartCoroutine(IEMoveStatUI(secondChkTime + 0.1f, avrSpeed, avrSpeedDestination, speed, false));
        yield return StartCoroutine(IEMoveStatUI(secondChkTime + 0.2f, drift, driftDestination, speed, false));
        yield return StartCoroutine(IEMoveStatUI(secondChkTime + 0.3f, booster, boosterDestination, speed, false));

        yield return new WaitForSeconds(4.0f);

        CKB_UIManager.Instance.NextButtonSetActive(true);
  
    }

    IEnumerator IEMoveStatUI(float deadline, RectTransform rctTransform, Vector2 destination, float speed, bool timeOnly)
    {
        while ((isRaceFailed ? pd2 : pd).time < deadline)
        {
            if (!timeOnly)
                if (rctTransform.anchoredPosition.x <= destination.x)
                    rctTransform.anchoredPosition = destination;
                else
                    rctTransform.anchoredPosition += Vector2.left * speed * Time.deltaTime;

            yield return null;
        }
    }

    string GetPlayerRankString()
    {
        int rank = ckbRnkMgr.riders.Count(x => x.GetComponent<CKB_RiderRank>().currentLap == 3);
        string strRank = "";

        switch (rank)
        {
            case 1 : strRank = "1st";
                break;
            case 2 : strRank = "2nd";
                break;
            case 3 : strRank = "3rd";
                break;
            default : strRank = rank + "th";
                break;
        }

        return strRank;
    }

    string FloatToTimeString(float time)
    {
        return TimeSpan.FromSeconds(time).ToString("mm':'ss':'ff");
    }
}
