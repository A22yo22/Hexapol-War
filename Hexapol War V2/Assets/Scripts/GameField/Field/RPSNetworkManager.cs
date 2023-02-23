using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RPSNetworkManager : NetworkBehaviour
{
    [Command]
    public void CmdSelectRPS(RPSGameManager.RPS choice, bool isPlayer1)
    {
        RpcRockSelected(choice, isPlayer1);
    }

    [ClientRpc]
    public void RpcRockSelected(RPSGameManager.RPS choice, bool isPlayer1)
    {
        FindObjectOfType<RPSGameManager>().CallChoice(choice, isPlayer1);
    }
}
