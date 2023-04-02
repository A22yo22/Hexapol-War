using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float movmentSpeed;
    public Rigidbody rb;

    void Update()
    {
        if (!isLocalPlayer)
        {
            enabled = false;
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        rb.velocity = new Vector3(horizontal * movmentSpeed, rb.velocity.y, vertical * movmentSpeed);
    }
}
