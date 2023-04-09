using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    public SteamLobby lobbyManager;
    public NetworkManager manager;

    public TMP_InputField ip;

    public GameObject mainMenu;
    public GameObject lobby;

    private void Awake()
    {
        if (instance == null) { instance = this; }
    }

    public void Host()
    {
        lobbyManager.HostLobby();

        StartGame();
    }


    public void HostOffline()
    {
        manager.StartHost();

        //StartGame(); 
    }

    public void Join()
    {
        //manager.networkAddress = ip.text;
        //manager.StartClient();

        StartGame();
    }
    public void JoinOffline()
    {
        manager.networkAddress = "localhost";
        manager.StartClient();

        StartGame();
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
    }
}
