using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKB_Player : MonoBehaviour
{
    public Animator anim;//�ִϸ��̼�

    public ParticleSystem speedline; //��ƼŬ �ν�Ʈ 1�ܰ�
    public ParticleSystem speedline2; //��ƼŬ �ν�Ʈ 2�ܰ�
    public ParticleSystem BoostL;
    public ParticleSystem BoostR;

    public static CKB_Player Instance;

    public TrailRenderer[] trails;

    public float stopCountDown;

    CharacterController cc;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
  
    }

    public enum State
    {
        Ready = 1,
        Drive = 2,
        Drift = 4,
        Collide = 8,
        NormalBoost = 16,
        MMTBoost = 32,
        TouchBoost = 64,
        TurboBoost = 128,
        BoostBox = 256,
        JumpBox = 512,
        End = 1024
    }
    public State state;

    // Start is called before the first frame update
    IEnumerator Start()
    {
       

        state = State.Ready;
        yield return new WaitForSeconds(GameManager.Instance.readyTime);
        state = State.Drive;
        cc = GetComponent<CharacterController>();

        CKB_AudioManager.Instance.StartAudio(CKB_AudioManager.AudioType.Engine);

        if (state == State.Drive || state == State.Drift || state == State.JumpBox)
        {
            anim.SetBool("Boost", false);
            StartCoroutine("AI_Animation");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.Drive || state == State.Drift || state == State.End)
        {
            anim.SetBool("Boost", false);
            speedline.Stop();
            speedline2.Stop();
        }
        if(state == State.MMTBoost || state == State.BoostBox || state == State.NormalBoost ||
            state == State.TouchBoost || state == State.TurboBoost || state == State.JumpBox ||
            ((GetComponent<CKB_PlayerDrive>().o_tmpState & (CKB_Player.State.NormalBoost | CKB_Player.State.MMTBoost |
            CKB_Player.State.TouchBoost | CKB_Player.State.TurboBoost | CKB_Player.State.BoostBox)) != 0 &&
            CKB_Player.Instance.state == CKB_Player.State.Drift))
        {
            anim.SetBool("Boost", true);
            if(state == State.MMTBoost || state == State.TouchBoost)
            {
                speedline.Play();              
            }
            else
            {
                speedline2.Play();
            }
        }
        if (state == State.Drift)
        {
            foreach(TrailRenderer T in trails)
            {
                T.emitting = true;
                BoostL.Play();
                BoostR.Play();
            }
        }
        else if(state != State.Drift)
        {
            foreach(TrailRenderer T in trails)
            {
                T.emitting = false;
                BoostL.Stop();
                BoostR.Stop();
            }
        }

        if (GameManager.Instance.finishCount >= 17)
        {
            stopCountDown += Time.deltaTime;
        
            if(stopCountDown >= 10)
            {
                if (GetComponent<CKB_RiderRank>().currentLap >= 3)
                {
                    anim.SetBool("Win", true);
                }
                if (GetComponent<CKB_RiderRank>().currentLap < 3)
                {
                    anim.SetBool("Lose", true);
                }
                else
                {
                    anim.SetBool("Win", true);
                }
            }
          
        }

    }

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
}
