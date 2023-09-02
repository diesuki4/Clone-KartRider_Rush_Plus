using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKB_RespawnZone : MonoBehaviour
{
    public Transform checkPoints;
    public float respawnHeight;

    public void Reset(GameObject go)
    {
        CKB_RiderRank ckbRdRank = go.GetComponent<CKB_RiderRank>();
        Transform checkPoint = checkPoints.GetChild(ckbRdRank.lastTouched - (ckbRdRank.currentLap - 1) * checkPoints.childCount);

        Vector3 moveVector = new Vector3(checkPoint.position.x, 0, checkPoint.position.z) - go.transform.position + Vector3.up * respawnHeight;

        go.GetComponent<CharacterController>().Move(moveVector);
        go.transform.forward = checkPoint.forward;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.name.Contains("Player") || other.transform.name.Contains("AI_"))
            Reset(other.gameObject);
    }

}
