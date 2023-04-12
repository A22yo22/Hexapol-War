using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataHolder : MonoBehaviour
{
    public static GameDataHolder instance;

    public List<PlayerInteractions> players;

    void Start()
    {
        if (instance == null) { instance = this; }
    }
}
