using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactions : NetworkBehaviour
{
    public FieldData.CaptureState thisPlayerTag;

    bool madeMove = false;

    public GameObject otherPlayer;

    GameObject selectedField;
    GameObject lastSelectedField;

    //Lists
    List<GameObject> selectedFields;


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
        if (FindObjectOfType<LobbyManager>().playerReady == 2 && !firstTileSpawned)
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

            if(lobbyManager.fieldssSpawned.Count == 0) return;

            //Set player start pos
            int selectedField = Random.Range(0, lobbyManager.fieldssSpawned.Count);
            CmdSetFieldState(lobbyManager.fieldssSpawned[selectedField].GetComponent<NetworkIdentity>(), thisPlayerTag);
            lastSelectedField = lobbyManager.fieldssSpawned[selectedField];

            //Get other player
            RpcSetOtherPlayeerVariable();
            //if (isServer) otherPlayer = NetworkServer.connections[1].identity.gameObject;
            //else if(isClient)  otherPlayer = NetworkServer.connections[0].identity.gameObject;

            firstTileSpawned = true;
        }
        
        if(Input.GetMouseButtonDown(0))
        {
            //Makes gets the field you've clicked on
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                //Sets last selected field to current selcted 
                if (selectedField != null) lastSelectedField = selectedField;
                selectedField = hit.transform.gameObject;

                //Gets the state of the selected field
                FieldData.CaptureState selectedFieldState = Checks.GetFieldState(hit.transform.gameObject);

                //Checks if player can move on this selected field
                if (Checks.CanMoveHere(selectedFieldState, thisPlayerTag))
                {
                    SwitchPlayerAtMove();

                    //Raise up
                    selectedField.transform.position = new Vector3(selectedField.transform.position.x, 0.4f, selectedField.transform.position.z);
                    Move(selectedFieldState);
                }
                else if (selectedFieldState != FieldData.CaptureState.Clear)    //Attack
                {
                    Attack();
                }
                else    //Clear
                {
                    //Reset used field possitions
                    selectedField.transform.position = new Vector3(selectedField.transform.position.x, 0f, selectedField.transform.position.z);
                    lastSelectedField.transform.position = new Vector3(lastSelectedField.transform.position.x, 0, lastSelectedField.transform.position.z);

                    //Reset selected fields
                    for (int i = 0; i < selectedFields.Count; i++)
                    {
                        selectedFields[i].GetComponent<FieldData>().SwitchCaptureState(FieldData.CaptureState.Clear);
                    }
                }
            }
        }
        else if(Input.GetMouseButtonDown(1))
        {
            //Reset used field possitions
            selectedField.transform.position = new Vector3(selectedField.transform.position.x, 0f, selectedField.transform.position.z);
            lastSelectedField.transform.position = new Vector3(lastSelectedField.transform.position.x, 0, lastSelectedField.transform.position.z);

            //Reset selected fields
            for (int i = 0; i < selectedFields.Count; i++)
            {
                selectedFields[i].GetComponent<FieldData>().SwitchCaptureState(FieldData.CaptureState.Clear);
            }
        }
    }

    //Move player to selected field
    public void Move(FieldData.CaptureState fieldState)
    {
        if (fieldState == FieldData.CaptureState.Select)
        {
            CmdSetFieldState(selectedField.GetComponent<NetworkIdentity>(), thisPlayerTag);
            SwitchPlayerAtMove();
        }
    }

    //Switches the player at move to the next one
    public void SwitchPlayerAtMove()
    {
        //Reset used field possitions
        selectedField.transform.position = new Vector3(selectedField.transform.position.x, 0f, selectedField.transform.position.z);
        lastSelectedField.transform.position = new Vector3(lastSelectedField.transform.position.x, 0, lastSelectedField.transform.position.z);

        //Reset selected fields
        for(int i = 0; i < selectedFields.Count; i++)
        {
            selectedFields[i].GetComponent<FieldData>().SwitchCaptureState(FieldData.CaptureState.Clear);
        }

        //Enable other player
        if (otherPlayer != null)
        {
            otherPlayer.SetActive(true);
            gameObject.SetActive(true);
        }

        //Switch player
        RpcSetPlayerAtMove(Checks.GetOppositeOfPlayerTag(thisPlayerTag));
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
        FindObjectOfType<LobbyManager>().fieldssSpawned.Add(id.gameObject);
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

    //Switch player at move 
    [ClientRpc]
    public void RpcSetPlayerAtMove(FieldData.CaptureState playerToWhoWillHaveTheMove)
    {
        FindObjectOfType<FieldManager>().playerAtMove = playerToWhoWillHaveTheMove;
    }

    //Sets other player variable
    [ClientRpc]
    public void RpcSetOtherPlayeerVariable()
    {
        List<Interactions> playersConnected = FindObjectsOfType<Interactions>().ToList();

        playersConnected[0].otherPlayer = playersConnected[1].gameObject;
        playersConnected[1].otherPlayer = playersConnected[0].gameObject;
    }

}

//Checks for game field
public class Checks
{
    //Checks if player can place on selected field
    public static bool CanMoveHere(FieldData.CaptureState state, FieldData.CaptureState playerTag)
    {
        if (state == playerTag || state == FieldData.CaptureState.Select) return true;

        return false;
    }

    //Gets the state of fields
    public static FieldData.CaptureState GetFieldState(GameObject field)
    {
        FieldData fieldData = field.GetComponent<FieldData>();
        if(fieldData != null)
        {
            return fieldData.fieldState;
        }

        return FieldData.CaptureState.Clear;
    }

    //Returnes the opposite of the player tag given
    public static FieldData.CaptureState GetOppositeOfPlayerTag(FieldData.CaptureState playerTag)
    {
        if (playerTag == FieldData.CaptureState.Player1)
        {
            return FieldData.CaptureState.Player2;
        }
        else
        {
            return FieldData.CaptureState.Player1;
        }
    }
}