using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossController : MonoBehaviour
{
    private enum State { Idle, Chasing, Attacking, Returning, Skill2, Skill3 }
    private State currentState;

    [Header("Settings")]
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float moveRange = 20f;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float attackCoolDown = 2f;
    [SerializeField] private float attack1Damage;
    [SerializeField] private float attack3Damage;

    [Header("Skill 2 - Summon Minions")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    private List<GameObject> spawnedMinions = new List<GameObject>();
    private bool isWaitingForMinions = false;

    [Header("Skill 3 - Charge Attack")]
    [SerializeField] private float chargeDistance = 5f;
    [SerializeField] private float chargeSpeed = 10f;

    [SerializeField] private Transform startPoint;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private EnemyHealth enemyHealth;

    private Transform player;
    private Animator anim;
    private float cooldownTimer = Mathf.Infinity;
    private float chaseTimer;
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
            spawnedMinions.RemoveAll(m => m == null);
            if (spawnedMinions.Count == 0)
            {
                isWaitingForMinions = false;
                currentState = State.Chasing;
                gameObject.layer = LayerMask.NameToLayer("Enemy");
            }
            else
            {
                anim.SetBool("IsMove", false);
                gameObject.layer = LayerMask.NameToLayer("Default");
                return;
            }
        }

        cooldownTimer += Time.deltaTime;
        float healthPercent = enemyHealth.currentHealth / enemyHealth.startingHealth;

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
                    float rng = Random.value;
                    if (rng < 0.7f)
                    {
                        anim.SetTrigger("IsAtk1");
                    }
                    else
                    {
                        StartCoroutine(UseSkill3());
                    }
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
                anim.SetBool("IsMove", true);
                MoveTowards(startPoint.position);
                if (Vector2.Distance(transform.position, startPoint.position) < 0.1f)
                    currentState = State.Idle;
                break;
        }
    }

    private void MoveTowards(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        FacePlayer(target);
    }

    private void FacePlayer()
    {
        FacePlayer(player.position);
    }

    private void FacePlayer(Vector2 target)
    {
        if (target.x > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }
    
    
    private void Attack1Damage()
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null && pc.isParrying)
        {
            enemyHealth.BeAttack(0.5f, 0.1f);
            AudioManager.Instance.PlaySound(AudioManager.Instance.parryClip);
            anim.SetTrigger("IsHit");
            cooldownTimer = -0.5f;
            return;
        }

        player.GetComponent<PlayerHealth>().TakeDamage(attack1Damage);
    }
    
    private void Attack3Damage()
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null && pc.isParrying)
        {
            enemyHealth.BeAttack(0.5f, 0.1f);
            anim.SetTrigger("IsHit");
            cooldownTimer = -0.5f;
            return;
        }

        player.GetComponent<PlayerHealth>().TakeDamage(attack3Damage);
    }

    private IEnumerator UseSkill2()
    {
        currentState = State.Skill2;

        // Di chuyển về điểm startpoint
        while (Vector2.Distance(transform.position, startPoint.position) > 0.1f)
        {
            MoveTowards(startPoint.position);
            yield return null;
        }

        anim.SetBool("IsMove", false);
        FacePlayer();
        yield return new WaitForSeconds(0.5f);
        anim.SetTrigger("IsAtk2");

        yield return new WaitForSeconds(1.5f);
        //SpawnMinions();
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

    private IEnumerator UseSkill3()
    {
        currentState = State.Skill3;
        anim.SetTrigger("IsAtk3");
        Vector2 direction = (player.position - transform.position).normalized;
        float timer = 0f;
        while (timer < chargeDistance / chargeSpeed)
        {
            rb.MovePosition(rb.position + direction * chargeSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        currentState = State.Chasing;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, moveRange);
    }
}
