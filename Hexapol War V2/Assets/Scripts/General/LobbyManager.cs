using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyManager : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnUpdatePlayerReady))]
    public int playerReady = 0;

    public TMP_Text playerReadyText;
    public GameObject lobbyUi;
    public GameObject mainCam;

    public List<GameObject> hexagonsSpawned = new List<GameObject>();

    public void ReadyUp()
    {
        playerReady++;
        if (playerReady == 2)
        {
            FindObjectOfType<FieldSpawner>().SpawnGrid();
        }
    }

    public void OnUpdatePlayerReady(int oldValue, int newValue)
    {
        playerReadyText.text = "Players Ready: " + newValue;

        if (newValue == 2)
        {
            UiManager.instance.lobbyUi.SetActive(false);
            UiManager.instance.mainCam.SetActive(false);
        }
    }
}
