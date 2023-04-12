using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TankTurret : NetworkBehaviour
{
    Transform aimAt;

    [Header("Gun")]
    public float bulletSpeed = 10;

    public LayerMask layerMask;

    public Transform gun;
    public Transform gunSpawnPos;
    public GameObject bullet;

    public float shootDelay = 0.1f;
    public bool canShoot = true;

    private void Start()
    {
        aimAt = Instantiate(new GameObject()).transform;

        if (!isLocalPlayer)
        {
            gun.parent.gameObject.layer = 8;
            gun.parent.gameObject.tag = "Enemy";
        }
        else
        {
            gun.parent.gameObject.layer = 9;
            gun.parent.gameObject.tag = "Player";
        }
    }

    private void Update()
    {
        //Check if is local player
        if (!isLocalPlayer) { return; }

        //Camera.main.transform.LookAt(transform.Find("Player"));

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            aimAt.position = hit.point;

            gun.LookAt(aimAt);
            gun.eulerAngles = new Vector3(0, gun.eulerAngles.y, gun.eulerAngles.z);
        }

        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            CmdSpawnBullet(gunSpawnPos.forward);
            StartCoroutine(StartShootDelayCounter());
        }
    }

    IEnumerator StartShootDelayCounter()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootDelay);
        canShoot = true;
    }

    [Command]
    void CmdSpawnBullet(Vector3 dir)
    {
        RpcSpawnBullet(dir);
    }

    [ClientRpc]
    void RpcSpawnBullet(Vector3 dir)
    {
        GameObject x = Instantiate(bullet, gunSpawnPos.position, Quaternion.identity);
        x.GetComponent<Rigidbody>().AddForce(dir * bulletSpeed, ForceMode.Impulse);
        x.GetComponent<BulletCollider>().tankTurret = this;
        Destroy(x, 5);
    }


    //deal damage to enemy
    public void TakeDamage(int damage, NetworkIdentity id)
    {
        CmdTakeDamage(damage, id);
    }

    [Command]
    public void CmdTakeDamage(int damage, NetworkIdentity id)
    {
        RpcTakeDamage(damage, id);
    }

    [ClientRpc]
    public void RpcTakeDamage(int damage, NetworkIdentity id)
    {
        id.GetComponent<Health>().health -= damage;
        id.GetComponent<Health>().UpdateHealth();

        if (id.GetComponent<Health>().health <= 0)
        {
            if (!transform.CompareTag("Enemy"))
            {
                MiniGameManager.instance.GameOver();
            }
        }
    }
}
