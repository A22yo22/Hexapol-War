using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public SteamLobby lobbyManager;
    public NetworkManager manager;

    public TMP_InputField ip;

    public GameObject mainMenu;
    public GameObject lobby;

    public void Host()
    {
        lobbyManager.HostLobby();

        StartGame();
    }


    public void HostOffline()
    {
        manager.StartHost();

        StartGame();
    }

    public void Join()
    {
        //manager.networkAddress = ip.text;
        //manager.StartClient();

        StartGame();
    }
    public void JoinOffline()
    {
        manager.networkAddress = ip.text;
        manager.StartClient();

        StartGame();
    }

    public void StartGame()
    {
        mainMenu.SetActive(false);
        lobby.SetActive(true);
    }
}
