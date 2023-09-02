using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKB_Billboard : MonoBehaviour
{
    public Transform targetCam;

    void Start()
    {
        
    }

    void Update()
    {
        transform.forward = targetCam.forward;
    }
}
