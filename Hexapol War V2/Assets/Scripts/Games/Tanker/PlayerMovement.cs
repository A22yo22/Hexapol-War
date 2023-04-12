using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movmentSpeed;
    public bool canMove;
    public Rigidbody rb;

    void Update()
    {
        if (canMove)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            rb.velocity = new Vector3(horizontal * movmentSpeed, rb.velocity.y, vertical * movmentSpeed);
        }
    }
}
