using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSpawner : NetworkBehaviour
{
    public static FieldSpawner instance;

    [Header("Spawn Propertys")]
    public int radius;
    float hexagonSize = 0;

    public List<GameObject> fieldsSpawned = new List<GameObject>();

    [Header("References")]
    public GameObject hexagonPrefab;
    public Transform parent;
    public GameObject indicator;

    private void Start()
    {
        if (instance == null) { instance = this; }
    }

    public void SpawnGrid()
    {
        hexagonSize = hexagonPrefab.GetComponent<Renderer>().bounds.size.x;

        for (int q = -radius; q <= radius; q++)
        {
            for (int r = -radius; r <= radius; r++)
            {
                if (Mathf.Abs(q + r) <= radius)
                {
                    float x = q * (3f / 2f * hexagonSize);
                    float y = (r * (Mathf.Sqrt(3f) / 2f * hexagonSize) + (q / 2f) * (Mathf.Sqrt(3f) / 2f * hexagonSize));
                    Vector3 hexPosition = new Vector3(x / 2, 0, y);

                    GameObject hexagon = Instantiate(hexagonPrefab, hexPosition, Quaternion.Euler(90, 90, 0));
                    hexagon.transform.parent = parent;
                    NetworkServer.Spawn(hexagon);

                    fieldsSpawned.Add(hexagon);

                    foreach (PlayerInteractions player in GameDataHolder.instance.players)
                    {
                        if(player.isOwned) player.CmdAddToList(hexagon.GetComponent<NetworkIdentity>());
                    }

                }
            }
        }
    }

}