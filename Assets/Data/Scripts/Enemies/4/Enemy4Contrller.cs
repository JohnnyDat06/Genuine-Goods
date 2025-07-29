using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy4Controller : MonoBehaviour
{
    private enum State { Idle, Chasing, Attacking, Stunned }
    private State currentState;

    [Header("Movement & Detection")]
    [SerializeField] private float moveRange = 15f;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Normal Attack")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackDamage = 30f;
    [SerializeField] private float attackCoolDown = 2f;

    [Header("Lunge Attack")]
    [SerializeField] private float lungeDistance = 3f;
    [SerializeField] private float lungeSpeed = 8f;

    [Header("Stun")]
    [SerializeField] private float stunDuration = 2f;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;

    private Transform player;
    private Animator anim;
    private float cooldownTimer = Mathf.Infinity;
    private EnemyHealth enemyHealth;
    private bool isLunging = false;

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
        if (enemyHealth.isDead || enemyHealth.isReviving || currentState == State.Stunned || isLunging) return;
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
                    anim.SetBool("IsMove", false);
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
        StartCoroutine(LungeRoutine());
    }

    private IEnumerator LungeRoutine()
    {
        isLunging = true;

        float directionToPlayer = player.position.x - transform.position.x;
        Vector2 lungeDirection = new Vector2(Mathf.Sign(directionToPlayer), 0);

        rb.velocity = lungeDirection * lungeSpeed;

        float lungeDuration = lungeDistance / lungeSpeed;
        yield return new WaitForSeconds(lungeDuration);

        rb.velocity = Vector2.zero;
        isLunging = false;
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

        if (Vector2.Distance(transform.position, player.position) <= attackRange + 1.5f) // Tăng vùng check damage một chút để hợp với cú lao
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