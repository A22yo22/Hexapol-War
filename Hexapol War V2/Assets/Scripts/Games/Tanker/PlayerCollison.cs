using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollison : MonoBehaviour
{
    public float speed = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Wall"))
        {
            Debug.Log(collision.transform.name);

            Rigidbody rb = GetComponent<Rigidbody>();

            //StartCoroutine(MoveTimeOut());
            //rb.velocity = new Vector3(0, 0, rb.velocity.z);
            //rb.AddForce((transform.position - collision.transform.position).normalized * speed, ForceMode.VelocityChange);
        }
    }

    IEnumerator MoveTimeOut()
    {
        transform.parent.GetComponent<PlayerMovement>().canMove = false;
        yield return new WaitForSeconds(0.1f);
        transform.parent.GetComponent<PlayerMovement>().canMove = true;
    }
}
