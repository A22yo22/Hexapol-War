using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveMap : NetworkBehaviour
{
    public void SaveGameMap(FieldSpawner fieldSpawner)
    {
        PlayerPrefs.SetInt("fieldsSpawned", fieldSpawner.fieldsSpawned.Count);

        int count = 0;
        foreach(GameObject field in fieldSpawner.fieldsSpawned)
        {
            FieldData fieldData = field.GetComponent<FieldData>();
            PlayerPrefs.SetInt("fildState" + count, (int)fieldData.fieldState);

            count++;
        }

        Debug.Log("--Saved Map--" + " Found fields: " + count);
    }

    public void LoadGameMap(FieldSpawner fieldSpawner)
    {
        int fieldsSpawned = PlayerPrefs.GetInt("fieldsSpawned");

        for (int i = 0; i < fieldsSpawned; i++)
        {
            FindObjectsOfType<PlayerInteractions>()[1].CmdSetFieldState(fieldSpawner.fieldsSpawned[i].GetComponent<NetworkIdentity>(), (FieldData.CaptureState)PlayerPrefs.GetInt("fildState" + i));
        }

        Debug.Log("--Loaded Map--" + " Fields: " + fieldsSpawned);
    }
}
