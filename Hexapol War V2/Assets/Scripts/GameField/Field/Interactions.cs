using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactions : NetworkBehaviour
{
    public FieldData.CaptureState thisPlayerTag;


    void Start()
    {
        //Asign playerTag
        if (isServer) thisPlayerTag = FieldData.CaptureState.Player1;
        else thisPlayerTag = FieldData.CaptureState.Player2;

        //Lobby
        if (isLocalPlayer)
        {
            UiManager.instance.button.onClick.AddListener(ReadyUp);
        }
    }

    bool firstTileSpawned;
    private void Update()
    {
        //Check if is local player
        if (!isLocalPlayer)
        {
            return;
        }

        if (FindObjectOfType<LobbyManager>().playerReady == 2 && firstTileSpawned)
        {
            //Get all spawned fields
            FieldSpawner fieldSpawner = FindObjectOfType<FieldSpawner>();
            if (isServer)
            {
                for (int i = 0; i < fieldSpawner.hexagonsSpawned.Count; i++)
                {
                    CmdAddHexagons(fieldSpawner.hexagonsSpawned[i].GetComponent<NetworkIdentity>());
                }
            }
        }
    }



    //Network section

    //Get spawned fields
    [Command]
    public void CmdAddHexagons(NetworkIdentity id)
    {
        RpcAddHexagons(id);
    }
    [ClientRpc]
    public void RpcAddHexagons(NetworkIdentity id)
    {
        FindObjectOfType<LobbyManager>().hexagonsSpawned.Add(id.gameObject);
    }

    //Lobby managment
    public void ReadyUp()
    {
        CmdReadyUp();
    }
    [Command]
    public void CmdReadyUp()
    {
        FindObjectOfType<LobbyManager>().ReadyUp();
    }
}

//Checks for game field
class Checks
{
    //Checks if player can place on selected field
    public bool CanPlaceHere(FieldData.CaptureState state, FieldData.CaptureState playerTag)
    {
        if (state == playerTag || state == FieldData.CaptureState.Select) return true;

        return false;
    }

    //Gets the state of fields
    public FieldData.CaptureState GetFieldState(GameObject field)
    {
        FieldData fieldData = field.GetComponent<FieldData>();
        if(fieldData != null)
        {
            return fieldData.fieldState;
        }

        return FieldData.CaptureState.Clear;
    }
}