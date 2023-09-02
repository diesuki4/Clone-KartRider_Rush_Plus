using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class CKB_AwardInitializer : MonoBehaviour
{
    public Transform[] riders;
    public Transform[] destinations;
    public Transform worldCanvas;
    public Transform screenCanvas;
    
    PlayableDirector pd;
    Dictionary<string, Sprite> riderProfiles;

    void Start()
    {
        string[] riderNames = {"Player", "AI_A", "AI_B", "AI_C", "AI_D", "AI_E", "AI_F", "AI_G"};
        riderProfiles = new Dictionary<string, Sprite>();

        foreach (string name in riderNames)
            riderProfiles.Add(name, Resources.Load<Sprite>("Riders/PP_" + name));

        string[] strWinners = new string[3];
        int winnerCount = 0;

        for (int i = 0; i < 3; ++i)
        {
            strWinners[i] = PlayerPrefs.GetString("Rank" + i);

            if (strWinners[i] != "Incomplete")
                ++winnerCount;
        }

        Transform[] winners = new Transform[winnerCount];

        for (int i = 0; i < winnerCount; ++i)
            winners[i] = riders.Where(x => x.name == strWinners[i]).First();

        for (int i = 0; i < winnerCount; ++i)
        {
            winners[i].position = destinations[i].position;
            winners[i].rotation = destinations[i].rotation;
        }

        for (int i = 0; i < riders.Length; ++i)
            worldCanvas.GetChild(i).position = riders[i].position + Vector3.up * 5;

        pd = GetComponent<PlayableDirector>();

        StartCoroutine(IEShowRankPanel());
    }

    void Update()
    {
        
    }

    IEnumerator IEShowRankPanel()
    {
        Transform rankPanel = screenCanvas.Find("Rank Panel");
        GameObject quitButton = screenCanvas.Find("Quit Button").gameObject;
        RectTransform[] ranks = new RectTransform[8];
        string[] lapTimes = new string[8];

        lapTimes = lapTimes.Select((x, i) => PlayerPrefs.GetString("Rank" + i) == "Incomplete" ? "Fail" : PlayerPrefs.GetString("Time" + i)).ToArray();

        List<string> lstNames = new List<string>();
        lstNames.Add("Player");
        lstNames.Add("AI_A");
        lstNames.Add("AI_B");
        lstNames.Add("AI_C");
        lstNames.Add("AI_D");
        lstNames.Add("AI_E");
        lstNames.Add("AI_F");
        lstNames.Add("AI_G");

        for (int i = 0; i < rankPanel.childCount; ++i)
        {
            ranks[i] = rankPanel.GetChild(i).GetComponent<RectTransform>();
            ranks[i].localEulerAngles = Vector3.right * 90;

            TMP_Text txtRank = ranks[i].Find("Rank").GetComponent<TMP_Text>();
            TMP_Text txtName = ranks[i].Find("Name").GetComponent<TMP_Text>();

            string name = PlayerPrefs.GetString("Rank" + i);

            if (name != "Incomplete")
            {
                txtRank.text = (i+1).ToString();
                txtName.text = name;
                lstNames.Remove(name);
            }
            else
            {
                txtRank.text = "X";
                txtName.text = lstNames.First();
                lstNames.Remove(txtName.text);
            }

            Image image = ranks[i].Find("Image").GetComponent<Image>();

            image.sprite = riderProfiles[txtName.text];
        }

        while (pd.time < 3.5f)
            yield return null;

        screenCanvas.gameObject.SetActive(true);

        for (int i = 0; i < rankPanel.childCount; ++i)
            yield return StartCoroutine(IERotateUI(3.6f + 0.1f * i, ranks[i]));

        while (pd.time < 6.5f)
            yield return null;

        for (int i = 0; i < rankPanel.childCount; ++i)
        {
            ranks[i].localEulerAngles = Vector3.right * 90;
            ranks[i].Find("Kart").GetComponent<TMP_Text>().text = lapTimes[i];
        }

        ranks[0].Find("Kart").GetComponent<TMP_Text>().color = new Color(255, 210, 0, 255) / 255f;

        for (int i = 0; i < rankPanel.childCount; ++i)
        {
            if (ranks[i].Find("Name").GetComponent<TMP_Text>().text == "Player")
                ranks[i].Find("New Record").gameObject.SetActive(PlayerPrefs.GetInt("Is Best Total Lap Time", 0) == 0 ? false : true);
            yield return StartCoroutine(IERotateUI(6.6f + 0.1f * i, ranks[i]));
        }

        while (pd.time < pd.duration)
            yield return null;

        quitButton.SetActive(true);
    }

    IEnumerator IERotateUI(float deadline, RectTransform rctTransform)
    {
        rctTransform.localEulerAngles = Vector3.right * 90;

        while (pd.time < deadline)
        {
            rctTransform.localEulerAngles = Vector3.Lerp(Vector3.right * 90, Vector3.zero, ((float)pd.time - (deadline - 0.1f)) / 0.1f);

            yield return null;
        }

        rctTransform.localEulerAngles = Vector3.zero;
    }

    public void QuitButtonDown()
    {
        GameObject endScreen = screenCanvas.Find("End Screen").gameObject;

        if (endScreen.activeSelf)
            Application.Quit();
        else
            endScreen.SetActive(true);
    }
}
