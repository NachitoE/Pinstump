using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody2D rb;

    public void Throw(Vector2 dir, float force)
    {
        rb.AddForce(dir * force);
    }
    public void Restart()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;
        rb.simulated = false;
        rb.simulated = true;
    }
}
