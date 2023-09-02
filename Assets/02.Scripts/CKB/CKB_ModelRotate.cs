using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKB_ModelRotate : MonoBehaviour
{
    // 좌우 핸들 시 플레이어 모델이 회전하는 최대 각도
    public float maxRotateAngle;
    // 드리프트 시 플레이어 모델이 회전하는 최대 각도
    public float driftMaxRotateAngle;

    CKB_PlayerDrive ckbPD;
    float o_maxRotateAngle;

    // Start is called before the first frame update
    void Start()
    {
        o_maxRotateAngle = maxRotateAngle;

        ckbPD = GetComponentInParent<CKB_PlayerDrive>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((CKB_Player.Instance.state & (CKB_Player.State.Ready | CKB_Player.State.End)) != 0)
            return;

        Vector3 angle = transform.localEulerAngles;
        angle.y = Input.GetAxis("Horizontal") * maxRotateAngle * (ckbPD.isBrake ? -1 : 1);
        transform.localEulerAngles = angle;

        if (CKB_Player.Instance.state == CKB_Player.State.Drift)
            maxRotateAngle = driftMaxRotateAngle;
        else
            maxRotateAngle = o_maxRotateAngle;
    }
}
