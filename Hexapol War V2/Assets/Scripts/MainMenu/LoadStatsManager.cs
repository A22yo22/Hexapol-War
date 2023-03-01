using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadStatsManager : MonoBehaviour
{
    public TMP_Text gamesPlayed;
    public TMP_Text timeSpentInGames;
    public TMP_Text fieldsCaptured;
    public TMP_Text enemyFieldsCaptured;
    public TMP_Text gamesWon;
    public TMP_Text gamesLost;

    public void LoadStats()
    {
        gamesPlayed.text = PlayerPrefs.GetInt("GamesPlayed").ToString();
        timeSpentInGames.text = PlayerPrefs.GetInt("TimeSpentInGames").ToString();
        fieldsCaptured.text = PlayerPrefs.GetInt("FieldsCaptured").ToString();
        enemyFieldsCaptured.text = PlayerPrefs.GetInt("EnemyFieldsCaptured").ToString();
        gamesWon.text = PlayerPrefs.GetInt("GamesWon").ToString();
        gamesLost.text = PlayerPrefs.GetInt("GamesLost").ToString();
    }

    //Resets stats when you want to
    public void ResetStats()
    {
        PlayerPrefs.SetInt("GamesPlayed", 0);
        PlayerPrefs.SetInt("TimeSpentInGames", 0);
        PlayerPrefs.SetInt("FieldsCaptured", 0);
        PlayerPrefs.SetInt("EnemyFieldsCaptured", 0);
        PlayerPrefs.SetInt("GamesWon", 0);
        PlayerPrefs.SetInt("GamesLost", 0);

        LoadStats();
    }
}
