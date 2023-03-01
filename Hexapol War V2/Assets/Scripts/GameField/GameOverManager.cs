using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : NetworkBehaviour
{
    public GameObject gameOverSceen;

    public TMP_Text lostOrWon;
    public TMP_Text timeText;
    public TMP_Text fieldsCaptured;
    public TMP_Text gamesPlayed;

    bool gameOver = false;

    public void GameOver(int time)
    {
        if(!gameOver)
        {
            gameOverSceen.SetActive(true);

            SetLostOrWon();
            SetTime(time);
            SetFieldsCaptured();
            SetMinigamesPlayed();
        }

        gameOver = true;
    }


    void SetLostOrWon()
    {
        //is player one and won
        if (isServer && FindObjectOfType<FieldManager>().remainingFieldsPlayer1.Count == 0)
        {
            lostOrWon.text = "Lost";
            PlayerPrefs.SetInt("GamesLost", PlayerPrefs.GetInt("GamesLost") + 1);
        }
        else
        {
            lostOrWon.text = "Won";
        }

        //is player two and won
        if (isClientOnly && FindObjectOfType<FieldManager>().remainingFieldsPlayer2.Count == 0)
        {
            lostOrWon.text = "Lost";
            PlayerPrefs.SetInt("GamesLost", PlayerPrefs.GetInt("GamesLost") + 1);
        }
        else
        {
            lostOrWon.text = "Won";
            PlayerPrefs.SetInt("GamesWon", PlayerPrefs.GetInt("GamesWon") + 1);
        }
    }

    void SetTime(int time)
    {
        timeText.text = "Time Played: " + time.ToString();
        PlayerPrefs.SetInt("TimeSpentInGames", PlayerPrefs.GetInt("TimeSpentInGames") + time);
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
