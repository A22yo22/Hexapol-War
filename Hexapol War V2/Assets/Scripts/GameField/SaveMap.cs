using Mirror;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SaveMap : NetworkBehaviour
{
    public static SaveMap instance;

    [SyncVar]
    public bool canSpawn = true;

    [SyncVar]
    public int currentSave = 0;

    [SerializeField] GameObject mainMenu;


    private void Start()
    {
        if (instance == null) { instance = this; }
    }

    public void SaveGameMap()
    {
        PlayerPrefs.SetInt("fieldsSpawned" + currentSave, FieldSpawner.instance.fieldsSpawned.Count);

        int blue = 0;
        int red = 0;
        int count = 0;
        foreach (GameObject field in FieldSpawner.instance.fieldsSpawned)
        {
            FieldData fieldData = field.GetComponent<FieldData>();
            PlayerPrefs.SetInt("fildState" + currentSave + count, (int)fieldData.fieldState);

            if (fieldData.fieldState == FieldData.CaptureState.Player1) { blue++; }
            else if (fieldData.fieldState == FieldData.CaptureState.Player2) { red++; }

            count++;
        }

        PlayerPrefs.SetInt("SavedMapBlue" + currentSave, blue);
        PlayerPrefs.SetInt("SavedMapRed" + currentSave, red);


    }

    public void LoadGameMap(int id)
    {
        currentSave = id;

        Debug.Log(id);

        mainMenu.SetActive(false);

        foreach (PlayerInteractions player in GameDataHolder.instance.players)
        {
            player.enabled = true;
        }

        int fieldsSpawned = PlayerPrefs.GetInt("fieldsSpawned" + currentSave);

        foreach (PlayerInteractions player in GameDataHolder.instance.players)
        {
            if (player.isOwned)
            {
                for (int i = 0; i < fieldsSpawned; i++)
                {
                    if ((FieldData.CaptureState)PlayerPrefs.GetInt("fildState" + currentSave + i) != FieldData.CaptureState.Clear)
                    {
                        FieldSpawner.instance.fieldsSpawned[i].GetComponent<FieldData>().SwitchCaptureState((FieldData.CaptureState)PlayerPrefs.GetInt("fildState" + currentSave + i));

                        player.CmdSetFieldState(
                            FieldSpawner.instance.fieldsSpawned[i].GetComponent<NetworkIdentity>(),
                            (FieldData.CaptureState)PlayerPrefs.GetInt("fildState" + currentSave + i)
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
