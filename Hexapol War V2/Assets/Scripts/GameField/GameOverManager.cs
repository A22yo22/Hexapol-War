using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverSceen;

    public TMP_Text timeText;
    public TMP_Text fieldsCaptured;
    public TMP_Text gamesPlayed;

    public void GameOver(int time)
    {
        gameOverSceen.SetActive(true);

        SetTime(time);
        SetFieldsCaptured();
        SetMinigamesPlayed();
    }


    void SetTime(int time)
    {
        timeText.text = "Time Played: " + time.ToString();
    }

    void SetFieldsCaptured()
    {
        foreach(PlayerInteractions player in FindObjectsOfType<PlayerInteractions>())
        {
            if (player.thisPlayerTag != FieldData.CaptureState.Clear)
            {
                fieldsCaptured.text = "Fields Captured: " + player.GetComponent<PlayerStats>().fieldsCaptured.ToString();
            }
        }
    }

    void SetMinigamesPlayed()
    {
        gamesPlayed.text = "Minigames Played: " + FindObjectOfType<MinigameManager>().minigamesPlayed.ToString();
    }
}
