using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPSGameManager : NetworkBehaviour
{
    public RPS player1Choice;
    public RPS player2Choice;

    public enum RPS
    {
        Rock,
        Paper,
        Scissor
    }
}
