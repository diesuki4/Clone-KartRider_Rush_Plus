using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayScene : MonoBehaviour
{
    public GameObject CanI;
    public GameObject CanB;
    public GameObject AIs;
    public GameObject Player;
    public GameObject PlayeS;
    public GameObject Button;


    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void startPlay()
    {
        StartCoroutine("GmaeStart");      
    }

    IEnumerator GmaeStart()
    {
        yield return new WaitForSeconds(1.0f);
        CanI.SetActive(false);
        CanB.SetActive(false);
        AIs.SetActive(true);
        Player.SetActive(true);
        PlayeS.SetActive(true);
        yield return new WaitForSeconds(5.0f);
        SceneManager.LoadScene("PlayScene");
    }
}
