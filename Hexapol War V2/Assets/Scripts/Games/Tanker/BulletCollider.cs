using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class BulletCollider : MonoBehaviour
{
    public int damage = 1;

    public TankTurret tankTurret;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);

        if (other.CompareTag("Enemy"))
        {
            tankTurret.TakeDamage(damage, other.transform.parent.GetComponent<NetworkIdentity>());
            Destroy(gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            tankTurret.TakeDamage(damage, other.transform.parent.GetComponent<NetworkIdentity>());
            Destroy(gameObject);
        }
    }
}
