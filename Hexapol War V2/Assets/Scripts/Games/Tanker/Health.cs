using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    [SyncVar]
    public int health = 3;

    public int startHealth = 3;

    [SyncVar]
    public int actualHealth;

    public Image healthbar;

    bool dead = false;

    private void Update()
    {
        if (!isLocalPlayer) { return; }


        if (health <= 0 && !dead)
        {
            foreach (PlayerInteractions player in FindObjectsOfType<PlayerInteractions>())
            {
                if (player.isOwned)
                {
                    FieldData.CaptureState winner = Checks.GetOppositeOfPlayerTag(GetComponent<PlayerInteractions>().thisPlayerTag);


                    foreach (FieldData field in MiniGameManager.instance.attackingPlayers)
                    {
                        field.SwitchCaptureState(winner);
                        player.CmdSetFieldState(field.GetComponent<NetworkIdentity>(), winner);
                    }

                    MiniGameManager.instance.fieldToPlayAbout.GetComponent<FieldData>().SwitchCaptureState(winner);
                    player.CmdSetFieldState(MiniGameManager.instance.fieldToPlayAbout.GetComponent<NetworkIdentity>(), winner);

                    SaveMap.instance.SaveGameMap();
                }
            }
            dead = true;
        }
    }

    public void UpdateHealth()
    {
        healthbar.fillAmount = Mathf.Lerp(0, 1, Mathf.InverseLerp(0, actualHealth, health));
    }

    public void ResetMinigameHealth(int attackers)
    {
        if (attackers != 0)
        {
            Debug.Log("Option 1 " + attackers);

            health = startHealth * attackers;
            actualHealth = health;
        }
        else
        {
            Debug.Log("Option 2 " + attackers);

            health = startHealth;
            actualHealth = startHealth;
        }

        UpdateHealth();

        dead = false;
    }
}
