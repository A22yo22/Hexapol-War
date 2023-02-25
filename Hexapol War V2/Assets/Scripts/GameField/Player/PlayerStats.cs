using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    public float time;

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
        remainingFields.Clear();

        FieldData.CaptureState thisPlayerTag = GetComponent<PlayerInteractions>().thisPlayerTag;
        if (thisPlayerTag != FieldData.CaptureState.Clear)
        {
            foreach (var field in FindObjectOfType<FieldSpawner>().fieldsSpawned)
            {
                if (field.GetComponent<FieldData>().fieldState == thisPlayerTag)
                {
                    remainingFields.Add(field);
                }
            }
        }

        RemainingCounterCheck();
    }

    void RemainingCounterCheck()
    {
        if (GetComponent<PlayerInteractions>().thisPlayerTag != FieldData.CaptureState.Clear)
        {
            Debug.Log(remainingFields.Count);
            if (remainingFields.Count <= 0)
            {
                CmdGameOver((int)time);
            }
        }
    }

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
