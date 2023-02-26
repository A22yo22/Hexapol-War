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
    public void CmdSetWinner(FieldData.CaptureState winner)
    {
        GetComponent<PlayerStats>().RefreshRemainingFields();
        RpcTellWinner(winner);
    }

    [ClientRpc]
    public void RpcTellWinner(FieldData.CaptureState winner)
    {
        MinigameManager minigameManager = FindObjectOfType<MinigameManager>();

        //Activate game field
        foreach (GameObject gameFieldObject in minigameManager.gameFieldFolder)
        {
            gameFieldObject.transform.position = Vector3.zero;
        }

        //Set winner fields
        minigameManager.fieldToPlayAbout.SwitchCaptureState(winner);
        minigameManager.attackingPlayer.SwitchCaptureState(winner);

        //Destroy runnting minigame
        Destroy(FindObjectOfType<MinigameManager>().minigameRunning);
    }
}
