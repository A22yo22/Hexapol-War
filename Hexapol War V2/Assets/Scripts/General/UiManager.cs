using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    public Button button;

    public GameObject lobbyUi;
    public GameObject mainCam;

    private void Awake()
    {
        instance = this;
    }
}
