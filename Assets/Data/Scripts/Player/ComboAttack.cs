using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboAttack : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerController playerController;
    private int noOfClicks = 0;
    private float lastClikedTime = 0f;
    [HideInInspector] public bool isAttacking;
    [SerializeField] private float maxComboDelay = 0.9f;


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
            if (noOfClicks == 1) anim.SetBool("Attack1", true);
            noOfClicks = Mathf.Clamp(noOfClicks, 0, 3);
        }
    }



    public void Return1()
    {
        if (noOfClicks >= 2)
        {
            anim.SetBool("Attack2", true);
            isAttacking = true;
        }
        else
        {
            anim.SetBool("Attack1", false);
            noOfClicks = 0;
            isAttacking = false;
        }
    }

    public void Return2()
    {
        if (noOfClicks >= 3)
        {
            anim.SetBool("Attack3", true);
            isAttacking = true;
        }
        else
        {
            anim.SetBool("Attack2", false);
            noOfClicks = 0;
            isAttacking = false;
        }
    }
    public void Return3()
    {
         anim.SetBool("Attack1", false);
         anim.SetBool("Attack2", false);
         anim.SetBool("Attack3", false);
         noOfClicks = 0;
         isAttacking = false;
    }

    public void Atk1()
    {
        if(playerController != null) playerController.AttackMoveForward(0.2f, 0.1f);
    }
    public void Atk2()
    {
        if (playerController != null) playerController.AttackMoveForward(0.4f, 0.1f);
    }
    public void Atk3()
    {
        if (playerController != null) playerController.AttackMoveForward(0.6f, 0.1f);
    }
}
