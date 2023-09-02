using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKB_AudioManager : MonoBehaviour
{
    public static CKB_AudioManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            audios = GetComponents<AudioSource>();
        }
    }

    public enum AudioType
    {
        VillageRemix, // 배찌 뒹글뒹글 - 빌리지 리믹스
        CountDown, // 카운트다운
        Engine, // 엔진
        Drift, // 드리프트
        NormalBooster, // 일반 부스터
        ShortBooster, // 짧은 부스터
        FinishLinePass, // 결승선 통과
        RaceFailed, // 완주 실패
        Backward, // 뒤로갈 때
        LapChange, // 뒤로갈 때
        Timer // 뒤로갈 때
    }

    AudioSource[] audios;
    CKB_PlayerDrive ckbPD;
    CKB_Boost ckbBoost;
    float maxSpeed;

    void Start()
    {
        ckbPD = CKB_Player.Instance.GetComponent<CKB_PlayerDrive>();
        ckbBoost = CKB_Player.Instance.GetComponent<CKB_Boost>();

        maxSpeed = Mathf.Max(new float[]{ckbBoost.turboBoostMaxSpeed, ckbBoost.touchBoostMaxSpeed,
                            ckbBoost.mmtBoostMaxSpeed, ckbBoost.normalBoostMaxSpeed});
    }

    void Update()
    {
        EnginePitchController();
    }

    public void StartAudio(AudioType audioType)
    {
        audios[(int)audioType].Play();
    }

    public void PlayAudio(AudioType audioType)
    {
        StartAudio(audioType);
    }

    public void StopAudio(AudioType audioType)
    {
        audios[(int)audioType].Stop();
    }

    public void StopAllAudio()
    {
        foreach (AudioSource audio in audios)
            audio.Stop();
    }

    void EnginePitchController()
    {
        audios[(int)AudioType.Engine].pitch = ckbPD.currentSpeed / ckbPD.maxSpeed * 1.2f;
    }
}
