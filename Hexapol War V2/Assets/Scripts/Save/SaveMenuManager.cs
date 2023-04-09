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
        PlayerPrefs.SetInt("SavedMapBlue" + createdMapId, 1);
        PlayerPrefs.SetInt("SavedMapRed" + createdMapId, 1);

        FieldSpawner.instance.radius = int.Parse(scale.text);
        FieldSpawner.instance.SpawnGrid();

        //Debug.Log(title + " " + createdMapId);
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

            //Debug.Log(title + " " + i);

            GameObject savedMapObject = Instantiate(savedMapPrefab);
            savedMapObject.transform.SetParent(loadSaveParent);
            savedMapObject.GetComponent<LoadObjectManager>().SetUp(title, playedWith, scale, blue, red);
        }
    }
}
