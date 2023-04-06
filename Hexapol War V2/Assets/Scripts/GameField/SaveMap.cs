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

        StreamWriter saveFile = new StreamWriter(saveFilePath);

        foreach (GameObject field in FieldSpawner.instance.fieldsSpawned)
        {
            FieldData fieldData = field.GetComponent<FieldData>();
            saveFile.WriteLine((int)fieldData.fieldState);
        }


        /*
        PlayerPrefs.SetInt("fieldsSpawned", FieldSpawner.instance.fieldsSpawned.Count);

        int count = 0;
        foreach(GameObject field in FieldSpawner.instance.fieldsSpawned)
        {
            FieldData fieldData = field.GetComponent<FieldData>();
            PlayerPrefs.SetInt("fildState" + count, (int)fieldData.fieldState);

            Debug.Log(fieldData.fieldState+ ": " + count);

            count++;
        }

        //Debug.Log("--Saved Map--" + " Found fields: " + count);
        */
    }

    public void LoadGameMap()
    {
        StreamReader saveFile = new StreamReader(saveFilePath);

        foreach (PlayerInteractions player in FindObjectsOfType<PlayerInteractions>())
        {
            if (player.isOwned)
            {
                int count = 0;
                while ((saveFile.ReadLine()) != null)
                {
                    int value;
                    if (int.TryParse(saveFile.ReadLine(), out value))
                    {
                        if (player.isOwned)
                        {
                            player.CmdSetFieldState(FieldSpawner.instance.fieldsSpawned[count].GetComponent<NetworkIdentity>(), (FieldData.CaptureState)value);
                        }

                        count++;
                    }
                }
            }
        }

        /*
        int fieldsSpawned = PlayerPrefs.GetInt("fieldsSpawned");

        foreach (PlayerInteractions player in FindObjectsOfType<PlayerInteractions>())
        {
            for (int i = 0; i < fieldsSpawned; i++)
            {
                if (player.isOwned)
                {
                    Debug.Log((FieldData.CaptureState)PlayerPrefs.GetInt("fildState" + i) + ": " + i);
                    player.CmdSetFieldState(
                        FieldSpawner.instance.fieldsSpawned[i].GetComponent<NetworkIdentity>(),
                        (FieldData.CaptureState)PlayerPrefs.GetInt("fildState" + i)
                    );
                }
            }
        }

        //Debug.Log("--Loaded Map--" + " Fields: " + fieldsSpawned);
        */
    }
}
