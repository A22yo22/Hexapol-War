using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactions : NetworkBehaviour
{
    void Start()
    {
        if (isLocalPlayer)
        {
            UiManager.instance.button.onClick.AddListener(ReadyUp);
        }
    }



    public void ReadyUp()
    {
        CmdReadyUp();
    }

    [Command]
    public void CmdReadyUp()
    {
        FindObjectOfType<LobbyManager>().ReadyUp();
    }
}
