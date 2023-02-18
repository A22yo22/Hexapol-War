using Mirror;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactions : NetworkBehaviour
{
    public FieldData.CaptureState thisPlayerTag;

    GameObject selectedTile;
    GameObject lastSelectedField;

    //References
    LobbyManager lobbyManager;

    void Start()
    {
        //Check if is local player
        if (!isLocalPlayer) return;

        //Asign playerTag
        if (isServer) thisPlayerTag = FieldData.CaptureState.Player1;
        else thisPlayerTag = FieldData.CaptureState.Player2;

        //Add event to ready up button
        UiManager.instance.button.onClick.AddListener(ReadyUp);

        //Get lobby manger
        lobbyManager = FindObjectOfType<LobbyManager>();
    }

    bool firstTileSpawned;
    private void Update()
    {
        //Check if is local player
        if (!isLocalPlayer) return;

        //Sets the spawn field of the current player and get spawnned hexagons
        if (FindObjectOfType<LobbyManager>().playerReady == 2 && firstTileSpawned)
        {
            //Sets all spawned fields
            FieldSpawner fieldSpawner = FindObjectOfType<FieldSpawner>();
            if (isServer)
            {
                for (int i = 0; i < fieldSpawner.hexagonsSpawned.Count; i++)
                {
                    CmdAddHexagons(fieldSpawner.hexagonsSpawned[i].GetComponent<NetworkIdentity>());
                }
            }

            if(fieldSpawner.hexagonsSpawned.Count == 0) return;

            //Set player start pos
            int selectedField = Random.Range(0, lobbyManager.hexagonsSpawned.Count);
            CmdSetFieldState(lobbyManager.hexagonsSpawned[selectedField].GetComponent<NetworkIdentity>(), thisPlayerTag);
            lastSelectedField = lobbyManager.hexagonsSpawned[selectedField];

            firstTileSpawned = true;
        }
        
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (selectedTile != null)
                {
                    lastSelectedField = selectedTile;
                }
                selectedTile = hit.transform.gameObject;

                FieldData.CaptureState clickedState = Checks.GetFieldState(hit.transform.gameObject);

                if (CanPlaceHere(clickedState))
                {
                    MadeMove();

                    selectedTile.transform.position = new Vector3(selectedTile.transform.position.x, 0.4f, selectedTile.transform.position.z);
                    Place(clickedState);
                }
                else if (clickedState != FieldData.CaptureState.Clear)
                {
                    Attack();
                }
            }
        }
    }


    //Events
    public void Attack()
    {

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

    //Set field state of selected field
    [Command]
    public void CmdSetFieldState(NetworkIdentity identity, FieldData.CaptureState state)
    {
        RpcSetFieldState(identity, state);
    }
    [ClientRpc]
    public void RpcSetFieldState(NetworkIdentity identity, FieldData.CaptureState state)
    {
        identity.GetComponent<FieldData>().SwitchCaptureState(state);
    }
}

//Checks for game field
public class Checks
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