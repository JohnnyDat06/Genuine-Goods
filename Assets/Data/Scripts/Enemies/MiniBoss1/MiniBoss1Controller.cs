using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss1Controller : MonoBehaviour
{
    private enum State { Idle, Chasing, Attacking, Returning, Skill2 }
    private State currentState;

    [Header("Attack Setting")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float attackDamage = 40f;
    [SerializeField] private float moveRange = 20f;
    [SerializeField] private float attackCoolDown = 2f;
    [SerializeField] private float moveSpeed = 2.5f;

    [Header("Skill 2 Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    
    private List<GameObject> spawnedMinions = new List<GameObject>();
    private bool isWaitingForMinions = false;

    [SerializeField] private Transform startPoint;
    [SerializeField] private Rigidbody2D rb;

    private Transform player;
    private Animator anim;
    private float cooldownTimer = Mathf.Infinity;
    private float skill2Timer = Mathf.Infinity;
    private float chaseTimer;

    [SerializeField] private EnemyHealth enemyHealth;
    private bool skill2UsedAt50 = false;
    private bool skill2UsedAt25 = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = State.Idle;
    }

    private void Update()
    {
        if (enemyHealth.currentHealth <= 0 || enemyHealth.isReviving || player == null) return;
        if (isWaitingForMinions)
        {
            // Nếu còn minion sống thì đứng yên
            spawnedMinions.RemoveAll(m => m == null); // xoá mấy con đã chết

            if (spawnedMinions.Count == 0)
            {
                isWaitingForMinions = false;
                currentState = State.Chasing;
                gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
            else
            {
                anim.SetBool("IsMove", false); // đứng yên
                gameObject.layer = LayerMask.NameToLayer("Default");
                return;
            }
        }

        cooldownTimer += Time.deltaTime;
        skill2Timer += Time.deltaTime;

        float healthPercent = enemyHealth.currentHealth / enemyHealth.startingHealth;

        // Kiểm tra điều kiện kích hoạt kỹ năng 2
        if (!skill2UsedAt50 && healthPercent <= 0.5f)
        {
            skill2UsedAt50 = true;
            anim.SetBool("IsMove", true);
            StartCoroutine(UseSkill2());
            return;
        }
        if (!skill2UsedAt25 && healthPercent <= 0.25f)
        {
            skill2UsedAt25 = true;
            anim.SetBool("IsMove", true);
            StartCoroutine(UseSkill2());
            return;
        }

        switch (currentState)
        {
            case State.Idle:
                anim.SetBool("IsMove", false);
                if (Vector2.Distance(player.position, transform.position) <= moveRange)
                    currentState = State.Chasing;
                else
                    currentState = State.Returning;
                break;

            case State.Chasing:
                anim.SetBool("IsMove", true);
                MoveTowards(player.position);

                if (Vector2.Distance(player.position, transform.position) <= attackRange)
                    currentState = State.Attacking;
                else if (Vector2.Distance(player.position, transform.position) > moveRange)
                    currentState = State.Returning;
                break;

            case State.Attacking:
                anim.SetBool("IsMove", false);
                FacePlayer();

                if (cooldownTimer >= attackCoolDown)
                {
                    cooldownTimer = 0;
                    anim.SetTrigger("IsAtk1");
                }

                if (Vector2.Distance(player.position, transform.position) > attackRange)
                {
                    chaseTimer += Time.deltaTime;
                    if (chaseTimer > 1f)
                        currentState = State.Chasing;
                }
                else
                {
                    chaseTimer = 0;
                }
                break;

            case State.Returning:
                if (Vector2.Distance(transform.position, startPoint.position) < 0.1f)
                {
                    currentState = State.Idle;
                }
                else
                {
                    anim.SetBool("IsMove", true);
                    MoveTowards(startPoint.position);
                }

                if (Vector2.Distance(player.position, transform.position) <= moveRange)
                    currentState = State.Chasing;
                break;
        }
    }

    public void AttackDamage()
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null && pc.isParrying)
        {
            enemyHealth.BeAttack(0.5f, 0.1f);
            anim.SetTrigger("IsHit");
            cooldownTimer = -0.5f;
            return;
        }

        player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
    }

    private void FacePlayer()
    {
        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    private void MoveTowards(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (target.x > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    private IEnumerator UseSkill2()
    {
        currentState = State.Skill2;

        // Quay về startPoint
        while (Vector2.Distance(transform.position, startPoint.position) > 0.1f)
        {
            MoveTowards(startPoint.position);
            yield return null;
        }
        anim.SetBool("IsMove", false);
        FacePlayer();
        yield return new WaitForSeconds(0.5f); // delay thi triển
        anim.SetTrigger("IsAtk2");
        
        yield return new WaitForSeconds(1f);
        anim.ResetTrigger("IsAtk2");
        currentState = State.Chasing;
    }
    
    public void SpawnMinions()
    {
        spawnedMinions.Clear();

        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject minion = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            spawnedMinions.Add(minion);
        }

        isWaitingForMinions = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, moveRange);
    }
}
