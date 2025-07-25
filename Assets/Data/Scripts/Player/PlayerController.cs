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
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float movementForceInAir;

    [Header("Wall Sliding Settings")]
    [SerializeField] private float wallHopForce;
    [SerializeField] private float wallJumpForce;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private Vector2 wallHopDirection;
    [SerializeField] private Vector2 wallJumpDirection;

    [Header("Combat setting")]
    [SerializeField] private ComboAttack comboAttack;
    [SerializeField] private float parryDuration = 0.5f;
    public bool isParrying { get; private set; }

    private int facingDirection = 1;
    private float movementInputDerection;

    private bool isRunning;
    private bool isFacingRight = true;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isWallJump = false;
    public bool canMove = true;


    private enum MovementState { Idle, Walk, JumpStart, JumpEnd, Run}

    void Start()
    {
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    void Update()
    {
        CheckInput();
        UpdateAnimation();
        CheckIfWallSliding();
        CheckAttack();
    }

    void FixedUpdate()
    {
        ApplyMovement();
        IsTouchingWall();
    }
    private IEnumerator ParryCoroutine()
    {
        isParrying = true;
        canMove = false;
        playerRigidbody.velocity = new Vector2(0f, playerRigidbody.velocity.y);
        playerAnimator.SetTrigger("IsParry");
        yield return new WaitForSeconds(parryDuration);
        isParrying = false;
        canMove = true;
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
            isWallJump = false;
        }
    }

    private void UpdateAnimation()
    {
        MovementState state;
        // Move
        if (movementInputDerection > 0 && !isRunning)
        {
            state = MovementState.Walk;
            if(!isFacingRight) Flip();
        }
        else if (movementInputDerection < 0 && !isRunning)
        {
            state = MovementState.Walk;
            if(isFacingRight) Flip();
        }
        // Running
        else if (movementInputDerection > 0 && isRunning)
        {
            state = MovementState.Run;
            if (!isFacingRight) Flip();
        }
        else if (movementInputDerection < 0 && isRunning)
        {
            state = MovementState.Run;
            if (isFacingRight) Flip();
        }
        else state = MovementState.Idle;

        // Jump
        if (playerRigidbody.velocity.y > 0.1f) state = MovementState.JumpStart;
        else if (playerRigidbody.velocity.y < -0.1f) state = MovementState.JumpEnd;

        playerAnimator.SetInteger("State", (int)state);
        // Wall Sliding
        playerAnimator.SetBool("IsWallSliding", isWallSliding);
        // Wall Jump
        playerAnimator.SetBool("IsWallJump", isWallJump);
    }

    private void CheckInput()
    {
        // Movement input
        movementInputDerection = Input.GetAxisRaw("Horizontal");

        // Jump input
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (Input.GetButtonUp("Jump"))
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.y * jumpHeight);
        }

        // Run input
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true;
            movementSpeed = runSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
            movementSpeed = 3f; 
        }
        
        if (Input.GetKeyDown(KeyCode.K) && !isParrying)
        {
            StartCoroutine(ParryCoroutine());
        }

    }

    private void Jump()
    {
        if (IsGrounded() == true && !isWallSliding)
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
        }
        else if ((isWallSliding || isTouchingWall) && movementInputDerection != 0) // Wall jump
        {
            isWallJump = true;
            isWallSliding = false;
            Vector2 forceToAdd = new Vector2(wallJumpDirection.x * wallJumpForce * movementInputDerection, wallJumpDirection.y * wallJumpForce);
            playerRigidbody.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
    }

    private void CheckAttack()
    {
        if (comboAttack.isAttacking && comboAttack != null)
        {
            canMove = false;
            playerRigidbody.velocity = new Vector2(0f, playerRigidbody.velocity.y);
        }
        else
        {
            canMove = true;
        }
    }

    public void AttackMoveForward(float distance, float duration)
    {
        StartCoroutine(SmoothAttackMove(distance, duration));
    }

    private IEnumerator SmoothAttackMove(float distance, float duration)
    {
        float elapsed = 0f;
        Vector2 startPos = playerRigidbody.position;
        int direction = isFacingRight ? 1 : -1;
        Vector2 targetPos = startPos + new Vector2(direction * distance, 0f);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            playerRigidbody.MovePosition(Vector2.Lerp(startPos, targetPos, t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerRigidbody.MovePosition(targetPos); // đảm bảo tới đúng vị trí cuối
    }


    private void ApplyMovement()
    {
        // Move the player
        if(IsGrounded() && canMove)
            playerRigidbody.velocity = new Vector2(movementInputDerection * movementSpeed, playerRigidbody.velocity.y);
        else if (!IsGrounded() && !isWallSliding && movementInputDerection != 0)
        {
            Vector2 forceToAdd = new Vector2(movementInputDerection * movementForceInAir, 0f);
            playerRigidbody.AddForce(forceToAdd, ForceMode2D.Force);

            if (Mathf.Abs(playerRigidbody.velocity.x) > movementSpeed)
            {
                playerRigidbody.velocity = new Vector2(movementInputDerection * movementSpeed, playerRigidbody.velocity.y);
            }
        }

        // Apply wall sliding
        if (isWallSliding)
        {
            if (playerRigidbody.velocity.y < -wallSlideSpeed)
            {
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, -wallSlideSpeed);
            }
        }       
    }

    public void TriggerDeathAnimation()
    {
        canMove = false;
        playerAnimator.SetTrigger("IsDeath");
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.isKinematic = true;
    }
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.down, 0.1f,layerGround);
    }

    private void IsTouchingWall()
    {
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, layerGround);
    }

    private void Flip()
    {
        if (!isWallSliding)
        {
            facingDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;      
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        
        if (isParrying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, new Vector3(1f, 1.5f, 0.1f));
        }

    }
}
