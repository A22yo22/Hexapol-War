using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    [SyncVar]
    public List<GameObject> lastSelectedFields;

    public bool canMove = true;

    public Material enemyColor;

    //Lists
    public List<GameObject> selectedFields;

    void Start()
    {
        //Check if is local player
        if (!isLocalPlayer)
        {
            //Disabel can move text
            Destroy(transform.GetChild(0).gameObject);

            return;
        }

        //Asign playerTag
        if (isServer)
        {
            thisPlayerTag = FieldData.CaptureState.Player1;
            transform.Find("CanMove").gameObject.SetActive(false);
            firstTimePlayerCanMove = false;
        }
        else if (isClientOnly)
        {
            thisPlayerTag = FieldData.CaptureState.Player2;
            transform.Find("CanMove").gameObject.SetActive(false);
            firstTimePlayerCanMove = true;
            CmdSetEnemyColor(GetComponent<NetworkIdentity>());
        }

        //Add player to GameDataHolder
        CmdAddPlayerToGameDataList(GetComponent<NetworkIdentity>());

        //Add event to ready up button
        UiManager.instance.button.onClick.AddListener(ReadyUp);
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
            if (isServer)
            {
                for (int i = 0; i < FieldSpawner.instance.fieldsSpawned.Count; i++)
                {
                    CmdAddHexagons(FieldSpawner.instance.fieldsSpawned[i].GetComponent<NetworkIdentity>());
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
                    field.transform.SetParent(FieldSpawner.instance.parent);
                }
            }

            if (FieldSpawner.instance.fieldsSpawned.Count == 0) return;

            //Get other player
            //CmdSetOtherPlayeerVariable();

            GetComponent<PlayerStats>().fieldsCaptured = 0;

            firstTileSpawned = true;
        }

        if (canMove)
        {
            //Disabel can move text
            transform.GetChild(0).gameObject.SetActive(true);

            if (!firstTimePlayerCanMove)
            {
                FieldSpawner.instance.indicator.SetActive(true);
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
                    if (selectedField != null) lastSelectedFields.Add(selectedField);
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
        else
        {
            //Disabel can move text
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }


    //Resets the selected fields
    public void RestSelectedFields()
    {
        //Reset used field possitions
        selectedField.transform.position = new Vector3(selectedField.transform.position.x, 0f, selectedField.transform.position.z);
        foreach (GameObject field in lastSelectedFields)
        {
            field.transform.position = new Vector3(field.transform.position.x, 0, field.transform.position.z);
        }

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
            selectedField.GetComponent<FieldData>().SwitchCaptureState(thisPlayerTag);
            CmdSetFieldState(selectedField.GetComponent<NetworkIdentity>(), thisPlayerTag);

            SaveMap.instance.SaveGameMap();

            GetComponent<PlayerStats>().remainingFields.Add(selectedField);

            SwitchPlayerAtMove();
            lastSelectedFields.Clear();
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
        FieldSpawner.instance.indicator.SetActive(false);
        firstTimePlayerCanMove = true;
        
        RestSelectedFields();

        if (Checks.GetOppositeOfPlayerTag(thisPlayerTag) == FieldData.CaptureState.Player1)
        {
            CmdSetPlayer1ToMove();
        }
        else if (Checks.GetOppositeOfPlayerTag(thisPlayerTag) == FieldData.CaptureState.Player2)
        {
            RpcSetPlayer2ToMove();
        }

        SaveMap.instance.SaveGameMap();
    }

    //Events
    public void Attack(GameObject fieldToPlayAbout)
    {
        RestSelectedFields();

        //foreach (PlayerInteractions player in FindObjectsOfType<PlayerInteractions>())
        //{
        //    player.GetComponent<Health>().ResetMinigameHealth(player.lastSelectedFields.Count);
        //}

        CmdStartMinigame(fieldToPlayAbout, lastSelectedFields, gameObject);
    }

    //Network section

    //Start minigame
    [Command]
    public void CmdStartMinigame(GameObject fieldToPlayAbout, List<GameObject> attackingPlayers, GameObject attackingPlayer)
    {
        MiniGameManager.instance.OpenMiniGameame();

        RpcStartMinigame(fieldToPlayAbout, attackingPlayers, attackingPlayer);
    }
    [ClientRpc]
    public void RpcStartMinigame(GameObject fieldToPlayAbout, List<GameObject> attackingPlayers, GameObject attackingPlayer)
    {
        MiniGameManager.instance.fieldToPlayAbout = fieldToPlayAbout.GetComponent<FieldData>();

        foreach (GameObject field in attackingPlayers)
        {
            MiniGameManager.instance.attackingPlayers.Add(field.GetComponent<FieldData>());
        }

        foreach (PlayerInteractions player in GameDataHolder.instance.players)
        {
            if (player.gameObject == attackingPlayer)
            {
                attackingPlayer.GetComponent<Health>().ResetMinigameHealth(attackingPlayers.Count);
            }
            else
            {
                player.GetComponent<Health>().ResetMinigameHealth(1);
            }
        }

        MiniGameManager.instance.StartMiniGame();
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
        if(SaveMap.instance.canSpawn) RpcSetFieldState(identity, state);
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
        PlayerStats.instance.StartTimer();

        //Game started
        GetComponent<PlayerStats>().gameStarted = true;
    }

    //Adds a field to the fields spawed list
    [Command]
    public void CmdAddToList(NetworkIdentity id)
    {
        RpcAddToList(id);
    }

    [ClientRpc]
    public void RpcAddToList(NetworkIdentity id)
    {
        id.transform.SetParent(FieldSpawner.instance.transform);
        if(!FieldSpawner.instance.fieldsSpawned.Contains(id.gameObject)) FieldSpawner.instance.fieldsSpawned.Add(id.gameObject);
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
        foreach (PlayerInteractions player in GameDataHolder.instance.players)
        {
            if (player.thisPlayerTag == FieldData.CaptureState.Player1) player.canMove = true;
            else if (player.thisPlayerTag == FieldData.CaptureState.Player2) player.canMove = false;
        }
    }
    [ClientRpc]
    public void RpcSetPlayer2ToMove()
    {
        foreach (PlayerInteractions player in GameDataHolder.instance.players)
        {
            if (player.thisPlayerTag == FieldData.CaptureState.Player1) player.canMove = false;
            else if (player.thisPlayerTag == FieldData.CaptureState.Player2) player.canMove = true;
        }
    }

    [Command]
    public void CmdSetEnemyColor(NetworkIdentity id)
    {
        RpcSetEnemyColor(id);
    }
    [ClientRpc]
    public void RpcSetEnemyColor(NetworkIdentity id)
    { 
        id.GetComponent<PlayerInteractions>().SetColorRed();
    }

    [Command]
    public void CmdAddPlayerToGameDataList(NetworkIdentity player)
    {
        RpcAddPlayerToGameDataList(player);
    }
    [ClientRpc]
    public void RpcAddPlayerToGameDataList(NetworkIdentity player)
    {
        GameDataHolder.instance.players.Add(player.GetComponent<PlayerInteractions>());
    }

    public void SetColorRed()
    {
        transform.Find("Player").GetComponent<MeshRenderer>().material = enemyColor;
    }
}

//Checks for game field
public class Checks
{
    //Checks if player can place on selected field
    public static bool CanMoveHere(FieldData.CaptureState state, FieldData.CaptureState playerTag)
    {
        //if (state == FieldData.CaptureState.Select) return true;
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