using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : NetworkBehaviour
{
    public FieldData fieldToPlayAbout;
    public FieldData.CaptureState attackingPlayer;

    public List<GameObject> gameFieldFolder;
    public List<GameObject> miniGameFolder;

    public void StartMiniGame()
    {
        foreach (GameObject gameFieldObject in gameFieldFolder)
        {
            gameFieldObject.SetActive(false);
        }
        foreach (GameObject miniGameObject in miniGameFolder)
        {
            miniGameObject.SetActive(true);
        }
    }
}
