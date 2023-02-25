using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class RPSUiManager : NetworkBehaviour
{
    public GameObject RPSUi;

    public TMP_Text selectedItem;

    public Button rock;
    public Button paper;
    public Button scissor;

    public void SetUp()
    {
        rock.onClick.AddListener(RockSelected);
        paper.onClick.AddListener(PaperSelected);
        scissor.onClick.AddListener(ScissorSelected);
    }

    void HideRPSUi()
    {
        RPSUi.SetActive(false);
    }

    public void RockSelected()
    {
        List<RPSNetworkManager> rpsNetworkManagers = FindObjectsOfType<RPSNetworkManager>().ToList();
        foreach(RPSNetworkManager rpsManager in rpsNetworkManagers)
        {
            if (rpsManager.gameObject.GetComponent<Interactions>().thisPlayerTag != FieldData.CaptureState.Clear)
            {
                rpsManager.CmdSelectRPS(RPSGameManager.RPS.Rock, isServer);
            }
        }

        HideRPSUi();
        selectedItem.text += ": Rock";
    }
    public void ScissorSelected()
    {
        List<RPSNetworkManager> rpsNetworkManagers = FindObjectsOfType<RPSNetworkManager>().ToList();
        foreach (RPSNetworkManager rpsManager in rpsNetworkManagers)
        {
            if (rpsManager.gameObject.GetComponent<Interactions>().thisPlayerTag != FieldData.CaptureState.Clear)
            {
                rpsManager.CmdSelectRPS(RPSGameManager.RPS.Scissor, isServer);
            }
        }

        HideRPSUi();
        selectedItem.text += ": Scissor";
    }
    public void PaperSelected()
    {
        List<RPSNetworkManager> rpsNetworkManagers = FindObjectsOfType<RPSNetworkManager>().ToList();
        foreach (RPSNetworkManager rpsManager in rpsNetworkManagers)
        {
            if (rpsManager.gameObject.GetComponent<Interactions>().thisPlayerTag != FieldData.CaptureState.Clear)
            {
                rpsManager.CmdSelectRPS(RPSGameManager.RPS.Paper, isServer);
            }
        }

        HideRPSUi();
        selectedItem.text += ": Paper";
    }
}
