using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : NetworkBehaviour
{
    public FieldData.CaptureState playerAtMove;

    public PlayerInteractions players1;
    public PlayerInteractions players2;
}
