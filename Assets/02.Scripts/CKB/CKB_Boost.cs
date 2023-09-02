using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKB_Boost : MonoBehaviour
{
    // 일반 부스터 가동 시간, 최대 속력
    public float normalBoostTime, normalBoostMaxSpeed;
    // 순간 부스터 가동 시간, 최대 속력
    public float mmtBoostTime, mmtBoostMaxSpeed;
    // 터치 부스터 가동 시간, 최대 속력
    public float touchBoostTime, touchBoostMaxSpeed;
    // 터보 부스터 가동 시간, 최대 속력
    public float turboBoostTime, turboBoostMaxSpeed;
    // 부스터 박스 접촉 시 부스터 가동 시간, 최대 속력
    public float boostBoxTime, boostBoxMaxSpeed;
    // 부스터 사용 시 deltaTime 당 기존 대비 속력이 증가하는 배수
    public float boostSpeedFactor;
    // 부스터 사용 시 deltaTime 당 최대 속력이 증가하는 배수
    public float boostMaxSpeedFactor;
    // 터보 부스터 사용 가능 시간
    public float turboBoostAvailableTime;
    // 순간 부스터 사용을 위한 최소 드리프트 시간
    public float mmtBoostRequireTime;
    // 순간 부스터 활성화 시 사용 가능 시간
    public float mmtBoostAvailableTime;
    // 총 일반 부스터 게이지
    public float normalBoostTotalGauge;
    // 현재 일반 부스터 게이지
    public float normalBoostGauge;
    // 일반 부스터 게이지 증가량 (deltaTime 당)
    public float normalBoostGaugeIncreaseSpeed;
    // 터치 부스터 게이지
    public int touchBoostGauge;
    // UI 에 터치 부스터가 보이기 위한 최소 터치 횟수
    public int touchBoostGaugeUIMinCount;
    // 터치 부스터 발동까지 필요한 총 터치 횟수
    public int touchBoostTotalGauge;
    // 터치 부스터 게이지 누적 가능 시간
    public float touchBoostKeepAvailableTime;
    // 드리프트 시 일반 부스터 게이지 증가량 (deltaTime 당)
    public float driftGaugeIncreaseSpeed;
    // 부스터 종료 후 복구에 걸리는 시간
    public float recoverTime;
    // 순간 부스터 사용 가능 여부
    public bool isMMTBoostAvailable;
    // 터치 부스터 사용 가능 여부
    public bool isTouchBoostAvailable;
    // 일반 부스터 사용 가능 여부
    public bool isNormalBoostAvailable;
    public float boostCount;
    public GameObject postProcessVolume;

    CKB_PlayerDrive ckbPD;
    // 기존 boostSpeedFactor를 저장하는 변수
    float o_boostSpeedFactor;
    // 최대 부스터 충전 개수 (2개 고정)
    int normalBoostMaxCount = 2;
    // 현재 일반 부스터 충전 개수
    int normalBoostCount;
    // touchBoostKeepAvailableTime 동안 터치 없을 시
    // 터치 부스터 게이지 초기화를 위해 시간을 누적하는 변수 
    float touchBoostKeepCurrentTime;

    void Start()
    {
        ckbPD = GetComponent<CKB_PlayerDrive>();

        o_boostSpeedFactor = boostSpeedFactor;
        boostSpeedFactor = 1;

        normalBoostGauge = 0;

        isTouchBoostAvailable = true;
        isNormalBoostAvailable = true;
    }

    void Update()
    {
        NormalBoostGaugeController();
        TouchBoostController();
        BoostConcurrencyController();
    }

    public bool Boost(CKB_Player.State state)
    {
        float boostTime = 0, maxSpeed = 0;

        if ((state & (CKB_Player.State.NormalBoost | CKB_Player.State.MMTBoost |
            CKB_Player.State.TouchBoost | CKB_Player.State.TurboBoost |
            CKB_Player.State.BoostBox)) == 0)
            return false;
        else
            StopCoroutine("IEBoost");

        if (CKB_Player.Instance.state != CKB_Player.State.Drift)
            CKB_Player.Instance.state = state;

        ckbPD.o_tmpState = state;

        boostSpeedFactor = o_boostSpeedFactor;

        switch (state)
        {
        case CKB_Player.State.NormalBoost :
            boostTime = normalBoostTime;
            maxSpeed = normalBoostMaxSpeed;
            break;
        case CKB_Player.State.MMTBoost :
            boostTime = mmtBoostTime;
            maxSpeed = mmtBoostMaxSpeed;
            break;
        case CKB_Player.State.TouchBoost :
            boostTime = touchBoostTime;
            maxSpeed = touchBoostMaxSpeed;
            break;
        case CKB_Player.State.TurboBoost :
            boostTime = turboBoostTime;
            maxSpeed = turboBoostMaxSpeed;
            break;
        case CKB_Player.State.BoostBox :
            boostTime = boostBoxTime;
            maxSpeed = boostBoxMaxSpeed;
            break;
        }

        StartCoroutine("IEBoost", new object[]{boostTime, maxSpeed});

        return true;
    }

    IEnumerator IEBoost(object[] args)
    {
        float boostTime = (float)args[0];
        float maxSpeed = (float)args[1];

        postProcessVolume.SetActive(true);

        CKB_PlayerDrive ckbPD = GetComponent<CKB_PlayerDrive>();
        
        float t = 0;

        while (t < boostTime)
        {
            t += Time.deltaTime;

            ckbPD.maxSpeed = Mathf.Clamp(ckbPD.maxSpeed * boostMaxSpeedFactor, ckbPD.o_maxSpeed, maxSpeed);

            yield return null;
        }

        t = 0;

        while (t < recoverTime)
        {
            t += Time.deltaTime;

            ckbPD.maxSpeed = Mathf.Clamp(ckbPD.maxSpeed / boostMaxSpeedFactor, ckbPD.o_maxSpeed, maxSpeed);

            yield return null;
        }

        if ((CKB_Player.Instance.state & (CKB_Player.State.Drift | CKB_Player.State.JumpBox | CKB_Player.State.End)) == 0)
            CKB_Player.Instance.state = CKB_Player.State.Drive;

        ckbPD.o_tmpState = CKB_Player.State.Drive;

        ckbPD.maxSpeed = ckbPD.o_maxSpeed;
        boostSpeedFactor = 1;

        isTouchBoostAvailable = true;
        isNormalBoostAvailable = true;

        postProcessVolume.SetActive(false);
    }

    public void EnableMMTBoost()
    {
        isMMTBoostAvailable = true;

        CKB_UIManager.Instance.MMTBoostButtonSetActive(true);

        StartCoroutine(IEEnableMMTBoost());
    }

    IEnumerator IEEnableMMTBoost()
    {
        float t = 0;

        while (isMMTBoostAvailable & (t += Time.deltaTime) < mmtBoostAvailableTime)
            yield return null;

        isMMTBoostAvailable = false;
        CKB_UIManager.Instance.MMTBoostButtonSetActive(false);
    }

    public void MMTBoostExecute()
    {
        if (!isMMTBoostAvailable)
            return;

        Boost(CKB_Player.State.MMTBoost);
        CKB_AudioManager.Instance.PlayAudio(CKB_AudioManager.AudioType.ShortBooster);

        isMMTBoostAvailable = false;
    }

    void NormalBoostGaugeController()
    {
        if (CKB_Player.Instance.state == CKB_Player.State.Drive)
            normalBoostGauge += normalBoostGaugeIncreaseSpeed * Time.deltaTime;
        else if (CKB_Player.Instance.state == CKB_Player.State.Drift)
            normalBoostGauge += driftGaugeIncreaseSpeed * Time.deltaTime;

        if (normalBoostTotalGauge <= normalBoostGauge)
        {
            AddNormalBoost();

            normalBoostGauge = 0;
        }
    }

    void AddNormalBoost()
    {
        if (normalBoostMaxCount <= normalBoostCount)
            return;
        else if (normalBoostCount == 0)
            CKB_UIManager.Instance.NormalBoostButtonSetActive(1, true);
        else
            CKB_UIManager.Instance.NormalBoostButtonSetActive(2, true);
        
        ++normalBoostCount;
    }

    public void NormalBoostExecute()
    {
        if (normalBoostCount <= 0)
            return;
        else if (normalBoostCount < normalBoostMaxCount)
            CKB_UIManager.Instance.NormalBoostButtonSetActive(1, false);
        else
            CKB_UIManager.Instance.NormalBoostButtonSetActive(2, false);

        Boost(CKB_Player.State.NormalBoost);
        CKB_AudioManager.Instance.PlayAudio(CKB_AudioManager.AudioType.NormalBooster);

        --normalBoostCount;

        ++boostCount;
    }

    public void SetTouchBoostGauge(int gauge)
    {
        touchBoostGauge = gauge;
        touchBoostKeepCurrentTime = 0;
    }

    public void TouchBoostController()
    {
        if (0 < touchBoostGauge)
            touchBoostKeepCurrentTime += Time.deltaTime;

        if (!isTouchBoostAvailable || touchBoostKeepAvailableTime <= touchBoostKeepCurrentTime)
            SetTouchBoostGauge(0);

        if (touchBoostTotalGauge <= touchBoostGauge)
        {
            Boost(CKB_Player.State.TouchBoost);
            CKB_AudioManager.Instance.PlayAudio(CKB_AudioManager.AudioType.NormalBooster);
            
            SetTouchBoostGauge(0);
        }
    }

    void BoostConcurrencyController()
    {
        switch (CKB_Player.Instance.state)
        {
        case CKB_Player.State.NormalBoost :
            isMMTBoostAvailable = false;
            isTouchBoostAvailable = false;
            isNormalBoostAvailable = false;
            break;
        case CKB_Player.State.MMTBoost :
            isTouchBoostAvailable = false;
            break;
        case CKB_Player.State.JumpBox :
            isMMTBoostAvailable = false;
            isTouchBoostAvailable = false;
            isNormalBoostAvailable = false;
            break;
        }
    }
}
