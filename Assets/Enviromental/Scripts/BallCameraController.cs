using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCameraController : MonoBehaviour
{
    public GameObject ball;

    private void FixedUpdate()
    {
        transform.position = ball.transform.position + Vector3.forward * -1;
    }
}
