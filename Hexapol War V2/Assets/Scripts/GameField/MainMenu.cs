using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public NetworkManager manager;

    public TMP_InputField ip;

    public GameObject mainMenu;
    public GameObject lobby;

    public void Host()
    {
        manager.StartHost();

        StartGame();
    }

    public void Join()
    {
        manager.networkAddress = ip.text;
        manager.StartClient();

        StartGame();
    }

    void StartGame()
    {
        mainMenu.SetActive(false);
        lobby.SetActive(true);
    }
}
