using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public int health = 3;
    public int startHealth = 3;

    bool dead = false;

    private void Update()
    {
        if (!isLocalPlayer) { return; }

        if (health <= 0 && !dead)
        {
            Debug.Log("Lost");

            foreach (PlayerInteractions player in FindObjectsOfType<PlayerInteractions>())
            {
                if (player.isOwned)
                {
                    FieldData.CaptureState winner = Checks.GetOppositeOfPlayerTag(GetComponent<PlayerInteractions>().thisPlayerTag);


                    player.CmdSetFieldState(FindObjectOfType<MinigameManager>().attackingPlayer.GetComponent<NetworkIdentity>(), winner);
                    player.CmdSetFieldState(FindObjectOfType<MinigameManager>().fieldToPlayAbout.GetComponent<NetworkIdentity>(), winner);
                }
            }
            dead = true;
        }
    }

    public void ResetMinigameHealth()
    {
        health = startHealth;
        dead = false;
    }
}
