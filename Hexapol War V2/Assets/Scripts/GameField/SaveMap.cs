using Mirror;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveMap : NetworkBehaviour
{
    public string saveFilePath = "fildData.txt";

    public void SaveGameMap()
    {
        PlayerPrefs.SetInt("fieldsSpawned", FieldSpawner.instance.fieldsSpawned.Count);

        int count = 0;
        foreach(GameObject field in FieldSpawner.instance.fieldsSpawned)
        {
            FieldData fieldData = field.GetComponent<FieldData>();
            PlayerPrefs.SetInt("fildState" + count, (int)fieldData.fieldState);

            Debug.Log(fieldData.fieldState);

            count++;

            //field.transform.position = new Vector3(field.transform.position.x, count * 0.25f, field.transform.position.z);
        }

        Debug.Log("--Saved Map--" + " Found fields: " + count);
    }

    public void LoadGameMap()
    {
        int fieldsSpawned = PlayerPrefs.GetInt("fieldsSpawned");

        foreach (PlayerInteractions player in FindObjectsOfType<PlayerInteractions>())
        {
            for (int i = 0; i < fieldsSpawned; i++)
            {
                if (player.isOwned)
                {
                    Debug.Log((FieldData.CaptureState)PlayerPrefs.GetInt("fildState" + i));
                    player.CmdSetFieldState(
                        FieldSpawner.instance.fieldsSpawned[i].GetComponent<NetworkIdentity>(),
                        (FieldData.CaptureState)PlayerPrefs.GetInt("fildState" + i)
                    );

                    //FieldSpawner.instance.fieldsSpawned[i].transform.position = new Vector3(FieldSpawner.instance.fieldsSpawned[i].transform.position.x, i * 0.25f, FieldSpawner.instance.fieldsSpawned[i].transform.position.z);
                }
            }
        }

        Debug.Log("--Loaded Map--" + " Fields: " + fieldsSpawned);
    }
}
