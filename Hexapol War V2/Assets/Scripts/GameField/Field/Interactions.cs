using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactions : NetworkBehaviour
{
    public FieldData.CaptureState thisPlayerTag;

    public int radius = 3;
    public LayerMask detectionLayer;

    public Interactions otherPlayer;

    GameObject selectedField;
    GameObject lastSelectedField;

    //Lists
    public List<GameObject> selectedFields;


    //References
    LobbyManager lobbyManager;

    void Start()
    {
        //Check if is local player
        if (!isLocalPlayer) return;

        //Asign playerTag
        if (isServer)
        {
            thisPlayerTag = FieldData.CaptureState.Player1;
        }
        else if (isClientOnly)
        {
            thisPlayerTag = FieldData.CaptureState.Player2;

            CmdSetPlayer2(gameObject);
        }

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
                CmdSetPlayer1(gameObject);

                for (int i = 0; i < fieldSpawner.hexagonsSpawned.Count; i++)
                {
                    CmdAddHexagons(fieldSpawner.hexagonsSpawned[i].GetComponent<NetworkIdentity>());
                }
            }
            else if (isClientOnly)
            {
                CmdSetPlayer2(gameObject);
            }

            if(lobbyManager.fieldssSpawned.Count == 0) return;

            //Set player start pos
            int selectedField = Random.Range(0, lobbyManager.fieldssSpawned.Count);
            CmdSetFieldState(lobbyManager.fieldssSpawned[selectedField].GetComponent<NetworkIdentity>(), thisPlayerTag);
            lastSelectedField = lobbyManager.fieldssSpawned[selectedField];

            //Get other player
            CmdSetOtherPlayeerVariable();

            firstTileSpawned = true;
        }
        else if (FindObjectOfType<LobbyManager>().playerReady == 2)
        {
            if (isClientOnly) enabled = false;
        }

        if (Input.GetMouseButtonDown(0))
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
                        if (selectedFields[i].GetComponent<FieldData>().fieldState == FieldData.CaptureState.Select)
                        {
                            selectedFields[i].GetComponent<FieldData>().SwitchCaptureState(FieldData.CaptureState.Clear);
                        }
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
                if (selectedFields[i].GetComponent<FieldData>().fieldState == FieldData.CaptureState.Select)
                {
                    selectedFields[i].GetComponent<FieldData>().SwitchCaptureState(FieldData.CaptureState.Clear);
                }
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
        else
        {
            Collider[] surroundingFields = GetSurroundingFields();
            if(surroundingFields.Length > 0)
            {
                Debug.Log(surroundingFields.Length);

                foreach(Collider field in surroundingFields)
                {
                    if (field.gameObject.GetComponent<FieldData>().fieldState == FieldData.CaptureState.Clear)
                    {
                        field.gameObject.GetComponent<FieldData>().SwitchCaptureState(FieldData.CaptureState.Select);
                        selectedFields.Add(field.gameObject);
                    }
                    else if (field.gameObject.GetComponent<FieldData>().fieldState == Checks.GetOppositeOfPlayerTag(thisPlayerTag))
                    {
                        field.gameObject.GetComponent<FieldData>().SwitchCaptureState(Checks.GetOppositeOfPlayerTag(thisPlayerTag));
                        selectedFields.Add(field.gameObject);
                    }
                }
            }
        }
    }

    //Gets the surrounding fields of the selected object
    Collider[] GetSurroundingFields()
    {
        Collider[] fieldsFound = Physics.OverlapSphere(selectedField.transform.position, radius, detectionLayer);
        return fieldsFound;
    }

    //Switches the player and moves to the next one
    public void SwitchPlayerAtMove()
    {
        //Reset used field possitions
        selectedField.transform.position = new Vector3(selectedField.transform.position.x, 0f, selectedField.transform.position.z);
        lastSelectedField.transform.position = new Vector3(lastSelectedField.transform.position.x, 0, lastSelectedField.transform.position.z);

        //Reset selected fields
        for(int i = 0; i < selectedFields.Count; i++)
        {
            if (selectedFields[i].GetComponent<FieldData>().fieldState == FieldData.CaptureState.Select)
            {
                selectedFields[i].GetComponent<FieldData>().SwitchCaptureState(FieldData.CaptureState.Clear);
            }
        }

        //Switch player
        CmdSetPlayerAtMove(Checks.GetOppositeOfPlayerTag(thisPlayerTag));

        //Enable other player
        if (otherPlayer != null)
        {
            otherPlayer.enabled = true;
            enabled = false;
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
    [Command]
    public void CmdSetPlayerAtMove(FieldData.CaptureState playerToWhoWillHaveTheMove)
    {
        FindObjectOfType<FieldManager>().playerAtMove = playerToWhoWillHaveTheMove;
    }

    //Sets other player variable
    [Command]
    public void CmdSetOtherPlayeerVariable()
    {
        RpcSetOtherPlayeerVariable();
    }
    [ClientRpc]
    public void RpcSetOtherPlayeerVariable()
    {
        List<Interactions> playersConnected = FindObjectsOfType<Interactions>().ToList();

        playersConnected[0].otherPlayer = playersConnected[1];
        playersConnected[1].otherPlayer = playersConnected[0];
    }

    //Set players in field manager
    [Command]
    public void CmdSetPlayer1(GameObject player1)
    {
        FindObjectOfType<FieldManager>().players1 = player1;
    }
    [Command]
    public void CmdSetPlayer2(GameObject player2)
    {
        FindObjectOfType<FieldManager>().players2 = player2;
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