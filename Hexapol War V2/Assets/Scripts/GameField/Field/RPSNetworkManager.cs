using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RPSNetworkManager : NetworkBehaviour
{
    //Tell the other player the selected rps of the current player
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

    //Tell all player who won
    [Command]
    public void CmdTellWinner(string winner)
    {
        RpcTellWinner(winner);
    }

    [ClientRpc]
    public void RpcTellWinner(string winner)
    {
        Debug.Log(winner);
    }
}
