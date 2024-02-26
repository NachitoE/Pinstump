using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Vector3 targetPosition;
    public bool attachedToZone;
    public float speed;

    void Start()
    {
        attachedToZone = false;
        targetPosition = GameObject.FindGameObjectWithTag("EnemyZone").transform.position;
    }

    private void FixedUpdate()
    {
        if (targetPosition != null && !attachedToZone)
        {
           transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            GameManager.instance.SumPoints(3);
            AudioManager.instance.PlaySFX(AudioManager.instance.enemyDead);
            Destroy(gameObject);
        }
    }


}
