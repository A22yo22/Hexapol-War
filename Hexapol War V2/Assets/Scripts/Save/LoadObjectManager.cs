using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoadObjectManager : MonoBehaviour
{
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text playedWithText;

    [SerializeField] TMP_Text scaleText;
    [SerializeField] TMP_Text blueText;
    [SerializeField] TMP_Text redText;

    public int id;
    int scale;

    public Button press;

    public void SetUp(string title, string playedWith, int scaleThis, int blue, int red, int idThis)
    {
        titleText.text = title;
        playedWithText.text = "Played with: \n" + playedWith;

        scaleText.text = "Scale: " + scale;     scale = scaleThis;
        blueText.text = "Blue: " + blue;
        redText.text = "Red: " + red;

        idThis++;

        press.onClick.AddListener(Clicked);
    }
    public void Clicked()
    {
        FieldSpawner.instance.radius = scale;
        FieldSpawner.instance.SpawnGrid();

        SaveMap.instance.LoadGameMap(id);
    }
}
