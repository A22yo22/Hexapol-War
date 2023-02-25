using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    public List<GameObject> remainingFields;


    public void RefreshRemainingFields()
    {
        remainingFields.Clear();

        FieldData.CaptureState thisPlayerTag = GetComponent<PlayerInteractions>().thisPlayerTag;
        if (thisPlayerTag != FieldData.CaptureState.Clear)
        {
            foreach (var field in FindObjectOfType<FieldSpawner>().fieldsSpawned)
            {
                if (field.GetComponent<FieldData>().fieldState == thisPlayerTag)
                {
                    remainingFields.Add(field);
                }
            }
        }
    }
}
