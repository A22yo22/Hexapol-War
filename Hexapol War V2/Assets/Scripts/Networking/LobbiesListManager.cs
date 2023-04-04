using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class LobbiesListManager : MonoBehaviour
{
    public static LobbiesListManager instance;

    //Lobbies List Variables
    public GameObject lobbyiesMenu;
    public GameObject lobbyDataItemPrefab;
    public GameObject lobbyListContent;

    public GameObject search;

    public List<GameObject> listOfLobbies = new List<GameObject>();

    private void Awake()
    {
        if(instance == null) { instance= this; }
    }

    public void GetListOfLobbies()
    {
        search.SetActive(false);

        lobbyiesMenu.SetActive(true);

        SteamLobby.instance.GetLobbiesList();
    }

    public void DisaplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
    {
        for(int i=0; i < lobbyIDs.Count; i++)
        {
            if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                GameObject createdItem = Instantiate(lobbyDataItemPrefab);

                createdItem.GetComponent<LobbyDataEntry>().lobbyID = (CSteamID)lobbyIDs[i].m_SteamID;
                createdItem.GetComponent<LobbyDataEntry>().lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID, "name");
                createdItem.GetComponent<LobbyDataEntry>().SetLobbyData();

                 createdItem.transform.SetParent(lobbyListContent.transform);
                 createdItem.transform.localScale = Vector3.one;

                listOfLobbies.Add(createdItem);
            }
        }
    }

    public void DestroyLobbies()
    {
        foreach(GameObject lobbyItem in listOfLobbies)
        {
            Destroy(lobbyItem);
        }
        listOfLobbies.Clear();
    }
}