using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : NetworkBehaviour
{
    public int minigamesPlayed;

    public FieldData fieldToPlayAbout;
    public FieldData attackingPlayer;

    public List<GameObject> gameFieldFolder;
    public GameObject miniGame;

    public GameObject minigameRunning;

    //Starts a minigame
    public void StartMiniGame()
    {
        minigamesPlayed++;
        foreach (GameObject gameFieldObject in gameFieldFolder)
        {
            gameFieldObject.transform.position = new Vector3(0, 300, 0);
        }

        Debug.Log("Start");

        foreach (PlayerInteractions player in FindObjectsOfType<PlayerInteractions>())
        {
            player.enabled = true;

            player.transform.Find("Player").gameObject.SetActive(true);
        }

        //FindObjectOfType<RPSUiManager>().SetUp();
    }

    public void OpenMiniGameame()
    {
        minigameRunning = Instantiate(miniGame);
        NetworkServer.Spawn(minigameRunning);
    }
}
