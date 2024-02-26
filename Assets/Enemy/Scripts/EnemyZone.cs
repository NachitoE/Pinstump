using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZone : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    private float time;
   // public float timeForFullRotation;

    private void FixedUpdate()
    {
        //float zTargetRot = transform.rotation.z + rotationSpeed * Time.fixedDeltaTime;
        transform.Rotate(0,0,rotationSpeed * Time.fixedDeltaTime);

        time += Time.deltaTime;
        if(transform.eulerAngles.z < 1 && transform.eulerAngles.z > 0 )
        {
            
            if(time > 1) GameManager.instance.EnemyZoneTimeForFullRotation = time; //making sure to not make timeForFullRotation = 0
            time = 0;
        }
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("hola");
            EnemyController enemyC = collision.GetComponent<EnemyController>();
            enemyC.attachedToZone = true;
            enemyC.transform.parent = transform;
        }
    }


}
