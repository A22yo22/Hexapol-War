using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTurret : NetworkBehaviour
{
    Transform aimAt;

    [Header("Gun")]
    public float bulletSpeed = 10;

    public LayerMask layerMask;

    public Transform gun;
    public Transform gunSpawnPos;
    public GameObject bullet;

    private void Start()
    {
        aimAt = Instantiate(new GameObject()).transform;
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            Debug.Log(hit.transform.name);
            aimAt.position = hit.point;

            /*
            if (hit.transform.CompareTag("Player"))
            {
                aimAt.position = hit.transform.position;
            }
            if (hit.transform.CompareTag("Enemy"))
            {
                aimAt.position = hit.transform.position;
            }
            */

            gun.LookAt(aimAt);
            gun.eulerAngles = new Vector3(0, gun.eulerAngles.y, gun.eulerAngles.z);
        }

        if (Input.GetMouseButtonDown(0))
        {
            CmdSpawnBullet(gunSpawnPos.forward);
        }
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
        Destroy(x, 3);

        //NetworkServer.Spawn(x);
    }

}
