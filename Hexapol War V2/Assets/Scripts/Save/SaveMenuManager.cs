using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SaveMenuManager : MonoBehaviour
{
    //Save Menu Settings
    [SerializeField] Transform loadSaveParent;
    [SerializeField] GameObject savedMapPrefab;

    [SerializeField] TMP_InputField title;
    [SerializeField] TMP_InputField scale;
    [SerializeField] TMP_InputField startingFields;


    private void Start()
    {
        LoadSavedList();
    }

    public void CreateMap()
    {
        PlayerPrefs.SetInt("SavedMaps", PlayerPrefs.GetInt("SavedMaps") + 1);

        int createdMapId = PlayerPrefs.GetInt("SavedMaps");

        PlayerPrefs.SetString("SavedMapTitle" + createdMapId, title.text);
        PlayerPrefs.SetString("SavedMapPlayedWith" + createdMapId, "Player 2");

        PlayerPrefs.SetInt("SavedMapScale" + createdMapId, int.Parse(scale.text));
        PlayerPrefs.SetInt("SavedMapBlue" + createdMapId, int.Parse(startingFields.text));
        PlayerPrefs.SetInt("SavedMapRed" + createdMapId, int.Parse(startingFields.text));

        FieldSpawner.instance.radius = int.Parse(scale.text);
        FieldSpawner.instance.SpawnGrid();

        StartCoroutine(SetStartingFields(int.Parse(startingFields.text)));
    }

    IEnumerator SetStartingFields(int fields)
    {
        yield return new WaitForSeconds(0.2f);

        Debug.Log("Set Fields");

        foreach (PlayerInteractions player in FindObjectsOfType<PlayerInteractions>())
        {
            if (player.isOwned)
            {
                int selectedField = Random.Range(0, FieldSpawner.instance.fieldsSpawned.Count);

                for (int i = 0; i < fields; i++)
                {
                    Debug.Log("Player 1");

                    while (FieldSpawner.instance.fieldsSpawned[selectedField].GetComponent<FieldData>().fieldState != FieldData.CaptureState.Clear)
                    {
                        selectedField = Random.Range(0, FieldSpawner.instance.fieldsSpawned.Count);
                    }

                    FieldSpawner.instance.fieldsSpawned[selectedField].GetComponent<FieldData>().SwitchCaptureState(FieldData.CaptureState.Player1);
                    player.CmdSetFieldState(FieldSpawner.instance.fieldsSpawned[selectedField].GetComponent<NetworkIdentity>(), FieldData.CaptureState.Player1);
                    player.lastSelectedField = FieldSpawner.instance.fieldsSpawned[selectedField];
                }


                for (int i = 0; i < fields; i++)
                {
                    Debug.Log("Player 2");

                    while (FieldSpawner.instance.fieldsSpawned[selectedField].GetComponent<FieldData>().fieldState != FieldData.CaptureState.Clear)
                    {
                        selectedField = Random.Range(0, FieldSpawner.instance.fieldsSpawned.Count);
                    }

                    FieldSpawner.instance.fieldsSpawned[selectedField].GetComponent<FieldData>().SwitchCaptureState(FieldData.CaptureState.Player2);
                    player.CmdSetFieldState(FieldSpawner.instance.fieldsSpawned[selectedField].GetComponent<NetworkIdentity>(), FieldData.CaptureState.Player2);
                    
                }
            }
        }
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
        //Debug.Log(savedMaps);

        //Show all new Saves
        for (int i = 1; i <= savedMaps; i++)
        {
            string title = PlayerPrefs.GetString("SavedMapTitle" + i);
            string playedWith = PlayerPrefs.GetString("SavedMapPlayedWith" + i);

            int scale = PlayerPrefs.GetInt("SavedMapScale" + i);
            int blue = PlayerPrefs.GetInt("SavedMapBlue" + i);
            int red = PlayerPrefs.GetInt("SavedMapRed" + i);

            GameObject savedMapObject = Instantiate(savedMapPrefab);
            savedMapObject.transform.SetParent(loadSaveParent);
            savedMapObject.GetComponent<LoadObjectManager>().SetUp(title, playedWith, scale, blue, red, i);
        }
    }
}
