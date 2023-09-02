using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKB_CornerZone : MonoBehaviour
{
    public float driftAccelerationFactor;

    CKB_PlayerDrive ckbPD;
    float o_steerAngle;

    void Start()
    {
        ckbPD = CKB_Player.Instance.GetComponent<CKB_PlayerDrive>();

        o_steerAngle = ckbPD.steerAngle;

        driftAccelerationFactor = transform.parent.GetComponentInChildren<CKB_CornerZone>().driftAccelerationFactor;
    }

    void OnTriggerEnter(Collider other)
    {
        ckbPD.steerAngle = o_steerAngle * driftAccelerationFactor;
    }

    void OnTriggerExit(Collider other)
    {
        ckbPD.steerAngle = o_steerAngle;
    }
}
