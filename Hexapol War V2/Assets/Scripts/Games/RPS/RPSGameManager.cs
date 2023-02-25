using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RPSGameManager : NetworkBehaviour
{
    [SyncVar] public RPS player1Choice;
    [SyncVar] public RPS player2Choice;

    [SyncVar] public int howManyPlayersChose;

    public enum RPS
    {
        None,
        Rock,
        Paper,
        Scissor
    }

    public void CallChoice(RPS choice, bool isPlayer1)
    {
        if(isPlayer1)
        {
            if (choice == RPS.Rock)
            {
                player1Choice = RPS.Rock;
            }
            else if (choice == RPS.Paper)
            {
                player1Choice = RPS.Paper;
            }
            else if (choice == RPS.Scissor)
            {
                player1Choice = RPS.Scissor;
            }
        }
        else
        {
            if (choice == RPS.Rock)
            {
                player2Choice = RPS.Rock;
            }
            else if (choice == RPS.Paper)
            {
                player2Choice = RPS.Paper;
            }
            else if (choice == RPS.Scissor)
            {
                player2Choice = RPS.Scissor;
            }
        }
    
        howManyPlayersChose++;

        if (howManyPlayersChose >= 2)
        {
            CheckResults();
        }
    }

    void CheckResults()
    {
        FieldData.CaptureState playerWon = FieldData.CaptureState.Clear;

        if (player1Choice == player2Choice)
        {
            //Do nothing when same
            return;
            //FindObjectOfType<RPSNetworkManager>().CmdTellWinner(FieldData.CaptureState.Clear);
        }

        else if (player1Choice == RPS.Rock && player2Choice == RPS.Scissor)
        {
            playerWon = FieldData.CaptureState.Player1;
        }
        else if (player1Choice == RPS.Scissor && player2Choice == RPS.Paper)
        {
            playerWon = FieldData.CaptureState.Player1;
        }
        else if (player1Choice == RPS.Paper && player2Choice == RPS.Rock)
        {
            playerWon = FieldData.CaptureState.Player1;
        }

        //Player two winns

        else if (player1Choice == RPS.Scissor && player2Choice == RPS.Rock)
        {
            playerWon = FieldData.CaptureState.Player2;
        }
        else if (player1Choice == RPS.Paper && player2Choice == RPS.Scissor)
        {
            playerWon = FieldData.CaptureState.Player2;
        }
        else if (player1Choice == RPS.Rock && player2Choice == RPS.Paper)
        {
            playerWon = FieldData.CaptureState.Player2;
        }


        List<RPSNetworkManager> rpsNetworkManagers = FindObjectsOfType<RPSNetworkManager>().ToList();
        foreach (RPSNetworkManager rpsManager in rpsNetworkManagers)
        {
            if (rpsManager.gameObject.GetComponent<Interactions>().thisPlayerTag != FieldData.CaptureState.Clear)
            {
                rpsManager.CmdSetWinner(playerWon);
            }
        }
    }
}
