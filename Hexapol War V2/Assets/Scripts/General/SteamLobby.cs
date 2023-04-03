using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using Steamworks;

public class SteamLobby : MonoBehaviour
{
    //Calbacks
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> joinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    //Variables
    public ulong currentLobbyID;
    private const string hostAddressKey = "HostAddress";
    public CustomNetworkManager manager;

    //GameObjects
    public GameObject hostButton;
    public TMP_Text lobbyNameText;

    private void Start()
    {
        if(!SteamManager.Initialized) { return; }

        manager = GetComponent<CustomNetworkManager>();

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        joinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEnterd);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) { return; }

        Debug.Log("Lobby created Succesfully");

        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request To Join Lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEnterd(LobbyEnter_t callback)
    {
        //Everyone
        currentLobbyID = callback.m_ulSteamIDLobby;

        //Clients
        if(NetworkServer.active) { return; }

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey);

        FindObjectOfType<MainMenu>().StartGame();

        manager.StartClient();
    }
}
