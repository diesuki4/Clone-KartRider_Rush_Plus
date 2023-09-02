using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMove : MonoBehaviour
{
    public GameObject player; //플레이어를 왁인하기 위해

    public ParticleSystem speedline;
    public ParticleSystem speedline2;

    public static AIMove Instance;

    public GameObject thisai;

    public float stopCountDown;

    //AI�� ������ ���� ��������
    //Vector3 dir= Vector3.forward;

    //AI�� �����̴� �⺻ �ӵ� ��������
    public float carspeed = 80;
    //Ÿ���� ��ġ
    public Transform target;
    //���° Ÿ������ ����ϴ� ����
    int nextTarget;

    public float currentTime = 0;
    public float boostTime = 3;

    public float randomTime = 5;
    public float currRandom = 0;

    public bool isRandom = false;

    public int thisaic = 0;

    //레이 변수선언
    Ray ray;
    //레이 범위를 지정하는 변수
    public float raydistance = 10.0f;
    // 레이의 충돌감지
    public RaycastHit rayHit;
    // 레이가 충돌을 감지할 레이어 설정
    public LayerMask worldLayer;

    public bool isboost = false;

    public enum State
    {
        Ready,
        Idle,
        Drive,
        Boost,
        Stop,
    }

    public State state;
    // Start is called before the first frame update

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
       
        anim.SetBool("Boost", false);
        //state = State.Ready;      
        //anim.SetBool("Start", true);

        StartCoroutine("AI_Move");
        StartCoroutine("AI_Animation");
    }

    private void Update()
    {
        thisaic = thisai.GetComponent<FinishCount>().Instance.lapCount;
        //StartCoroutine("AI_Move");
        BposterOn();



        currRandom += Time.deltaTime;
        if (currRandom > randomTime)
        {
            isRandom = true;
            RandomPlay();
            currRandom = 0;
        }
        thisai.GetComponent<NavMeshAgent>().speed = carspeed;
        if (carspeed >= 90)
        {
            currentTime += Time.deltaTime;
            if (currentTime > boostTime)
            {
                carspeed = 80;
                isboost = false;
                anim.SetBool("Boost", false);
                speedline.Stop();
                currentTime = 0;
            }
        }
        else if (carspeed <= 80)
        {
            speedline.Stop();
            speedline2.Stop();
        }
      
        if (GameManager.Instance.finishCount >= 17)
        {
            stopCountDown += Time.deltaTime;
            if (thisaic >= 3)
            {
                anim.SetBool("Win", true);
                thisai.GetComponent<NavMeshAgent>().enabled = false;
                thisai.GetComponent<AIMove>().StopCoroutine("AI_Move");
            }
            if (stopCountDown >= 10)
            {
                if (thisaic < 3)
                {
                    anim.SetBool("Lose", true);
                    thisai.GetComponent<NavMeshAgent>().enabled = false;
                    thisai.GetComponent<AIMove>().StopCoroutine("AI_Move");
                }

            }

        }

    }
    public void StartAI()
    {

        player = GameObject.Find("Player");
        ray = new Ray();// ���̻���
        ray.origin = this.transform.position;
        ray.direction = this.transform.forward;

        //dir = Vector3.forward;
        target = GameManager.Instance.target1[nextTarget];   //���Ӹ޴����� ����ִ� Ÿ���� �迭�� ������ �ͼ� ����Ÿ���� Ÿ�ٿ� �����Ѵ�

        thisai.GetComponent<NavMeshAgent>().speed = carspeed;      //�׺�޽�������Ʈ�� ���ǵ� ���� �� ��ũ��Ʈ�� carspeed���� �����Ų��.
        thisai.GetComponent<NavMeshAgent>().                       //�׺�޽� ������Ʈ�� �������� Ÿ���� ���������� �Ѵ�.
              SetDestination(target.position);
        //StartCoroutine("AI_Animation");

        //StartCoroutine("AI_Move");                          //AI_Move �ڷ�ƾ�� �����Ѵ�.
        //AI_Animation �ڷ�ƾ�� �����Ѵ�.
    }

    IEnumerator AI_Move()
    {

        yield return new WaitForSeconds(3.0f);
        thisai.GetComponent<NavMeshAgent>().                       //�׺�޽� ������Ʈ�� �������� Ÿ���� ���������� �Ѵ�.
           SetDestination(target.position);
        while (true)
        {
            float dis = (target.position - transform.position).magnitude;

            if (dis <= 30)
            {

                nextTarget += 1;
                if (nextTarget >= GameManager.Instance.target1.Length)
                    nextTarget = 0;


                target = GameManager.Instance.target1[nextTarget];
                // NavMeshAgent가 활성화 상태일 때만 목적지 지정
                if (thisai.GetComponent<NavMeshAgent>().enabled)
                    thisai.GetComponent<NavMeshAgent>().
                       SetDestination(target.position);
            }
            //RayHit();

            yield return null;
        }
    }

    public Animator anim;
    IEnumerator AI_Animation()
    {
        Vector3 lastPosition;

        while (true)
        {

            lastPosition = transform.position;
            yield return new WaitForSecondsRealtime(0.04f);

            if ((lastPosition - transform.position).magnitude > 0)
            {
                Vector3 dir = transform.InverseTransformPoint(lastPosition);

                if (dir.x >= -0.01f && dir.x <= 0.01f)
                {
                    anim.SetBool("Boost", false);
                    anim.SetBool("Ltrun", false);
                    anim.SetBool("Rturn", false);

                }
                if (dir.x < -0.01f)
                {
                    anim.SetBool("Ltrun", true);
                }
                if (dir.x > 0.01f)
                {
                    anim.SetBool("Rturn", true);
                }

            }
            if ((lastPosition - transform.position).magnitude <= 0)
            {
                anim.SetBool("Boost", false);
                anim.SetBool("Ltrun", false);
                anim.SetBool("Rturn", false);
            }


            //if ((lastPosition - transform.position).magnitude <= 30)
            //GetComponent<Animator>().Play("Boost");


            yield return null;
        }
    }

    public void RandomPlay()
    {
        thisaic = thisai.GetComponent<FinishCount>().Instance.lapCount;
        if (thisaic >= 3 || thisaic < 3 && stopCountDown >= 10)
            return;

        if (isRandom == true || thisaic < 3)
        {
            thisai.GetComponent<NavMeshAgent>().speed = carspeed;
            //GetComponent<NavMeshAgent>().SetDestination(target.position);
            //Vector3 dir = Vector3.up;


            int random = Random.Range(0, 4);
            switch (random)
            {
                case 0:
                    carspeed = 80;
                    break;
                case 1:
                    carspeed = 90;
                    anim.SetBool("Boost", true);
                    speedline.Play();
                    //transform.Rotate(Vector3.up * 70 * Time.deltaTime);
                    break;
                case 2:
                    carspeed = 100;
                    //transform.Rotate(Vector3.up * 100 * Time.deltaTime);
                    anim.SetBool("Boost", true);
                    speedline2.Play();
                    break;
                case 3:
                    carspeed = 80;
                    //transform.Rotate(Vector3.down * 70 * Time.deltaTime);
                    break;
            }
            isRandom = false;

        }

    }

    public void BposterOn()
    {
        if (isboost == true)
        {
            carspeed = 100;
            anim.SetBool("Boost", true);
            speedline2.Play();
        }
    }

}
