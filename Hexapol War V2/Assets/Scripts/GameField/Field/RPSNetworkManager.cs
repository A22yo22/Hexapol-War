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
        RpcTellWinner(winner);
    }

    [ClientRpc]
    public void RpcTellWinner(FieldData.CaptureState winner)
    {
        //Activate game field
        foreach (GameObject gameFieldObject in FindObjectOfType<MinigameManager>().gameFieldFolder)
        {
            gameFieldObject.transform.position = Vector3.zero;
        }

        //Set winner fields
        FindObjectOfType<MinigameManager>().fieldToPlayAbout.SwitchCaptureState(winner);
        FindObjectOfType<MinigameManager>().attackingPlayer.SwitchCaptureState(winner);

        foreach (PlayerInteractions playerInteractions in FindObjectsOfType<PlayerInteractions>())
        {
            if (playerInteractions.thisPlayerTag != FieldData.CaptureState.Clear)
            {
                playerInteractions.GetComponent<PlayerStats>().RefreshRemainingFields();
            }
        }

        //Destroy runnting minigame
        Destroy(FindObjectOfType<MinigameManager>().minigameRunning);
    }
}
