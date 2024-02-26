using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperController : MonoBehaviour
{
    [SerializeField] private float pushForce;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<Sprite> sprites;
    private bool isChanging;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            BallController ballC = collision.gameObject.GetComponent<BallController>();
            Vector2 pushDir = ballC.transform.position - transform.position;
            ballC.Restart();
            GameManager.instance.SumPoints(1);
            AudioManager.instance.PlaySFX(AudioManager.instance.bumper);
            ballC.Throw(pushDir, pushForce);

            if (isChanging)
            {
                StopCoroutine(ChangeSprite());
                StartCoroutine(ChangeSprite());
            }else StartCoroutine(ChangeSprite());
        }
    }

    IEnumerator ChangeSprite()
    {
        isChanging = true;
        spriteRenderer.sprite = sprites[1];
        yield return new WaitForSeconds(0.35f);
        spriteRenderer.sprite = sprites[0];
        isChanging = false;  
    }
}
