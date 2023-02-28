using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverSceen;

    public TMP_Text timeText;
    public TMP_Text fieldsCaptured;
    public TMP_Text gamesPlayed;

    bool gameOver = false;

    public void GameOver(int time)
    {
        if(!gameOver)
        {
            gameOverSceen.SetActive(true);

            SetTime(time);
            SetFieldsCaptured();
            SetMinigamesPlayed();
        }

        gameOver = true;
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
                PlayerPrefs.SetInt("FieldsCaptured", PlayerPrefs.GetInt("FieldsCaptured") + player.GetComponent<PlayerStats>().fieldsCaptured);
                fieldsCaptured.text = "Fields Captured: " + player.GetComponent<PlayerStats>().fieldsCaptured.ToString();
            }
        }
    }

    void SetMinigamesPlayed()
    {
        PlayerPrefs.SetInt("GamesPlayed", PlayerPrefs.GetInt("GamesPlayed") + FindObjectOfType<MinigameManager>().minigamesPlayed);
        gamesPlayed.text = "Minigames Played: " + FindObjectOfType<MinigameManager>().minigamesPlayed.ToString();
    }

    //Back
    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
