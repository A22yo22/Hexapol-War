using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    public static PlayerStats instance;

    public float time;
    public int fieldsCaptured = -1;

    public bool gameStarted = false;

    public List<GameObject> remainingFields;

    private void Awake()
    {
        if (instance == null) { instance = this; }
    }

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
    int blueFieldsFound = 0;
    int redFieldsFound = 0;
    public void RefreshRemainingFields()
    {
        StartCoroutine(RemainingCounterCheck());
    }

    IEnumerator RemainingCounterCheck()
    {
        yield return new WaitForSeconds(0.2f);

        foreach (GameObject field in FieldSpawner.instance.fieldsSpawned)
        {
            if (field.GetComponent<FieldData>().fieldState == FieldData.CaptureState.Player1)
            {
                blueFieldsFound++;
            }
            else if (field.GetComponent<FieldData>().fieldState == FieldData.CaptureState.Player2)
            {
                redFieldsFound++;
            }
        }

        Debug.Log("Blue: " + blueFieldsFound);
        Debug.Log("Red: " + redFieldsFound);

        if (blueFieldsFound == 0)
        {
            CmdGameOver((int)time);
        }
        else if (redFieldsFound == 0)
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
