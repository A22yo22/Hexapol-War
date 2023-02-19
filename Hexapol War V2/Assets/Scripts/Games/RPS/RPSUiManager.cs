using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RPSUiManager : NetworkBehaviour
{
    public GameObject RPSUi;

    public Button rock;
    public Button paper;
    public Button scissor;

    public void SetUp()
    {
        rock.onClick.AddListener(CmdRockSelected);
        paper.onClick.AddListener(CmdPaperSelected);
        scissor.onClick.AddListener(CmdScissorSelected);
    }

    void HideRPSUi()
    {
        RPSUi.SetActive(false);
    }

    //Network stuff

    //Rock
    [Command]
    public void CmdRockSelected()
    {
        RpcRockSelected(FindObjectOfType<Interactions>().thisPlayerTag);
        HideRPSUi();
    }
    [ClientRpc]
    public void RpcRockSelected(FieldData.CaptureState currentPlayer)
    {
        if (currentPlayer == FieldData.CaptureState.Player1)
        {
            FindObjectOfType<RPSGameManager>().player1Choice = RPSGameManager.RPS.Rock;
        }
        else
        {
            FindObjectOfType<RPSGameManager>().player2Choice = RPSGameManager.RPS.Rock;
        }
    }

    //Paper
    [Command]
    public void CmdPaperSelected()
    {
        RpcPaperSelected(FindObjectOfType<Interactions>().thisPlayerTag);
        HideRPSUi();
    }
    [ClientRpc]
    public void RpcPaperSelected(FieldData.CaptureState currentPlayer)
    {
        if (currentPlayer == FieldData.CaptureState.Player1)
        {
            FindObjectOfType<RPSGameManager>().player1Choice = RPSGameManager.RPS.Paper;
        }
        else
        {
            FindObjectOfType<RPSGameManager>().player2Choice = RPSGameManager.RPS.Paper;
        }
    }

    //Scissor
    [Command]
    public void CmdScissorSelected()
    {
        RpcScissorSelected(FindObjectOfType<Interactions>().thisPlayerTag);
        HideRPSUi();
    }
    [ClientRpc]
    public void RpcScissorSelected(FieldData.CaptureState currentPlayer)
    {
        if (currentPlayer == FieldData.CaptureState.Player1)
        {
            FindObjectOfType<RPSGameManager>().player1Choice = RPSGameManager.RPS.Scissor;
        }
        else
        {
            FindObjectOfType<RPSGameManager>().player2Choice = RPSGameManager.RPS.Scissor;
        }
    }
}
