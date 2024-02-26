using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlungerController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float maxReleaseForce;
    [SerializeField] private float changingSpeed;
    private float currForce;
    private bool isCharging;
    private bool hasReachedMaxForce;

    //ball related
    private BallController ballC;

    private void Start()
    {
        currForce = 0;
        GameManager.instance.onPlungerPerformed += CalculateCurrForce;
        GameManager.instance.onPlungerReleased += ShootBall;

    }
    private void FixedUpdate()
    {
        currForce = Mathf.Clamp(currForce, 0, maxReleaseForce);
        //if (currForce <= 0) spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        float GBComponent = 1f - (currForce / maxReleaseForce);
        float RComponent = 1f;
        if (currForce > 0) spriteRenderer.color = new Color(RComponent, GBComponent, GBComponent, 1f);
        else spriteRenderer.color = new Color(1f, 1f , 1f,  1f);

        if (isCharging)
        {
            if (!hasReachedMaxForce)
            {
                currForce = (currForce + Time.deltaTime * changingSpeed + Time.deltaTime * currForce * 5f );
                if (currForce >= maxReleaseForce)
                {
                    hasReachedMaxForce = true;
                }
            }
            else
            {
                currForce = (currForce - Time.deltaTime * changingSpeed - Time.deltaTime * currForce);
                if (currForce <= 0)
                {
                    hasReachedMaxForce = false;
                }
            }


        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            ballC = collision.gameObject.GetComponent<BallController>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    { 
        if (collision.gameObject.CompareTag("Ball"))
        {
            ballC = null;
        }
    }

    private void CalculateCurrForce()
    {
        isCharging = true;
    }

    private void ShootBall()
    {
        if(ballC != null)
        {
            ballC.Throw(Vector2.up, currForce);
            AudioManager.instance.PlaySFX(AudioManager.instance.plunger);
            
        }
        isCharging = false;
        currForce = 0;
    }
}
