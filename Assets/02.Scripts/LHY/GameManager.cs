using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    //���� �޴����� ������ �ٸ� ��ũ��Ʈ ������ ����� �� �ֵ��� ����ƽ���� �����.
    public static GameManager Instance;

    public Transform[] target1;
    public Transform[] target2;

    public float readyTime;

    //finishLine�� �������� ������Ʈ�� ����� �����ִ� ������ ������ �ʿ��ϴ�
    public int finishCount;

    public bool playerfinish;
    //public GameObject[] ais; 
    // public int[] ailaps;

    public string[] names;

    public int lap;
    public bool checkLine;

    public GameObject CanvasMap;

    [Header("CountDown")]
    public GameObject[] imageCount;
    public TextMeshProUGUI tenCount;
    public GameObject[] lapc;
    //SpriteRenderer count;

    [Header("Text")]
    public TextMeshProUGUI curTimeText;
    public TextMeshProUGUI[] lapTimeText;

    float curTime = 0;
    float curA = 0;
    float curB = 0;
    float curPlay = 0;

    public enum GameState
    {
        Start,
        Play,
        Stop,
    }
    public GameState state;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        names = new string[8];
        for (int i = 0; i < names.Length; ++i)
            names[i] = "Incomplete";

        //state = GameState.Start;
        checkLine = true;
        StartCoroutine("StartCount");
        StartCoroutine("StopCount");
    }

    private void Update()
    {
        print(finishCount);
        switch (state)
        {
            case GameState.Start:
                StartState();
                break;
            case GameState.Play:
                PlayState();
                break;
        }

        if(lap >= 2)
        {
            lapc[0].gameObject.SetActive(false);
            lapc[1].gameObject.SetActive(true);
        }
        else
        {
            lapc[1].gameObject.SetActive(false);
        }
        //StartCoroutine("StopPlay");
    }

    private void StartState()
    {

        curPlay += Time.deltaTime;
        if (curPlay > 5)
        {
            state = GameState.Play;
        }

    }

    private void PlayState()
    {
        StopCoroutine("StartCount");
        //���� finishCount�� ���� 17���� ũ�ų� ���ٸ�
        if (finishCount >= 17)
        {
            state = GameState.Stop;
        }
    }



    IEnumerator StartCount()
    {
        CKB_AudioManager.Instance.PlayAudio(CKB_AudioManager.AudioType.CountDown);

        imageCount[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        imageCount[1].gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        imageCount[2].gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        imageCount[0].GetComponent<Image>().color = new Color(0, 1, 0, 1);
        imageCount[1].GetComponent<Image>().color = new Color(0, 1, 0, 1);
        imageCount[2].GetComponent<Image>().color = new Color(0, 1, 0, 1);
        CanvasMap.gameObject.SetActive(true);

        CKB_AudioManager.Instance.StartAudio(CKB_AudioManager.AudioType.VillageRemix);

        StartCoroutine("Timer");
        yield return new WaitForSeconds(0.1f);
        imageCount[0].gameObject.SetActive(false);
        imageCount[1].gameObject.SetActive(false);
        imageCount[2].gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
    }

    public void LapTime()
    {
        lapTimeText[0].gameObject.SetActive(false);
        lapTimeText[0].text =
           string.Format("{0:00}:{1:00.00}",
           (int)(curTime / 60 % 60), curTime % 60);
        lapTimeText[0].gameObject.SetActive(true);

        for (int i = 0; i <= 3; i++)
        {
            if (lap >= 2)
            {
                lapTimeText[0].gameObject.SetActive(false);
                lapTimeText[1].gameObject.SetActive(false);
                lapTimeText[1].text =
                   string.Format("{0:00}:{1:00.00}",
                   (int)(curA / 60 % 60), curA % 60);
                //if()
                lapTimeText[1].gameObject.SetActive(true);
            }
            if (lap >= 3)
            {
                lapTimeText[1].gameObject.SetActive(false);
                lapTimeText[2].gameObject.SetActive(false);
                lapTimeText[2].text =
                   string.Format("{0:00}:{1:00.00}",
                   (int)(curB / 60 % 60), curB % 60);
                lapTimeText[2].gameObject.SetActive(true);
            }
        }

    }

    IEnumerator Timer()
    {
        while (true)
        {
            if (CKB_Player.Instance.state == CKB_Player.State.End)
            {
                CanvasMap.gameObject.SetActive(false);
                AIMove.Instance.state = AIMove.State.Stop;
                yield break;
            }
            curTime += Time.deltaTime;
            if (lap >= 2)
            {
                curA += Time.deltaTime;               
            }
            if (lap >= 3)
            {
                curB += Time.deltaTime;
            }
            curTimeText.text = string.Format("{0:00}:{1:00.00}", (int)(curTime / 60 % 60), curTime % 60);
            yield return null;
        }
    }
    /*
        IEnumerator StopPlay()
        {
            if (lap >= 4)
            {
                yield return new WaitForSeconds(1.0f);
                Time.timeScale = 0;
            }
        }
    */
    IEnumerator StopCount()
    {
        while (state != GameState.Stop)             //state�� stop���°� �ƴ� ���ȿ���
            yield return null;                      //�ƹ��͵� ������� �ʴ´�.
        CanvasMap.gameObject.SetActive(false);      //ȭ�鿡 ǥ�õǾ��� ���緦, ������,�̴ϸ��� �Ⱥ��̰� �Ѵ�.

        if (playerfinish == false)
        {
            tenCount.gameObject.SetActive(true);    //ī��Ʈ�ٿ� ���ӿ�����Ʈ�� ȭ�鿡 ǥ���ϰ�

            for (int i = 10; i >= 1; i--)                //i�� 10���� 1���� �����ϴ� ����
            {
                if (playerfinish == true)
                {
                    tenCount.gameObject.SetActive(false);
                }
                tenCount.text = i.ToString();           //10�� ī��Ʈ �ٿ� �ؽ�Ʈ�� ������ i���� �����Ѵ�.
                CKB_AudioManager.Instance.PlayAudio(CKB_AudioManager.AudioType.Timer);
                yield return new WaitForSeconds(1);     //1�� ��ٸ� ��
                
            }                                         //���� �÷��̾ ��¼��� ��� ���� ���ߴٸ�
        }
       
        tenCount.gameObject.SetActive(false);


        if (CKB_Player.Instance.state != CKB_Player.State.End)
            CKB_UIManager.Instance.GetComponent<CKB_FinishCeremony>().StartRaceFailedCut();
    }
}
