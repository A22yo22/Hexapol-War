using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerInteractions : NetworkBehaviour
{
    public FieldData.CaptureState thisPlayerTag;

    public int radius = 3;
    public LayerMask detectionLayer;

    public PlayerInteractions otherPlayer;

    GameObject selectedField;
    GameObject lastSelectedField;

    public bool canMove = true;

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

            FindObjectOfType<FieldSpawner>().indicator.SetActive(true);
            firstTimePlayerCanMove = false;
        }
        else if (isClientOnly)
        {
            thisPlayerTag = FieldData.CaptureState.Player2;
            FindObjectOfType<FieldSpawner>().indicator.SetActive(false);
            firstTimePlayerCanMove = true;
        }

        //Add event to ready up button
        UiManager.instance.button.onClick.AddListener(ReadyUp);

        //Get lobby manger
        lobbyManager = FindObjectOfType<LobbyManager>();


    }

    bool firstTileSpawned;
    bool firstTimePlayerCanMove = true;
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
                for (int i = 0; i < fieldSpawner.fieldsSpawned.Count; i++)
                {
                    CmdAddHexagons(fieldSpawner.fieldsSpawned[i].GetComponent<NetworkIdentity>());
                }
            }
            else if (isClientOnly)
            {
                canMove = false;
            }


            //Set field parent
            if (isClientOnly && FindObjectOfType<LobbyManager>().fieldssSpawned.Count != 0)
            {
                foreach (GameObject field in FindObjectOfType<LobbyManager>().fieldssSpawned)
                {
                    field.transform.SetParent(FindObjectOfType<FieldSpawner>().parent);
                }
            }

            if (lobbyManager.fieldssSpawned.Count == 0) return;

            //Set player start pos
            int selectedField = Random.Range(0, lobbyManager.fieldssSpawned.Count);
            while (lobbyManager.fieldssSpawned[selectedField].GetComponent<FieldData>().fieldState != FieldData.CaptureState.Clear)
            {
                selectedField = Random.Range(0, lobbyManager.fieldssSpawned.Count);
            }

            CmdSetFieldState(lobbyManager.fieldssSpawned[selectedField].GetComponent<NetworkIdentity>(), thisPlayerTag);
            lastSelectedField = lobbyManager.fieldssSpawned[selectedField];

            //Get other player
            CmdSetOtherPlayeerVariable();

            GetComponent<PlayerStats>().fieldsCaptured = 0;

            firstTileSpawned = true;
        }

        if (canMove)
        {
            Debug.Log(firstTimePlayerCanMove);
            if (!firstTimePlayerCanMove)
            {
                FindObjectOfType<FieldSpawner>().indicator.SetActive(true);
                firstTimePlayerCanMove = false;
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
                    FieldData.CaptureState selectedFieldState = Checks.GetFieldState(selectedField);

                    //Checks if player can move on this selected field
                    if (Checks.CanMoveHere(selectedFieldState, thisPlayerTag))
                    {
                        //Raise up
                        selectedField.transform.position = new Vector3(selectedField.transform.position.x, 0.4f, selectedField.transform.position.z);
                        Move(selectedFieldState);
                    }
                    else if (selectedFieldState == Checks.GetOppositeOfPlayerTag(thisPlayerTag) && selectedFields.IndexOf(selectedField) != -1)    //Attack
                    {
                        Attack(selectedField);
                        SwitchPlayerAtMove();
                    }
                    else    //Clear
                    {
                        RestSelectedFields();
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))   //Reset selected fields
            {
                RestSelectedFields();
            }
        }
    }

    //Resets the selected fields
    public void RestSelectedFields()
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

        //Clear List
        selectedFields.RemoveRange(0, selectedFields.Count);
    }

    //Move player to selected field
    public void Move(FieldData.CaptureState fieldState)
    {
        if (fieldState == FieldData.CaptureState.Select)
        {
            CmdSetFieldState(selectedField.GetComponent<NetworkIdentity>(), thisPlayerTag);
            GetComponent<PlayerStats>().remainingFields.Add(selectedField);

            SwitchPlayerAtMove();
        }
        else
        {
            Collider[] surroundingFields = GetSurroundingFields();
            if(surroundingFields.Length > 0)
            {
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
        FindObjectOfType<FieldSpawner>().indicator.SetActive(false);
        firstTimePlayerCanMove = true;
        
        RestSelectedFields();

        //Switch player
        CmdSetPlayerAtMove(Checks.GetOppositeOfPlayerTag(thisPlayerTag));

        if (Checks.GetOppositeOfPlayerTag(thisPlayerTag) == FieldData.CaptureState.Player1)
        {
            CmdSetPlayer1ToMove();
        }
        else if (Checks.GetOppositeOfPlayerTag(thisPlayerTag) == FieldData.CaptureState.Player2)
        {
            RpcSetPlayer2ToMove();
        }

        //Enable other player
        if (otherPlayer != null)
        {
            //otherPlayer.enabled = true;
            //enabled = false;
        }
    }



    //Events
    public void Attack(GameObject fieldToPlayAbout)
    {
        RestSelectedFields();

        CmdStartMinigame(fieldToPlayAbout, lastSelectedField);
    }

    //Network section

    //Mini game stuff


    //Start minigame
    [Command]
    public void CmdStartMinigame(GameObject fieldToPlayAbout, GameObject attackingPlayer)
    {
        FindObjectOfType<MinigameManager>().OpenMiniGameame();
        RpcStartMinigame(fieldToPlayAbout, attackingPlayer);
    }
    [ClientRpc]
    public void RpcStartMinigame(GameObject fieldToPlayAbout, GameObject attackingPlayer)
    {
        FindObjectOfType<MinigameManager>().fieldToPlayAbout = fieldToPlayAbout.GetComponent<FieldData>();
        FindObjectOfType<MinigameManager>().attackingPlayer = attackingPlayer.GetComponent<FieldData>();
        
        FindObjectOfType<MinigameManager>().StartMiniGame();
    }

    //Field stuff
    //Get spawned fields
    [Command]
    public void CmdAddHexagons(NetworkIdentity id)
    {
        RpcAddHexagons(id);
    }
    [ClientRpc]
    public void RpcAddHexagons(NetworkIdentity id)
    {
        if (FindObjectOfType<LobbyManager>().fieldssSpawned.IndexOf(id.gameObject) == -1)
        {
            FindObjectOfType<LobbyManager>().fieldssSpawned.Add(id.gameObject);
        }
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


        if (FindObjectOfType<FieldManager>().usedFields.IndexOf(identity.gameObject) == -1)
        {
            FindObjectOfType<FieldManager>().usedFields.Add(identity.gameObject);
        }

        //Start timer
        FindAnyObjectByType<PlayerStats>().StartTimer();

        //Game started
        GetComponent<PlayerStats>().gameStarted = true;
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
        List<PlayerInteractions> playersConnected = FindObjectsOfType<PlayerInteractions>().ToList();

        FindObjectOfType<FieldManager>().players1 = playersConnected[1];
        FindObjectOfType<FieldManager>().players2 = playersConnected[0];
    }

    //Set player who can move
    [Command]
    public void CmdSetPlayer1ToMove()
    {
        RpcSetPlayer1ToMove();
    }
    [ClientRpc]
    public void RpcSetPlayer1ToMove()
    {
        FindObjectOfType<FieldManager>().players1.canMove = true;
        FindObjectOfType<FieldManager>().players2.canMove = false;
    }
    [ClientRpc]
    public void RpcSetPlayer2ToMove()
    {
        FindObjectOfType<FieldManager>().players2.canMove = true;
        FindObjectOfType<FieldManager>().players1.canMove = false;
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