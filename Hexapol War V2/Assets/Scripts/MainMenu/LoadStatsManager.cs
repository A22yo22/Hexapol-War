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

    void Start()
    {
        LoadStats();
    }

    void SetStatsSave()
    {
        PlayerPrefs.SetInt("GamesPlayed", PlayerPrefs.GetInt("GamesPlayed"));
        PlayerPrefs.SetInt("TimeSpentInGames", PlayerPrefs.GetInt("TimeSpentInGames"));
        PlayerPrefs.SetInt("FieldsCaptured", PlayerPrefs.GetInt("FieldsCaptured"));
        PlayerPrefs.SetInt("EnemyFieldsCaptured", PlayerPrefs.GetInt("EnemyFieldsCaptured"));
        PlayerPrefs.SetInt("GamesWon", PlayerPrefs.GetInt("GamesWon"));
        PlayerPrefs.SetInt("GamesLost", PlayerPrefs.GetInt("GamesLost"));
    }

    public void LoadStats()
    {
        gamesPlayed.text = PlayerPrefs.GetInt("GamesPlayed").ToString();
        timeSpentInGames.text = PlayerPrefs.GetInt("TimeSpentInGames").ToString();
        fieldsCaptured.text = PlayerPrefs.GetInt("FieldsCaptured").ToString();
        enemyFieldsCaptured.text = PlayerPrefs.GetInt("EnemyFieldsCaptured").ToString();
        gamesWon.text = PlayerPrefs.GetInt("GamesWon").ToString();
        gamesLost.text = PlayerPrefs.GetInt("GamesLost").ToString();
    }
}
