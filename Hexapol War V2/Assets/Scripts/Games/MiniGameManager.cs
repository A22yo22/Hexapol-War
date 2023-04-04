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
;

        foreach (PlayerInteractions player in FindObjectsOfType<PlayerInteractions>())
        {
            player.enabled = false;

            player.transform.Find("Player").gameObject.SetActive(true);

            if (player.transform.Find("CanMove") != null)
            {
                player.transform.Find("CanMove").gameObject.SetActive(false);
            }

            player.GetComponent<PlayerMovement>().enabled = true;
            player.GetComponent<TankTurret>().enabled = true;
            player.GetComponent<Health>().enabled = true;
            player.GetComponent<Health>().ResetMinigameHealth();
        }

        //FindObjectOfType<RPSUiManager>().SetUp();
    }

    public void OpenMiniGameame()
    {
        minigameRunning = Instantiate(miniGame);
        NetworkServer.Spawn(minigameRunning);
    }

    public void GameOver(NetworkIdentity loser)
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

        //Refreshing game over manager              BROKEN!!!
        foreach (PlayerInteractions player in FindObjectsOfType<PlayerInteractions>())
        {
            if (player.thisPlayerTag != FieldData.CaptureState.Clear)
            {
                player.GetComponent<PlayerStats>().RefreshRemainingFields();
            }
        }

        //
    }
}
