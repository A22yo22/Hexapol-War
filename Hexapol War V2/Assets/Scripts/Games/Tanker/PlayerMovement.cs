using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float movmentSpeed;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        //Application.targetFrameRate = 60;
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            //return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical);
        rb.velocity = direction * movmentSpeed;

    }
}
