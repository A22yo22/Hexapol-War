using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    public float time;
    public int fieldsCaptured = -1;

    public bool gameStarted = false;

    public List<GameObject> remainingFields;

    private void Update()
    {
        time += Time.deltaTime;
    }

    //Starts timer
    public void StartTimer()
    {
        time = 0;
    }

    //Refresh remaining fields list
    public void RefreshRemainingFields()
    {
        FieldManager fieldManager = FindObjectOfType<FieldManager>();
        fieldManager.remainingFieldsPlayer1.Clear();
        fieldManager.remainingFieldsPlayer2.Clear();

        foreach (var field in fieldManager.usedFields)
        {
            if (field.GetComponent<FieldData>().fieldState == FieldData.CaptureState.Player1)
            {
                fieldManager.remainingFieldsPlayer1.Add(field);
            }
            else if (field.GetComponent<FieldData>().fieldState == FieldData.CaptureState.Player2)
            {
                fieldManager.remainingFieldsPlayer2.Add(field);

            }
        }

        if (GetComponent<PlayerInteractions>().thisPlayerTag == FieldData.CaptureState.Player1)
        {
            fieldsCaptured++;
        }
        else if (GetComponent<PlayerInteractions>().thisPlayerTag == FieldData.CaptureState.Player2)
        {
            fieldsCaptured++;
        }

        RemainingCounterCheck();
    }

    public void RemainingCounterCheck()
    {
        FieldManager fieldManager = FindObjectOfType<FieldManager>();

        if (fieldManager.remainingFieldsPlayer1.Count == 0)
        {
            CmdGameOver((int)time);
        }
        else if (fieldManager.remainingFieldsPlayer2.Count == 0)
        {
            CmdGameOver((int)time);
        }
    }

    //Network remaining player fields
    
    [Command]
    public void CmdAddFieldTo(NetworkIdentity id)
    {
        RpcAddFieldTo(id);
    }
    [ClientRpc]
    public void RpcAddFieldTo(NetworkIdentity id)
    {
        if (FindObjectOfType<FieldManager>().usedFields.IndexOf(id.gameObject) == -1)
        {
            FindObjectOfType<FieldManager>().usedFields.Add(id.gameObject);
        }
    }
    

    //Network game over screen
    [Command]
    public void CmdGameOver(int roundedTime)
    {
        RpcGameOver(roundedTime);
    }
    [ClientRpc]
    public void RpcGameOver(int roundedTime)
    {
        FindObjectOfType<GameOverManager>().GameOver(roundedTime);
    }
}
