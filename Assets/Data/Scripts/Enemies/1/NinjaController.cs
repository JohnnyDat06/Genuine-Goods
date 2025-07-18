using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaController : MonoBehaviour
{
    private enum State { Idle, Chasing, Attacking, Returning }
        private State currentState;
    
        [Header("Attack Setting")]
        [SerializeField] private float attackRange = 10f;
        [SerializeField] private float moveRange = 15f;
        [SerializeField] private float attackCoolDown = 2f;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private Transform startPoint;
        [SerializeField] private Rigidbody2D rb;
        
        private Transform player;
        private Animator anim;
        private float cooldownTimer = Mathf.Infinity;
        private EnemyHealth enemyHealth;
    
        private void Awake()
        {
            anim = GetComponent<Animator>();
            enemyHealth = GetComponent<EnemyHealth>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            currentState = State.Idle;
        }
    
        private void Update()
        {
            if (enemyHealth.currentHealth <= 0) return;
            if (player == null) return;
            if (enemyHealth.isReviving) return;
    
            cooldownTimer += Time.deltaTime;
            
            switch (currentState)
            {
                case State.Idle:
                    anim.SetBool("IsMove", false);
                    if (Vector2.Distance(player.position, transform.position) <= moveRange)
                    {
                        currentState = State.Chasing;
                    }
                    else
                    {
                        currentState = State.Returning;
                    }
                    break;
    
                case State.Chasing:
                    anim.SetBool("IsMove", true);
                    MoveTowards(player.position);
    
                    if (Vector2.Distance(player.position, transform.position) <= attackRange)
                    {
                        currentState = State.Attacking;
                    }
                    else if (Vector2.Distance(player.position, transform.position) > moveRange)
                    {
                        currentState = State.Returning;
                    }
                    break;
    
                case State.Attacking:
                    anim.SetBool("IsMove", false);
                    FacePlayer();
    
                    if (cooldownTimer >= attackCoolDown)
                    {
                        cooldownTimer = 0;
                        Attack();
                    }
    
                    if (Vector2.Distance(player.position, transform.position) > attackRange)
                    {
                        currentState = State.Chasing;
                    }
                    break;
    
                case State.Returning:
                    float distToStart = Vector2.Distance(transform.position, startPoint.position);
    
                    if (distToStart < 0.1f)
                    {
                        currentState = State.Idle;
                    }
                    else
                    {
                        anim.SetBool("IsMove", true);
                        MoveTowards(startPoint.position);
                    }
                    if (Vector2.Distance(player.position, transform.position) <= moveRange)
                    {
                        currentState = State.Chasing;
                    }
                    break;
            }
        }
        

        private void Attack()
        {
            anim.SetTrigger("IsAtk");
        }

        public void AttackDamage()
        {
            player.GetComponent<PlayerHealth>().TakeDamage(30);
        }
        private void FacePlayer()
        {
            if (player.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }

        private void MoveTowards(Vector2 targetPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (targetPosition.x > transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, moveRange);
        }
}
