using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRigidbody;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private BoxCollider2D playerCollider;
    [SerializeField] private LayerMask layerGround;

    [SerializeField] private Transform wallCheck;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float wallSlideSpeed;

    private float movementInputDerection;
    private bool isFacingRight = true;
    private bool isTouchingWall;
    private bool isWallSliding;

    private enum MovementState { Idle, Walk, JumpStart, JumpEnd}
    
    void Update()
    {
        CheckInput();
        UpdateAnimation();
        CheckIfWallSliding();
    }

    void FixedUpdate()
    {
        ApplyMovement();
        IsTouchingWall();
    }

    private void CheckIfWallSliding()
    {
        if(isTouchingWall && !IsGrounded() && playerRigidbody.velocity.y < 0)
        {
            isWallSliding = true;       
        }
        else
        {
            isWallSliding = false;
        }
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
        // Wall Sliding
        playerAnimator.SetBool("IsWallSliding", isWallSliding);
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
        // Move the player
        playerRigidbody.velocity = new Vector2(movementInputDerection * movementSpeed, playerRigidbody.velocity.y);

        // Apply wall sliding
        if (isWallSliding)
        {
            if(playerRigidbody.velocity.y < -wallSlideSpeed)
            {
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(playerCollider.bounds.center, 
            playerCollider.bounds.size, 
            0f, 
            Vector2.down, 
            0.1f, 
            layerGround
        );
    }

    private void IsTouchingWall()
    {
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, layerGround);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;      
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }
}
