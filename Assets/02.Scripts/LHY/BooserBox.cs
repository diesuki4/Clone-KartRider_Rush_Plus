using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BooserBox : MonoBehaviour
{

    public GameObject box;
    private void OnTriggerEnter(Collider other)
    {
        AIMove booster = other.GetComponent<AIMove>();
        if (booster)
        {
            booster.isboost = true;
        }
        AIMove2 booster2 = other.GetComponent<AIMove2>();
        if (booster2)
        {
            booster2.isboost = true;
        }

        CKB_Boost playerboost = other.GetComponent<CKB_Boost>();
        if (playerboost)
        {
            playerboost.Boost(CKB_Player.State.BoostBox);
            CKB_AudioManager.Instance.PlayAudio(CKB_AudioManager.AudioType.NormalBooster);
        }

        box.gameObject.SetActive(false);
    }
    private void OnTriggerExit(Collider other)
    {
        box.gameObject.SetActive(true);
    }
}
