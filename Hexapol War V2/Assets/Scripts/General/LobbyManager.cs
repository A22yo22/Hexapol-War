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

    public List<GameObject> fieldssSpawned = new List<GameObject>();

    public void ReadyUp()
    {
        playerReady++;
        if (playerReady == 1)
        {
            FieldSpawner.instance.SpawnGrid();
        }
        if (playerReady == 2)
        {
            if (PlayerPrefs.GetInt("fieldsSpawned") != 0) FindObjectOfType<SaveMap>().LoadGameMap();
        }
    }

    public void OnUpdatePlayerReady(int oldValue, int newValue)
    {
        playerReadyText.text = "Players Ready: " + newValue;

        if (newValue == 2)
        {
            UiManager.instance.lobbyUi.SetActive(false);
            //UiManager.instance.mainCam.SetActive(tr);
        }
    }
}
