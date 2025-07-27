using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy4Controller : MonoBehaviour
{
    private enum State { Idle, Chasing, Attacking, Stunned }
    private State currentState;

    [Header("Attack Setting")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackDamage = 30f;
    [SerializeField] private float moveRange = 15f;
    [SerializeField] private float attackCoolDown = 2f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float stunDuration = 2f;

    private Transform player;
    private Animator anim;
    private float cooldownTimer = Mathf.Infinity;
    private float chaseTimer;
    private EnemyHealth enemyHealth;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        currentState = State.Idle;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("ENEMY KHÔNG TÌM THẤY GAMEOBJECT CÓ TAG 'Player'!", this.gameObject);
            this.enabled = false;
        }
    }

    private void Update()
    {
        if (enemyHealth.isDead || enemyHealth.isReviving || currentState == State.Stunned) return;
        if (player == null) return;

        cooldownTimer += Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(player.position, transform.position);

        switch (currentState)
        {
            case State.Idle:
                anim.SetBool("IsMove", false);
                if (distanceToPlayer <= moveRange)
                {
                    currentState = State.Chasing;
                }
                break;

            case State.Chasing:
                anim.SetBool("IsMove", true);
                MoveTowards(player.position);

                if (distanceToPlayer <= attackRange)
                {
                    currentState = State.Attacking;
                }
                else if (distanceToPlayer > moveRange)
                {
                    currentState = State.Idle;
                    anim.SetBool("IsMove", false); // Dòng code fix lỗi
                }
                break;

            case State.Attacking:
                anim.SetBool("IsMove", false);

                if (cooldownTimer >= attackCoolDown)
                {
                    cooldownTimer = 0;
                    Attack();
                }

                if (distanceToPlayer > moveRange)
                {
                    currentState = State.Idle;
                }
                else if (distanceToPlayer > attackRange)
                {
                    currentState = State.Chasing;
                }
                break;

            case State.Stunned:
                rb.velocity = Vector2.zero;
                anim.SetBool("IsMove", false);
                break;
        }
    }

    

    private void Attack()
    {
        anim.SetTrigger("IsAtk");
    }

    private void MoveTowards(Vector2 targetPosition)
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    public void AttackDamage()
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null && pc.isParrying)
        {
            Debug.Log("Bị Parry!");
            StartCoroutine(StunRoutine());
            return;
        }

        if (Vector2.Distance(transform.position, player.position) <= attackRange + 1f)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        }
    }

    private IEnumerator StunRoutine()
    {
        currentState = State.Stunned;
        anim.SetTrigger("IsHit");

        yield return new WaitForSeconds(stunDuration);

        Debug.Log("Hết choáng!");
        currentState = State.Idle;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, moveRange);
    }
}