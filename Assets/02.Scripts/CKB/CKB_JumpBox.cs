using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CKB_JumpBox : MonoBehaviour
{
    public float jumpTime;
    public float dampCorrection;

    Transform destination;

    void Start()
    {
        destination = transform.Find("Destination");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.name.Contains("Player") || other.transform.name.Contains("AI_"))
            StartCoroutine(IEJumpBox(other.gameObject));
    }

    IEnumerator IEJumpBox(GameObject go)
    {
        CKB_Player player = (go == CKB_Player.Instance.gameObject ? CKB_Player.Instance : null);
        CKB_PlayerDrive ckbPD = null;
        CKB_Boost ckbBoost = null;

        NavMeshAgent agent = null;
        Vector3 o_destination = Vector3.zero;

        CharacterController cc = go.GetComponent<CharacterController>();
        Vector3 horizontalVelocity = Vector3.zero;
        Vector3 verticalVelocity = Vector3.zero;

        if (player)
        {
            ckbPD = player.GetComponent<CKB_PlayerDrive>();
            ckbBoost = player.GetComponent<CKB_Boost>();

            ckbPD.isDriftAvailable = false;
            CKB_Player.Instance.state = CKB_Player.State.JumpBox;    
                    
            CKB_AudioManager.Instance.PlayAudio(CKB_AudioManager.AudioType.ShortBooster);
        }
        else
        {
            agent = go.GetComponent<NavMeshAgent>();

            o_destination = agent.destination;
            agent.enabled = false;
        }

        float t = 0;

        while (t <= jumpTime)
        {
            t += Time.deltaTime;

            Vector3 horizontalVector = Vector3.SmoothDamp(go.transform.position, destination.position, ref horizontalVelocity, jumpTime * 0.5f) - go.transform.position;
            Vector3 verticalVector = Vector3.SmoothDamp(go.transform.position, destination.position,
                                                    ref verticalVelocity, jumpTime * dampCorrection) - go.transform.position;

            cc.Move(Vector3.Scale(horizontalVector, new Vector3(1, 0, 1)) + Vector3.Scale(verticalVector, Vector3.up));

            yield return null;
        }

        t = 0;
        horizontalVelocity = verticalVelocity = Vector3.zero;
        Vector3 destinationAI = Vector3.Scale(go.transform.position, new Vector3(1, 0, 1)) + go.transform.forward * 7;

        if (!player)
        {
            while (t <= jumpTime)
            {
                t += Time.deltaTime;

                Vector3 horizontalVector = Vector3.Lerp(go.transform.position, destinationAI, t / jumpTime) - go.transform.position;
                Vector3 verticalVector = Vector3.Lerp(go.transform.position, destinationAI, t / jumpTime) - go.transform.position;

                cc.Move(Vector3.Scale(horizontalVector, new Vector3(1, 0, 1)) + Vector3.Scale(verticalVector, Vector3.up));

                yield return null;
            }

            go.transform.position = destinationAI;
        }

        if (player)
        {
            ckbPD.isDriftAvailable = true;
            ckbBoost.isTouchBoostAvailable = true;
            ckbBoost.isNormalBoostAvailable = true;

            CKB_UIManager.Instance.DriftButtonUp();
        }
        else
        {
            agent.enabled = true;
            agent.ResetPath();
            agent.destination = o_destination;
        }
    }
    /*
        IEnumerator IEJumpBox(GameObject go)
        {
            CKB_Player player = (go == CKB_Player.Instance.gameObject ? CKB_Player.Instance : null);
            CKB_PlayerDrive ckbPD = null;
            CKB_Boost ckbBoost = null;

            NavMeshAgent agent = null;
            Vector3 o_destination = Vector3.zero;

            CharacterController cc = go.GetComponent<CharacterController>();
            Vector3 horizontalVelocity = Vector3.zero;
            Vector3 verticalUpVelocity = Vector3.zero;
            Vector3 verticalDownVelocity = Vector3.zero;

            if (player)
            {
                ckbPD = player.GetComponent<CKB_PlayerDrive>();
                ckbBoost = player.GetComponent<CKB_Boost>();

                ckbPD.isDriftAvailable = false;
                CKB_Player.Instance.state = CKB_Player.State.JumpBox;
            }
            else
            {
                agent = go.GetComponent<NavMeshAgent>();

                o_destination = agent.destination;
                agent.enabled = false;
            }

            float t = 0;

            while (t < jumpTime)
            {
                t += Time.deltaTime;

                Vector3 horizontalVector = Vector3.SmoothDamp(go.transform.position, destination.position, ref horizontalVelocity, jumpTime*0.5f) - go.transform.position;
                Vector3 verticalVector = Vector3.zero;

                if (t <= jumpTime*0.5f)
                    verticalVector = Vector3.SmoothDamp(go.transform.position, destination.position + Vector3.up * maxHeight, 
                                                        ref verticalUpVelocity, jumpTime*0.5f*dampCorrection) - go.transform.position;
                else
                    verticalVector = Vector3.SmoothDamp(go.transform.position, destination.position,
                                                        ref verticalDownVelocity, jumpTime*0.5f*dampCorrection) - go.transform.position;

                cc.Move(Vector3.Scale(horizontalVector, new Vector3(1, 0, 1)) + Vector3.Scale(verticalVector, Vector3.up));

                yield return null;
            }

            if (player)
            {
                ckbPD.isDriftAvailable = true;
                ckbBoost.isTouchBoostAvailable = true;
                ckbBoost.isNormalBoostAvailable = true;

                CKB_UIManager.Instance.DriftButtonUp();
            }
            else
            {
                agent.enabled = true;
                agent.ResetPath();
                agent.destination = o_destination;
            }
        }
    */
    /*
        IEnumerator IEJumpBox(GameObject go)
        {
            CKB_Player player = (go == CKB_Player.Instance.gameObject ? CKB_Player.Instance : null);
            CKB_PlayerDrive ckbPD = null;
            CKB_Boost ckbBoost = null;

            NavMeshAgent agent = null;

            CharacterController cc = go.GetComponent<CharacterController>();

            if (player)
            {
                ckbPD = player.GetComponent<CKB_PlayerDrive>();
                ckbBoost = player.GetComponent<CKB_Boost>();

                ckbPD.isDriftAvailable = false;
                CKB_Player.Instance.state = CKB_Player.State.JumpBox;
            }
            else
            {
                agent = go.GetComponent<NavMeshAgent>();
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }

            Vector3 dir = (destination.position - go.transform.position).normalized;
            float distanceXZ = Vector3.Distance(go.transform.position, destination.position);
            float deltaXZ = distanceXZ / jumpTime * Time.deltaTime;
            float deltaY = maxHeight / jumpTime / 2 * Time.deltaTime;

            float t = 0;

            while (t < jumpTime)
            {
                t += Time.deltaTime;

                Vector3 horizontalVector = dir * deltaXZ;
                Vector3 verticalVector = Vector3.zero;

                if (t <= jumpTime/2)
                    verticalVector = Vector3.up * deltaY;
                else
                    verticalVector = Vector3.down * deltaY;

                cc.Move(horizontalVector + verticalVector);

                yield return null;
            }

            if (player)
            {
                CKB_Player.Instance.state = ckbPD.o_tmpState;

                ckbPD.isDriftAvailable = true;
                ckbBoost.isTouchBoostAvailable = true;
                ckbBoost.isNormalBoostAvailable = true;
            }
            else
            {
                agent.isStopped = false;
            }
        }
    */
}
