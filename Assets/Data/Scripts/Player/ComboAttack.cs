using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboAttack : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerController playerController;

    private List<EnemyHealth> allEnemies = new List<EnemyHealth>();
    [SerializeField] private LayerMask attackMask;
    [SerializeField] private Vector2 attackOffset;

    private int noOfClicks = 0;
    private float lastClikedTime = 0f;
    [HideInInspector] public bool isAttacking;

    [Header("Attack Setting")]
    [SerializeField] private int atk1 = 20;
    [SerializeField] private int atk2 = 30;
    [SerializeField] private int atk3 = 50;
    [SerializeField] private float attackRange = 10f;
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

    private void UpdateEnemyList()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        allEnemies.Clear();

        foreach (GameObject enemy in enemies)
        {
            EnemyHealth eh = enemy.GetComponentInChildren<EnemyHealth>();
            if (eh != null)
            {
                allEnemies.Add(eh);
            }
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
            ResetAllAnimations();
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
            ResetAllAnimations();
        }
    }

    public void Return3()
    {
        ResetAllAnimations();
    }

    private void ResetAllAnimations()
    {
        anim.SetBool("Attack1", false);
        anim.SetBool("Attack2", false);
        anim.SetBool("Attack3", false);
        noOfClicks = 0;
        isAttacking = false;
    }

    private void Attack(int damage)
    {
        UpdateEnemyList();

        Vector3 pos = transform.position + transform.right * attackOffset.x + transform.up * attackOffset.y;
        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);

        if (colInfo != null)
        {
            foreach (EnemyHealth eh in allEnemies.ToArray())
            {
                if (eh == null)
                {
                    allEnemies.Remove(eh);
                    continue;
                }

                if (colInfo.transform.IsChildOf(eh.transform) || colInfo.transform == eh.transform)
                {
                    eh.TakeDamage(damage);
                    break;
                }
            }
        }
    }

    public void Atk1()
    {
        if (playerController != null) playerController.AttackMoveForward(0.2f, 0.1f);
        Attack(atk1);
    }

    public void Atk2()
    {
        if (playerController != null) playerController.AttackMoveForward(0.4f, 0.1f);
        Attack(atk2);
    }

    public void Atk3()
    {
        if (playerController != null) playerController.AttackMoveForward(0.6f, 0.1f);
        Attack(atk3);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position + transform.right * attackOffset.x + transform.up * attackOffset.y;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, attackRange);
    }
}
