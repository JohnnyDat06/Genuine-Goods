using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    //Rito added
    [Header("Health Settings")]
    [SerializeField] public float startingHealth = 3f;

    //Rito added
    [Header("Revive & Destroy Settings")]
    [SerializeField] private GameObject enemyObj;
    [SerializeField] private float reviveCoolDown = 1.5f;
    [SerializeField] private float destroyDelayForNormalEnemy = 3f; //Rito added

    // --- State Variables ---
    public float currentHealth { get; private set; }
    public bool isDead { get; private set; }
    public bool isReviving { get; private set; }
    public bool preventRespawn = false;
    public bool isFinishedAndTalkable { get; private set; } = false; //Rito added

    // --- Components ---
    private Animator anim;
    private Transform player;
    private Rigidbody2D rb;

    private bool isPlayerNearby = false;
    private bool isFacingRight = true;

    protected virtual void Awake()
    {
        float multiplier = 1f;
        if (MissionManager.Instance != null)
        {
            multiplier += 0.5f * MissionManager.Instance.failCount;
        }

        startingHealth *= multiplier;
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    //Rito edited
    protected virtual void Update()
    {
        if (isDead)
        {
            CheckPlayer();

            if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
            {
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySound(AudioManager.Instance.handCuffClip);
                preventRespawn = true;
            }
        }
        else
        {
            CheckAndUpdateDirection();
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, startingHealth);
        if (currentHealth > 0)
        {
            if (anim != null) anim.SetTrigger("IsHit"); //Rito edited
        }
        else
        {
            Die();
        }
        BeAttack(0.6f, 0.1f);
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        if (anim != null) anim.SetTrigger("IsDeath"); //Rito edited
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        StartCoroutine(WaitForKillOrRespawn(5f));
    }

    //Rito edited
    IEnumerator WaitForKillOrRespawn(float delay)
    {
        float timer = 0f;
        while (timer < delay)
        {
            if (preventRespawn)
            {
                if (anim != null) anim.SetTrigger("IsBlock");

                NPCInteractionController chatController = GetComponent<NPCInteractionController>();

                if (chatController != null)
                {
                    isFinishedAndTalkable = true;
                }
                else
                {
                    Destroy(enemyObj, destroyDelayForNormalEnemy);
                }

                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        currentHealth = Mathf.RoundToInt(startingHealth * 0.3f);
        isDead = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = false;
        if (anim != null) anim.SetTrigger("IsRevive");
        Revive();
    }

    void CheckPlayer()
    {
        if (player == null) return; //Rito added
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        isPlayerNearby = distanceToPlayer < 1f && isDead;
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
        Vector2 targetPos = startPos + new Vector2(direction * distance, 0f);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            rb.MovePosition(Vector2.Lerp(startPos, targetPos, t));
            elapsed += Time.deltaTime;
            yield return null;
        }
        rb.MovePosition(targetPos);
    }

    private void CheckAndUpdateDirection()
    {
        if (player == null || rb == null || isDead || isReviving) return; //Rito edited

        Vector2 directionToPlayer = player.position - transform.position;
        if (directionToPlayer.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (directionToPlayer.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        if (isDead || isReviving) return; //Rito added
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void Revive()
    {
        StartCoroutine(ReviveCooldown());
    }

    public void ConfirmFinisher()
    {
        preventRespawn = true;
    }

    private IEnumerator ReviveCooldown()
    {
        isReviving = true;
        yield return new WaitForSeconds(reviveCoolDown);
        isReviving = false;
    }
}