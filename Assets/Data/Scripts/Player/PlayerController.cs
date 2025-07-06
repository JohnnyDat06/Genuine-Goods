using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRigidbody;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private BoxCollider2D playerCollider;
    [SerializeField] private LayerMask layerGroud;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float jumpForce = 16f;

    private float movementInputDerection;
    private bool isFacingRight = true;

    private enum MovementState { Idle, Walk, JumpStart, JumpEnd}
    void Start()
    {
        
    }

    
    void Update()
    {
        CheckInput();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        ApplyMovement();
    }


    private void UpdateAnimation()
    {
        MovementState state;
        // Move
        if (movementInputDerection > 0)
        {
            state = MovementState.Walk;
            if(!isFacingRight) Flip();
        }
        else if (movementInputDerection < 0)
        {
            state = MovementState.Walk;
            if(isFacingRight) Flip();
        }
        else state = MovementState.Idle;

        // Jump
        if (playerRigidbody.velocity.y > 0.1f) state = MovementState.JumpStart;
        else if (playerRigidbody.velocity.y < -0.1f) state = MovementState.JumpEnd;

        playerAnimator.SetInteger("State", (int)state);
    }

    private void CheckInput()
    {
        movementInputDerection = Input.GetAxisRaw("Horizontal");


        if (Input.GetButtonDown("Jump") && IsGrounded() == true)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (IsGrounded() == true)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
        }           
    }

    private void ApplyMovement()
    {
        playerRigidbody.velocity = new Vector2(movementInputDerection * movementSpeed, playerRigidbody.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(playerCollider.bounds.center, 
            playerCollider.bounds.size, 
            0f, 
            Vector2.down, 
            0.1f, 
            layerGroud
        );
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}
