using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverSceen;

    public TMP_Text timeText;

    public void GameOver(int time)
    {
        gameOverSceen.SetActive(true);

        SetTime(time);
    }


    void SetTime(int time)
    {
        timeText.text = "Time Played: " + time.ToString();
    }
}
