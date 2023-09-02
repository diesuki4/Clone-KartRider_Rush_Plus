
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CKB_UIManager : MonoBehaviour
{
    public static CKB_UIManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public GameObject _Canvas;
    public CKB_RespawnZone respawnPlane;
    public Transform checkPoints;

    GameObject leftButton;
    GameObject rightButton;
    TMP_Text currentStateText;
    GameObject speed;
    GameObject resetButton;
    GameObject driftButton;
    GameObject brakeButton;
    GameObject turboBoostButton;
    GameObject mmtBoostButton;
    GameObject normalBoostButton1;
    GameObject normalBoostButton2;
    GameObject normalBoostGaugeBG;
    GameObject normalBoostGaugeSlider;
    GameObject emptySpace;
    GameObject touchBoostGauge1;
    GameObject touchBoostGauge2;
    Transform rank;
    Transform finCeremonyUI;
    Transform lapTimeUI;
    Transform finalStatsUI;
    Transform raceFailUI;
    GameObject nextButton;
    GameObject stopNotifier;

    Dictionary<string, Sprite> riderProfiles;

    CKB_Boost ckbBoost;
    CKB_PlayerDrive ckbPD;
    AudioSource asrc;

    float o_opacity;
    
    void Start()
    {
        leftButton = _Canvas.transform.Find("Left").gameObject;
        rightButton = _Canvas.transform.Find("Right").gameObject;
        currentStateText = _Canvas.transform.Find("Current State").GetComponent<TMP_Text>();
        speed = _Canvas.transform.Find("Speed").gameObject;
        resetButton = _Canvas.transform.Find("Reset").gameObject;
        driftButton = _Canvas.transform.Find("Drift").gameObject;
        normalBoostGaugeSlider = _Canvas.transform.Find("Boost Gauge").gameObject;
        normalBoostGaugeBG = _Canvas.transform.Find("Boost Gauge Background").gameObject;
        brakeButton = _Canvas.transform.Find("Brake").gameObject;
        turboBoostButton = _Canvas.transform.Find("Turbo Boost").gameObject;
        mmtBoostButton = _Canvas.transform.Find("MMT Boost").gameObject;
        normalBoostButton1 = _Canvas.transform.Find("Normal Boost 1").gameObject;
        normalBoostButton2 = _Canvas.transform.Find("Normal Boost 2").gameObject;
        emptySpace = _Canvas.transform.Find("Empty Space").gameObject;
        touchBoostGauge1 = _Canvas.transform.Find("Touch Boost Gauge 1").gameObject;
        touchBoostGauge2 = _Canvas.transform.Find("Touch Boost Gauge 2").gameObject;
        rank = _Canvas.transform.Find("Rank");
        finCeremonyUI = _Canvas.transform.Find("Finish Ceremony UI");
        lapTimeUI = _Canvas.transform.Find("Lap Time UI");
        finalStatsUI = _Canvas.transform.Find("Final Stats UI");
        raceFailUI = _Canvas.transform.Find("Race Fail UI");
        nextButton = _Canvas.transform.Find("Next Button").gameObject;
        stopNotifier = _Canvas.transform.Find("Stop Notifier").gameObject;

        string[] riderNames = {"Player", "AI_A", "AI_B", "AI_C", "AI_D", "AI_E", "AI_F", "AI_G"};
        riderProfiles = new Dictionary<string, Sprite>();

        foreach (string name in riderNames)
            riderProfiles.Add(name, Resources.Load<Sprite>("Riders/PP_" + name));

        ckbBoost = CKB_Player.Instance.GetComponent<CKB_Boost>();
        ckbPD = CKB_Player.Instance.GetComponent<CKB_PlayerDrive>();
        asrc = CKB_Player.Instance.GetComponent<AudioSource>();

        o_opacity = driftButton.transform.GetComponentInChildren<Image>().color.a;

        StartCoroutine("IETurboBoostTimer");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            LeftButtonDown();
        if (Input.GetKeyUp(KeyCode.A))
            LeftButtonUp();
        if (Input.GetKeyDown(KeyCode.D))
            RightButtonDown();
        if (Input.GetKeyUp(KeyCode.D))
            RightButtonUp();

        SetNormalBoostGaugeValue();
        DriftButtonOpacityController();
        NormalBoostButtonInteractableController();
        TouchBoostGaugeController();
        SetCurrentSpeedText();
        SetCurrentStateText();
        UpdateSpeedNotifier();
        StopNotifierController();
        DriftButton();
    }

    public void LeftButtonDown()
    {
        leftButton.transform.GetChild(0).gameObject.SetActive(true);
        leftButton.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void LeftButtonUp()
    {
        leftButton.transform.GetChild(0).gameObject.SetActive(false);
        leftButton.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void RightButtonDown()
    {
        rightButton.transform.GetChild(0).gameObject.SetActive(true);
        rightButton.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void RightButtonUp()
    {
        rightButton.transform.GetChild(0).gameObject.SetActive(false);
        rightButton.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void ResetButtonDown()
    {
        respawnPlane.Reset(CKB_Player.Instance.gameObject);
    }

    bool isDriftButtonDown;

    public void DriftButtonDown()
    {
        isDriftButtonDown = true;

        if (!ckbPD.isDriftAvailable || !(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) || !ckbPD.GetComponent<CharacterController>().isGrounded)
            return;

        driftButton.transform.GetChild(0).gameObject.SetActive(false);
        driftButton.transform.GetChild(1).gameObject.SetActive(true);

        ckbPD.o_tmpState = CKB_Player.Instance.state;
        CKB_Player.Instance.state = CKB_Player.State.Drift;

        ++ckbPD.driftCount;

        CKB_AudioManager.Instance.PlayAudio(CKB_AudioManager.AudioType.Drift);
    }

    void DriftButton()
    {
        if (isDriftButtonDown && CKB_Player.Instance.state != CKB_Player.State.Drift && CKB_Player.Instance.state != CKB_Player.State.End)
            DriftButtonDown();
    }

    public void DriftButtonUp()
    {
        isDriftButtonDown = false;

        if (!ckbPD.isDriftAvailable)
            return;

        driftButton.transform.GetChild(0).gameObject.SetActive(true);
        driftButton.transform.GetChild(1).gameObject.SetActive(false);

        CKB_Player.Instance.state = ckbPD.o_tmpState;

        CKB_AudioManager.Instance.StopAudio(CKB_AudioManager.AudioType.Drift);
    }

    public void BrakeButtonDown()
    {
        brakeButton.transform.GetChild(0).gameObject.SetActive(false);
        brakeButton.transform.GetChild(1).gameObject.SetActive(true);
        ckbPD.isBrake = true;
    }

    public void BrakeButtonUp()
    {
        brakeButton.transform.GetChild(0).gameObject.SetActive(true);
        brakeButton.transform.GetChild(1).gameObject.SetActive(false);
        ckbPD.isBrake = false;
    }

    public void TurboBoostButtonDown()
    {
        StopCoroutine("IETurboBoostTimer");

        ckbBoost.Boost(CKB_Player.State.TurboBoost);
        CKB_AudioManager.Instance.PlayAudio(CKB_AudioManager.AudioType.NormalBooster);

        InitializeDriveModeUI();
    }

    IEnumerator IETurboBoostTimer()
    {
        yield return new WaitForSeconds(GameManager.Instance.readyTime);
        turboBoostButton.SetActive(true);
        yield return new WaitForSeconds(ckbBoost.turboBoostAvailableTime);
        InitializeDriveModeUI();
    }

    void InitializeDriveModeUI()
    {
        foreach (Transform tr in _Canvas.transform)
            tr.gameObject.SetActive(true);

        turboBoostButton.SetActive(false);
        normalBoostButton1.SetActive(false);
        normalBoostButton2.SetActive(false);
        currentStateText.gameObject.SetActive(false);
        finCeremonyUI.gameObject.SetActive(false);
        lapTimeUI.gameObject.SetActive(false);
        raceFailUI.gameObject.SetActive(false);
        nextButton.SetActive(false);
        stopNotifier.SetActive(false);
    }

    public void MMTBoostButtonSetActive(bool active)
    {
        mmtBoostButton.transform.GetChild(0).gameObject.SetActive(!active);
        mmtBoostButton.transform.GetChild(1).gameObject.SetActive(active);
    }

    public void MMTBoostButtonDown()
    {
        ckbBoost.MMTBoostExecute();
    }

    void DriftButtonOpacityController()
    {
        foreach (Image img in driftButton.transform.GetComponentsInChildren<Image>())
        {
            Color clr = img.color;
            clr.a = ckbPD.isDriftAvailable ? o_opacity : 0.4f;
            img.color = clr;
        }
    }

    public void NormalBoostButtonSetActive(int numBtn, bool active)
    {
        GameObject button = (numBtn == 1 ? normalBoostButton1 : normalBoostButton2);

        button.SetActive(active);
    }

    void NormalBoostButtonInteractableController()
    {
        bool interactable = ckbBoost.isNormalBoostAvailable;

        normalBoostButton1.GetComponent<Button>().interactable = interactable;
        normalBoostButton2.GetComponent<Button>().interactable = interactable;

        Image img1 = normalBoostButton1.GetComponent<Image>();
        Image img2 = normalBoostButton2.GetComponent<Image>();

        Color clr = img1.color;
        clr.a = interactable ? o_opacity : 0.4f;

        img1.color = clr;
        img2.color = clr;
    }

    public void NormalBoostButtonDown()
    {
        ckbBoost.NormalBoostExecute();
    }

    void SetNormalBoostGaugeValue()
    {
        normalBoostGaugeSlider.GetComponent<Slider>().value = ckbBoost.normalBoostGauge / ckbBoost.normalBoostTotalGauge;
    }

    public void TouchBoostSpaceDown()
    {
        ckbBoost.SetTouchBoostGauge(++ckbBoost.touchBoostGauge);
    }

    void TouchBoostGaugeController()
    {
        int gauge = ckbBoost.touchBoostGauge - ckbBoost.touchBoostGaugeUIMinCount;

        for (int i = 0; i < 3; ++i)
        {
            touchBoostGauge1.transform.GetChild(i).gameObject.SetActive(i == gauge);
            touchBoostGauge2.transform.GetChild(i).gameObject.SetActive(i == gauge);
        }
    }

    void SetCurrentSpeedText()
    {
        float currentSpeedKMH = ckbPD.currentSpeed * 3.6f * 0.5f;

        speed.GetComponentInChildren<TMP_Text>().text = $"{((int)currentSpeedKMH).ToString().PadLeft(3, ' ')}.{(int)(currentSpeedKMH%1.0f*10)}" + " km/h";
    }

    void UpdateSpeedNotifier()
    {
        GameObject[] objs = new GameObject[4];

        float value = (ckbPD.currentSpeed * 3.6f * 0.5f - 94) * 0.02f;

        if ((CKB_Player.Instance.state & (CKB_Player.State.NormalBoost | CKB_Player.State.MMTBoost |
            CKB_Player.State.TouchBoost | CKB_Player.State.TurboBoost |
            CKB_Player.State.BoostBox | CKB_Player.State.JumpBox)) != 0 ||
            (ckbPD.o_tmpState & (CKB_Player.State.NormalBoost | CKB_Player.State.MMTBoost |
            CKB_Player.State.TouchBoost | CKB_Player.State.TurboBoost |
            CKB_Player.State.BoostBox | CKB_Player.State.JumpBox)) != 0)
        {
            objs[0] = speed.transform.GetChild(4).gameObject;
            objs[1] = speed.transform.GetChild(5).gameObject;
            objs[2] = speed.transform.GetChild(2).gameObject;
            objs[3] = speed.transform.GetChild(3).gameObject;
        }
        else
        {
            objs[0] = speed.transform.GetChild(2).gameObject;
            objs[1] = speed.transform.GetChild(3).gameObject;
            objs[2] = speed.transform.GetChild(4).gameObject;
            objs[3] = speed.transform.GetChild(5).gameObject;
        }

        objs[0].SetActive(true);
        objs[1].SetActive(true);
        objs[2].SetActive(false);
        objs[3].SetActive(false);

        objs[0].GetComponent<Image>().fillAmount = value;
        objs[1].GetComponent<Image>().fillAmount = value;
    }

    void SetCurrentStateText()
    {
        currentStateText.text = CKB_Player.Instance.state.ToString();
    }

    public void UpdateRankUI(GameObject[] sortedRiders)
    {
        int idxPlayer = Array.IndexOf(sortedRiders, CKB_Player.Instance.gameObject);

        float posY = 0;

        for (int i = 0; i < rank.transform.childCount; ++i)
        {
            RectTransform rt = rank.transform.GetChild(i).GetComponent<RectTransform>();

            rt.anchoredPosition = new Vector2((i == idxPlayer ? (1.4f*300 - 300) / 2 : 0), posY);

            rt.localScale = (i == idxPlayer ? new Vector3(1.4f, 1.4f, 1f) : new Vector3(1, 1, 1));

            rt.GetChild(1).gameObject.SetActive(i == idxPlayer);

            posY -= rt.rect.height * rt.localScale.y + 5;

            rt.GetChild(0).GetComponent<Image>().color = new Color(0, (i == idxPlayer ? 180 : 0), (i == idxPlayer ? 255 : 0), 105) / 255f;

            TMP_Text text = rt.Find("Name").GetComponent<TMP_Text>();
            
            text.text = sortedRiders[i].name;

            text.color = new Color(255, 255, (i == idxPlayer ? 0 : 255), 255) / 255f;

            Image image = rt.Find("Profile").GetComponent<Image>();

            image.sprite = riderProfiles[sortedRiders[i].name];
        }
    }

    public void DisableControlUI()
    {
        leftButton.SetActive(false);
        rightButton.SetActive(false);
        currentStateText.gameObject.SetActive(false);
        speed.SetActive(false);
        resetButton.SetActive(false);
        driftButton.SetActive(false);
        normalBoostGaugeSlider.SetActive(false);
        normalBoostGaugeBG.SetActive(false);
        brakeButton.SetActive(false);
        turboBoostButton.SetActive(false);
        mmtBoostButton.SetActive(false);
        normalBoostButton1.SetActive(false);
        normalBoostButton2.SetActive(false);
        emptySpace.SetActive(false);
        touchBoostGauge1.SetActive(false);
        touchBoostGauge2.SetActive(false);
        rank.gameObject.SetActive(false);
        nextButton.SetActive(false);
        stopNotifier.SetActive(false);
    }

    public void NextButtonSetActive(bool active)
    {
        nextButton.SetActive(active);
    }

    public void NextButtonDown()
    {
        for (int i = 0; i < GameManager.Instance.names.Length; ++i)
        {
            string name = GameManager.Instance.names[i];

            PlayerPrefs.DeleteKey("Rank" + i);
            PlayerPrefs.DeleteKey("Time" + i);

            PlayerPrefs.SetString("Rank" + i, name);

            if (name != "Incomplete")
                if (name == "Player")
                    PlayerPrefs.SetString("Time" + i, FloatToTimeString(CKB_Player.Instance.GetComponent<CKB_RiderRank>().totalLapTime));
                else
                    PlayerPrefs.SetString("Time" + i, GameObject.Find("LHY/AIs/" + name).GetComponent<FinishCount>().lapTime);
        }
        SceneManager.LoadScene("AwardScene");
    }

    bool isBackward;

    public void StopNotifierController()
    {
        CKB_RiderRank ckbRdRank = CKB_Player.Instance.GetComponent<CKB_RiderRank>();
        Transform checkPoint = checkPoints.GetChild(ckbRdRank.lastTouched - (ckbRdRank.currentLap-1) * checkPoints.childCount);
    
        Vector3 forward = CKB_Player.Instance.transform.forward;
        Vector3 chkPtForward = checkPoint.forward;

        float dot = Vector3.Dot(forward, chkPtForward);

        if (dot < 0)
        {
            if (!isBackward)
                CKB_AudioManager.Instance.PlayAudio(CKB_AudioManager.AudioType.Backward);

            stopNotifier.SetActive(true);
            isBackward = true;
        }
        else
        {
            stopNotifier.SetActive(false);
            isBackward = false;
        }
    }

    string FloatToTimeString(float time)
    {
        return TimeSpan.FromSeconds(time).ToString("mm':'ss':'ff");
    }

    /*public void UpdateRankUI(GameObject[] sortedRiders)
    {
        int length = sortedRiders.Length;
        int idxPlayer = Array.IndexOf(sortedRiders, CKB_Player.Instance.gameObject);

        for (int i = 0; i < length; ++i)
        {
            rankName.GetChild(i).GetComponent<TMP_Text>().text = sortedRiders[i].name;

            rank.GetChild(i).gameObject.SetActive(i==idxPlayer ? true : false);
        }
    }*/
}
