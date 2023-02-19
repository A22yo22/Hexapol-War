using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : NetworkBehaviour
{
    public FieldData fieldToPlayAbout;
    public FieldData.CaptureState attackingPlayer;

    public List<GameObject> gameFieldFolder;
    public GameObject miniGame;

    GameObject minigameRunning;

    //Starts a minigame
    public void StartMiniGame()
    {
        foreach (GameObject gameFieldObject in gameFieldFolder)
        {
            gameFieldObject.SetActive(false);
        }

        minigameRunning = Instantiate(miniGame);
    }
}
