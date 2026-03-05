using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RaceManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject finishPanel;

    public TextMeshProUGUI firstPlaceText;
    public TextMeshProUGUI secondPlaceText;
    public TextMeshProUGUI thirdPlaceText;
    public TextMeshProUGUI fourthPlaceText;

    private float raceTimer;
    private int finishOrder = 0;

    private Dictionary<RunController, bool> finishedPlayers =
        new Dictionary<RunController, bool>();

    void Start()
    {
        finishPanel.SetActive(false);
    }

    void Update()
    {
        raceTimer += Time.deltaTime;
    }

    public void PlayerFinished(RunController controller)
    {
        if (finishedPlayers.ContainsKey(controller))
            return;

        finishedPlayers.Add(controller, true);

        finishOrder++;

        controller.enabled = false;

        PlayerIdentity identity =
            controller.GetComponent<PlayerIdentity>();

        string playerName = identity != null
            ? identity.playerName
            : "Unknown";

        string formattedTime = FormatTime(raceTimer);

        AssignRanking(finishOrder, playerName, formattedTime);

        finishPanel.SetActive(true);
    }

    void AssignRanking(int place, string name, string time)
    {
        string resultLine = $"{name}   {time}";

        switch (place)
        {
            case 1:
                firstPlaceText.text = "🥇 " + resultLine;
                break;

            case 2:
                secondPlaceText.text = "🥈 " + resultLine;
                break;

            case 3:
                thirdPlaceText.text = "🥉 " + resultLine;
                break;

            case 4:
                fourthPlaceText.text = "🏅 " + resultLine;
                break;
        }
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);

        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }
}