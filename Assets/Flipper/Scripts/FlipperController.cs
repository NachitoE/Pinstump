using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlipperController : MonoBehaviour
{
    public Rigidbody2D rb;
    public HingeJoint2D hinge;
    private JointMotor2D motor;
    public bool isFlipping;

    

    public float rotationVariation = 10000f;
    public Action OnFlip;
    public Action OnUnflip;
    public bool isLeftFlipper;
    public float flipSpeed;
    private AudioManager audioM;

    private void Start()
    {

        audioM = AudioManager.instance;
        if (isLeftFlipper)
        {
            OnFlip += OnLeftFlip;
            OnUnflip += OnLeftUnflip;
            GameManager.instance.onLeftFlipperPerformed += () => OnFlip?.Invoke();
            GameManager.instance.onLeftFlipperReleased += () => OnUnflip?.Invoke();
            rotationVariation = -rotationVariation;
            flipSpeed = -flipSpeed;
        }
        else
        {
            OnFlip += OnRightFlip;
            OnUnflip += OnRightUnflip;
            GameManager.instance.onRightFlipperPerformed += () => OnFlip?.Invoke();
            GameManager.instance.onRightFlipperReleased += () => OnUnflip?.Invoke();
        }

    }
    private void FixedUpdate()
    {

        

        if (isFlipping)
        {

            rb.AddTorque(-flipSpeed);
        }
        else
        {
            rb.AddTorque(flipSpeed);
        }
    }

    private void OnLeftFlip()
    {
        if (this.CompareTag("LeftFlipper"))
        {
            isFlipping = true;
            audioM.PlaySFX(audioM.flipper);
        }
    }
    private void OnRightFlip()
    {
        if (this.CompareTag("RightFlipper"))
        {
            isFlipping = true;
            audioM.PlaySFX(audioM.flipper);
        }
    }
    private void OnLeftUnflip()
    {
        if (this.CompareTag("LeftFlipper")) isFlipping = false;
    }
    private void OnRightUnflip()
    {
        if (this.CompareTag("RightFlipper")) isFlipping = false;
    }


}
