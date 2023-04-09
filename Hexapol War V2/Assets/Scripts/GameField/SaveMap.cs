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

    //Save Menu Settings
    [SerializeField] Transform loadSaveParent;
    [SerializeField] GameObject savedMapPrefab;


    private void Start()
    {
        if (instance == null) { instance = this; }
    }

    public void CreateMap(string title, string playedWith, int scale, int blue = 1, int red = 1)
    {
        PlayerPrefs.SetInt("SavedMaps", PlayerPrefs.GetInt("SavedMaps") + 1);

        int createdMapId = PlayerPrefs.GetInt("SavedMaps");

        PlayerPrefs.SetString("SavedMapTitle" + createdMapId, title);
        PlayerPrefs.SetString("SavedMapPlayedWith" + createdMapId, playedWith);

        PlayerPrefs.SetInt("SavedMapScale" + createdMapId, scale);
        PlayerPrefs.SetInt("SavedMapBlue" + createdMapId, blue);
        PlayerPrefs.SetInt("SavedMapRed" + createdMapId, red);

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

    public void LoadSavedList()
    {
        //Clear loaded saves
        for (int i = 0; i < loadSaveParent.childCount; i++)
        {
            Destroy(loadSaveParent.GetChild(i).gameObject);
        }

        //Get Saved maps
        int savedMaps = PlayerPrefs.GetInt("SavedMaps");

        //Show all new Saves
        for(int i = 0; i < savedMaps; i++)
        {
            string title = PlayerPrefs.GetString("SavedMapTitle " + i);
            string playedWith = PlayerPrefs.GetString("SavedMapPlayedWith " + i);

            int scale = PlayerPrefs.GetInt("SavedMapScale " + i);
            int blue = PlayerPrefs.GetInt("SavedMapBlue " + i);
            int red = PlayerPrefs.GetInt("SavedMapRed " + i);

            GameObject savedMapObject = Instantiate(savedMapPrefab);
            savedMapObject.transform.SetParent(loadSaveParent);
            savedMapObject.GetComponent<LoadObjectManager>().SetUp(title, playedWith, scale, blue, red);
        }
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
