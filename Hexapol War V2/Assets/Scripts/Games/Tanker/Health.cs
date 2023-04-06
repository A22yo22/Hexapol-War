using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public int health = 3;
    public int startHealth = 3;
    public TMP_Text healthbar;

    bool dead = false;

    private void Update()
    {
        healthbar.text = health.ToString();

        if (!isLocalPlayer) { return; }

        if (health <= 0 && !dead)
        {
            Debug.Log("Lost");

            foreach (PlayerInteractions player in FindObjectsOfType<PlayerInteractions>())
            {
                if (player.isOwned)
                {
                    FieldData.CaptureState winner = Checks.GetOppositeOfPlayerTag(GetComponent<PlayerInteractions>().thisPlayerTag);


                    player.CmdSetFieldState(MinigameManager.instance.attackingPlayer.GetComponent<NetworkIdentity>(), winner);
                    player.CmdSetFieldState(MinigameManager.instance.fieldToPlayAbout.GetComponent<NetworkIdentity>(), winner);


                    SaveMap.instance.SaveGameMap();
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
