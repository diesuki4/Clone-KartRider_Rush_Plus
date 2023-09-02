using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKB_RankManager : MonoBehaviour
{
    public GameObject[] riders;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] sortedRiders = riders.OrderByDescending(x => x.GetComponent<CKB_RiderRank>().lastTouched).ToArray();
    
        CKB_UIManager.Instance.UpdateRankUI(sortedRiders);
    }
}
