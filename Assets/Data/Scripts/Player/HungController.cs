using UnityEngine;

public class HungController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRigidbody;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float walkSpeed = 3f;

    private float movementInput;
    private bool isFacingRight = true;

    private enum MovementState { Idle, Walk }

    void Update()
    {
        movementInput = Input.GetAxisRaw("Horizontal");
        UpdateAnimation();
        HandleFlip();
    }

    void FixedUpdate()
    {
        playerRigidbody.velocity = new Vector2(movementInput * walkSpeed, playerRigidbody.velocity.y);
    }

    private void UpdateAnimation()
    {
        MovementState state = (movementInput != 0) ? MovementState.Walk : MovementState.Idle;
        playerAnimator.SetInteger("State", (int)state);
    }

    private void HandleFlip()
    {
        if (movementInput > 0 && !isFacingRight)
            Flip();
        else if (movementInput < 0 && isFacingRight)
            Flip();
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
}