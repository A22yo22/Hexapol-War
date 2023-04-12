using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : NetworkBehaviour
{
    public static MiniGameManager instance;

    public int minigamesPlayed;

    public FieldData fieldToPlayAbout;
    public List<FieldData> attackingPlayers;

    public List<GameObject> gameFieldFolder;
    public GameObject miniGame;

    public GameObject minigameRunning;

    private void Start()
    {
        if (instance == null) { instance = this; }
    }

    //Starts a minigame
    public void StartMiniGame()
    {
        minigamesPlayed++;
        foreach (GameObject gameFieldObject in gameFieldFolder)
        {
            gameFieldObject.transform.position = new Vector3(0, 300, 0);
        }

        foreach (PlayerInteractions player in FindObjectsOfType<PlayerInteractions>())
        {
            player.enabled = false;

            player.transform.Find("Player").gameObject.SetActive(true);

            BoxCollider spawnField = miniGame.transform.Find("SpawnField").GetComponent<BoxCollider>();
            Vector3 spawnPosition = transform.position + new Vector3( Random.Range(-spawnField.size.x / 2f, spawnField.size.x / 2f), 1.5f, Random.Range(-spawnField.size.z / 2f, spawnField.size.z / 2f));
            player.transform.Find("Player").position = spawnPosition;

            if (player.transform.Find("CanMove") != null)
            {
                player.transform.Find("CanMove").gameObject.SetActive(false);
            }

            //Disables all minigame objects
            player.GetComponent<PlayerMovement>().enabled = true;
            player.GetComponent<TankTurret>().enabled = true;

            //player.GetComponent<Health>().ResetMinigameHealth(1);
        }
    }

    public void OpenMiniGameame()
    {
        minigameRunning = Instantiate(miniGame);
        NetworkServer.Spawn(minigameRunning);
    }

    public void GameOver()
    {
        //Enable game map
        foreach (GameObject gameFieldObject in gameFieldFolder)
        {
            gameFieldObject.transform.position = new Vector3(0, 0, 0);
        }

        foreach (PlayerInteractions player in FindObjectsOfType<PlayerInteractions>())
        {
            player.enabled = true;

            player.transform.Find("Player").gameObject.SetActive(false);

            player.GetComponent<PlayerMovement>().enabled = false;
            player.GetComponent<TankTurret>().enabled = false;
        }

        Destroy(minigameRunning);

        //Refreshing game over manager
        foreach (PlayerInteractions player in FindObjectsOfType<PlayerInteractions>())
        {
            if (player.thisPlayerTag != FieldData.CaptureState.Clear)
            {
                player.GetComponent<PlayerStats>().RefreshRemainingFields();
            }

            //Clears all sellected fields of the player
            player.lastSelectedFields.Clear();
        }


        //Set camera
        Camera.main.transform.position = new Vector3(0, 9.3f, -8.59f);
        Camera.main.transform.rotation = Quaternion.Euler(51.74f, 0f, 0f);
    }
}
