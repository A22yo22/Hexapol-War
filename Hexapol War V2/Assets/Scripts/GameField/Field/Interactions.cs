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

    private void Update()
    {
        //Check if is local player
        if (!isLocalPlayer)
        {
            return;
        }
    }



    //Network
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
}