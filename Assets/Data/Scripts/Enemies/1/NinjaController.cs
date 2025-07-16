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
        [SerializeField] private float reviveCoolDown = 1.5f;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private Transform startPoint;
        [SerializeField] private Rigidbody2D rb;
    
        private bool isFacingRight = true;
        private bool isReviving = false;
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
            if (isReviving) return;
    
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
        public void BeAttack(float distance, float duration)
        {
            StartCoroutine(SmoothBeAttackMove(distance, duration));
        }

        private IEnumerator SmoothBeAttackMove(float distance, float duration)
        {
            float elapsed = 0f;
            Vector2 startPos = rb.position;
            int direction = isFacingRight ? -1 : 1;
            Vector2 targetPos = startPos + new Vector2(direction *  distance, 0f);

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                rb.MovePosition(Vector2.Lerp(startPos, targetPos, t));
                elapsed += Time.deltaTime;
                yield return null;
            }

            rb.MovePosition(targetPos); // đảm bảo tới đúng vị trí cuối
        }
        public void Revive()
        {
            StartCoroutine(ReviveCooldown());
        }

        private IEnumerator ReviveCooldown()
        {
            isReviving = true;
            yield return new WaitForSeconds(reviveCoolDown);
            isReviving = false;
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
                isFacingRight = true;
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
                isFacingRight = false;
            }
        }

        private void MoveTowards(Vector2 targetPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (targetPosition.x > transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                isFacingRight = true;
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
                isFacingRight = false;
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
