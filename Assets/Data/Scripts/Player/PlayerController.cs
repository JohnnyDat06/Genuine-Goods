using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRigidbody;
    [SerializeField] private Animator playerAnimator;

    [Header("Movement Settings")]
    private float movementInputDerection;
    private bool isFacingRight = true;
    private bool isWalking;

    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float jumpForce = 16f;

    void Start()
    {
        
    }

    
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }

    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDerection < 0 )
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDerection > 0)
        {
            Flip();
        }

        if(playerRigidbody.velocity.x != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    private void UpdateAnimation()
    {
        playerAnimator.SetBool("isWalking", isWalking);
    }

    private void CheckInput()
    {
        movementInputDerection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    private void Jump()
    {
        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
    }

    private void ApplyMovement()
    {
        playerRigidbody.velocity = new Vector2(movementInputDerection * movementSpeed, playerRigidbody.velocity.y);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}
