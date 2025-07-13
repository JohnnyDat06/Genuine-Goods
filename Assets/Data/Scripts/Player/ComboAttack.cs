using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboAttack : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerController playerController;
    private int noOfClicks = 0;
    private float lastClikedTime = 0f;
    public bool isAttacking;
    [SerializeField] private float maxComboDelay = 0.9f;

    [SerializeField] private float pushForce = 2f;
    [SerializeField] private Transform moveTarget;

    private void Awake()
    {
        moveTarget = this.transform;
    }

    void Update()
    {
        if (Time.time - lastClikedTime > maxComboDelay)
        {
            noOfClicks = 0;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            lastClikedTime = Time.time;
            noOfClicks++;
            isAttacking = true;
            playerController.canMove = false;
            if (noOfClicks == 1) anim.SetBool("Attack1", true);
            noOfClicks = Mathf.Clamp(noOfClicks, 0, 3);
        }
    }

    public void PushForward()
    {
        if (moveTarget != null)
        {
            Vector3 forward = transform.forward;
            moveTarget.position += forward * pushForce * Time.deltaTime;
        }
    }

    public void Return1()
    {
        if (noOfClicks >= 2)
        {
            anim.SetBool("Attack2", true);
            playerController.canMove = false;
            isAttacking = true;
        }
        else
        {
            anim.SetBool("Attack1", false);
            noOfClicks = 0;
            isAttacking = false;
            playerController.canMove = true;
        }
    }

    public void Return2()
    {
        if (noOfClicks >= 3)
        {
            anim.SetBool("Attack3", true);
            isAttacking = true;
            playerController.canMove = false;
        }
        else
        {
            anim.SetBool("Attack2", false);
            noOfClicks = 0;
            isAttacking = false;
            playerController.canMove = true;
        }
    }
    public void Return3()
    {
         anim.SetBool("Attack1", false);
         anim.SetBool("Attack2", false);
         anim.SetBool("Attack3", false);
         noOfClicks = 0;
         isAttacking = false;
         playerController.canMove = true;
    }
}
