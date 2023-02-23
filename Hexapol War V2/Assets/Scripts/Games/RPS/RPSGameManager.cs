using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPSGameManager : NetworkBehaviour
{
    [SyncVar] public RPS player1Choice;
    [SyncVar] public RPS player2Choice;

    public enum RPS
    {
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
    }
}
