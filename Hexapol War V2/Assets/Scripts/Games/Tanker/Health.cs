using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    public int health = 3;
    public int startHealth = 3;
    public Image healthbar;

    bool dead = false;

    private void Update()
    {
        UpdateHelath();
        //healthbar.text = health.ToString();

        if (!isLocalPlayer) { return; }

        if (health <= 0 && !dead)
        {
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

    void UpdateHelath()
    {
        healthbar.fillAmount = Mathf.Lerp(0, 1, Mathf.InverseLerp(0, startHealth, health));
    }

    public void ResetMinigameHealth()
    {
        health = startHealth;
        dead = false;
    }
}
