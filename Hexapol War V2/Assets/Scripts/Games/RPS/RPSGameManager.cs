using Mirror;
using System.Collections;
using System.Collections.Generic;
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
        if (player1Choice == player2Choice)
        {
            FindObjectOfType<RPSNetworkManager>().CmdTellWinner("Same");
        }

        if (player1Choice == RPS.Rock && player2Choice == RPS.Scissor)
        {
            FindObjectOfType<RPSNetworkManager>().CmdTellWinner("Player 1 won");
        }
        else if (player1Choice == RPS.Scissor && player2Choice == RPS.Paper)
        {
            FindObjectOfType<RPSNetworkManager>().CmdTellWinner("Player 1 won");
        }
        else if (player1Choice == RPS.Paper && player2Choice == RPS.Rock)
        {
            FindObjectOfType<RPSNetworkManager>().CmdTellWinner("Player 1 won");
        }

        //Player two winns

        else if (player1Choice == RPS.Scissor && player2Choice == RPS.Rock)
        {
            FindObjectOfType<RPSNetworkManager>().CmdTellWinner("Player 2 won");
        }
        else if (player1Choice == RPS.Paper && player2Choice == RPS.Scissor)
        {
            FindObjectOfType<RPSNetworkManager>().CmdTellWinner("Player 2 won");
        }
        else if (player1Choice == RPS.Rock && player2Choice == RPS.Paper)
        {
            FindObjectOfType<RPSNetworkManager>().CmdTellWinner("Player 2 won");
        }
        
    }
}
