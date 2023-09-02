using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO
public class CKB_CamZoomInOut : MonoBehaviour
{
    public float sensitivity;

    CKB_PlayerDrive ckbPD;
    CKB_Boost ckbBoost;
    Vector3 o_position;
    float maxSpeed;

    // Start is called before the first frame update
    void Start()
    {
        ckbPD = CKB_Player.Instance.GetComponent<CKB_PlayerDrive>();
        ckbBoost = CKB_Player.Instance.GetComponent<CKB_Boost>();

        maxSpeed = Mathf.Max(new float[]{ckbBoost.turboBoostMaxSpeed, ckbBoost.touchBoostMaxSpeed,
                                ckbBoost.mmtBoostMaxSpeed, ckbBoost.normalBoostMaxSpeed});
        o_position = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = 0;

            if ((CKB_Player.Instance.state & (CKB_Player.State.NormalBoost | CKB_Player.State.MMTBoost |
                CKB_Player.State.TouchBoost | CKB_Player.State.TurboBoost |
                CKB_Player.State.BoostBox | CKB_Player.State.JumpBox)) != 0 ||
                (ckbPD.o_tmpState & (CKB_Player.State.NormalBoost | CKB_Player.State.MMTBoost |
                CKB_Player.State.TouchBoost | CKB_Player.State.TurboBoost |
                CKB_Player.State.BoostBox)) != 0 && CKB_Player.Instance.state == CKB_Player.State.Drift)
                distance = -8;
        else
            distance = (maxSpeed * 0.5f - Mathf.Clamp(ckbPD.currentSpeed, 0, maxSpeed)) * sensitivity;

        Vector3 localForward = transform.localRotation * Vector3.forward;

        transform.localPosition = o_position + localForward * distance;
    }
}
