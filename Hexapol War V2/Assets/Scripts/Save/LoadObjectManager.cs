using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadObjectManager : MonoBehaviour
{
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text playedWithText;

    [SerializeField] TMP_Text scaleText;
    [SerializeField] TMP_Text blueText;
    [SerializeField] TMP_Text redText;

    public void SetUp(string title, string playedWith, int scale, int blue, int red)
    {
        titleText.text = title;
        playedWithText.text = "Played with: \n" + playedWith;

        scaleText.text = "Scale: " + scale;
        blueText.text = "Blue: " + blue;
        redText.text = "Red: " + red;
    }
}
