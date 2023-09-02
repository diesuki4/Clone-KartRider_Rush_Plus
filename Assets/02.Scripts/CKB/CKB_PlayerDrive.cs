using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKB_PlayerDrive : MonoBehaviour
{
    // 이동 속력
    public float moveSpeed = 50f;
    // 최대 이동 속력
    public float maxSpeed = 15f;
    // 최대 속력 초깃값
    public float o_maxSpeed;
    // 좌우 핸들 속력
    public float handleSpeed = 0.2f;
    // 드리프트 민감도
    public float steerAngle = 20f;
    // 드리프트 시 deltaTime 당 속력이 줄어드는 배수
    public float drag;
    // 드리프트 종료 후 정방향 보정에 걸리는 시간
    public float driftLerpTime;
    // 드리프트 사용 가능 여부
    public bool isDriftAvailable;
    // 드리프트, 점프 박스 시 기존 상태를 저장하는 임시 변수
    public CKB_Player.State o_tmpState;
    // 현재 속력
    public float currentSpeed;
    // 브레이크 상태 여부
    public bool isBrake;
    // 적용될 중력의 배수
    public float gravityFactor;
    public float driftCount;
    public long sumSpeed;
    public long countSpeed;

    CharacterController cc;
    CKB_Boost ckbBoost;
    // 이동하는 방향 (후진일 경우 뒤로)
    Vector3 moveDirection;
    // 이동에 사용되는 벡터
    Vector3 moveForce;
    // driftLerpTime까지 시간을 누적하기 위한 변수
    float currentLerpTime;
    // 속력을 계산하기 위해 이전 위치를 저장하는 변수
    Vector3 prevPos;
    // 중력 가속도
    float gravity = 9.81f;
    // 자유 낙하 속도 누적 값
    float yVelocity;
    // "Horizontal" 값 저장
    float steerInput;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        ckbBoost = GetComponent<CKB_Boost>();

        o_maxSpeed = maxSpeed;
        o_tmpState = CKB_Player.State.Drive;

        isDriftAvailable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if ((CKB_Player.Instance.state & (CKB_Player.State.Ready | CKB_Player.State.End | CKB_Player.State.JumpBox)) != 0)
            return;

        moveDirection = transform.forward * (isBrake ? -1 : 1);

        moveForce += moveDirection * moveSpeed * ckbBoost.boostSpeedFactor * Time.deltaTime;

        DriftController();

        if (cc.isGrounded)
            yVelocity = 0;
        else
            yVelocity += gravity * gravityFactor * Time.deltaTime;

        cc.Move((moveForce + Vector3.down * yVelocity) * Time.deltaTime);

        moveForce = Vector3.Lerp(moveForce.normalized, moveDirection, Time.deltaTime) * moveForce.magnitude;

        if (CKB_Player.Instance.state != CKB_Player.State.Drift)
            transform.Rotate(Vector3.up * handleSpeed * steerInput * (isBrake ? -1 : 1));

        currentSpeed = Vector3.Distance(prevPos, transform.position) / Time.deltaTime;
        prevPos = transform.position;

        sumSpeed += (int)currentSpeed;
        ++countSpeed;

        mmtBoostEnabler();
    }

    void DriftController()
    {
        if (CKB_Player.Instance.state == CKB_Player.State.Drift || isBrake)
        {
            currentLerpTime = 0;
        }
        else
        {
            currentLerpTime += Time.deltaTime;

            if (driftLerpTime <= currentLerpTime)
                moveForce = moveDirection * moveForce.magnitude;
            else
                moveForce = Vector3.Lerp(moveForce.normalized, moveDirection, currentLerpTime / driftLerpTime) * moveForce.magnitude;
        }
        
        moveForce = Vector3.ClampMagnitude(moveForce, maxSpeed);

        steerInput = Input.GetAxisRaw("Horizontal");

        if (CKB_Player.Instance.state == CKB_Player.State.Drift)
        {
            transform.Rotate(Vector3.up * steerInput * (isBrake ? -1 : 1) * moveForce.magnitude * steerAngle * Time.deltaTime);
            moveForce *= drag;
        }
    }

    // 드리프트를 한 누적 시간
    float currentDriftTime;
    void mmtBoostEnabler()
    {
        if (CKB_Player.Instance.state == CKB_Player.State.Drift)
        {
            currentDriftTime += Time.deltaTime;
        }
        else
        {
            if (ckbBoost.mmtBoostRequireTime <= currentDriftTime)
                ckbBoost.EnableMMTBoost();

            currentDriftTime = 0;
        }
    }
}
