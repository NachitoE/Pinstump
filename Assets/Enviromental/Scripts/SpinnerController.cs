using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpinnerController : MonoBehaviour
{
    private int maxPoints;
    private int currGenPoints;
    private BallController ballC;
    public float shootForce;
    private void GenerateRandomPoints()
    {
        maxPoints = Random.Range(3, 10);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball") && ballC == null) //checking for ball to not have been in the spinner recently
        {
            ballC = collision.GetComponent<BallController>();
            ballC.Restart();
            ballC.rb.simulated = false; //disable rigidbody
            GenerateRandomPoints();
            StartCoroutine(SetPoints());
        }

    }

    IEnumerator SetPoints()
    {
        if (currGenPoints < maxPoints)
        {
            currGenPoints += 1;
            GameManager.instance.SumPoints(1);
            AudioManager.instance.PlaySFX(AudioManager.instance.spinner);
            yield return new WaitForSeconds(.2f);
            StartCoroutine(SetPoints()); //keep executing until all points are asigned
        }
        else
        {
            currGenPoints = 0;
            ballC.rb.simulated = true;
            ballC.Throw(Vector2.down, shootForce);
            yield return new WaitForSeconds(1f);
            ballC = null;

        }
    }
        
}

