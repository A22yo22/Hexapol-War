using Mirror;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveMap : NetworkBehaviour
{
    public static SaveMap instance;

    [SyncVar]
    public bool canSpawn = true;


    private void Start()
    {
        if (instance == null) { instance = this; }
    }

    public void SaveGameMap()
    {
        PlayerPrefs.SetInt("fieldsSpawned", FieldSpawner.instance.fieldsSpawned.Count);

        int count = 0;
        foreach (GameObject field in FieldSpawner.instance.fieldsSpawned)
        {
            FieldData fieldData = field.GetComponent<FieldData>();
            PlayerPrefs.SetInt("fildState" + count, (int)fieldData.fieldState);

            count++;
        }

        //Debug.Log("--Saved Map--" + " Found fields: " + count);
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
                    if ((FieldData.CaptureState)PlayerPrefs.GetInt("fildState" + i) != FieldData.CaptureState.Clear)
                    {
                        FieldSpawner.instance.fieldsSpawned[i].GetComponent<FieldData>().SwitchCaptureState((FieldData.CaptureState)PlayerPrefs.GetInt("fildState" + i));

                        player.CmdSetFieldState(
                            FieldSpawner.instance.fieldsSpawned[i].GetComponent<NetworkIdentity>(),
                            (FieldData.CaptureState)PlayerPrefs.GetInt("fildState" + i)
                        );
                    }
                }
            }
        }

        //Debug.Log("--Loaded Map--" + " Fields: " + fieldsSpawned);

        StartCoroutine(DelyToCanMove());
    }

    IEnumerator DelyToCanMove()
    {
        canSpawn = false;
        yield return new WaitForSeconds(0.5f);
        canSpawn = true;
    }
}
