using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float startingHealth = 3f;
    [SerializeField] private GameObject enemyObj;
    [SerializeField] private float reviveCoolDown = 1.5f;
    public bool isReviving { get; private set; }
    public float currentHealth { get; private set; }
    private Animator anim;
    private Transform player;
    private Rigidbody2D rb;

    public bool isDead;
    private bool preventRespawn = false;
    private bool isPlayerNearby = false;

    private bool isFacingRight = true; // Biến để theo dõi hướng nhìn của enemy

    protected virtual void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void Update()
    {
        if (isDead && isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            preventRespawn = true;
        }

        CheckPlayer();
        CheckAndUpdateDirection();
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, startingHealth);
        Debug.Log(currentHealth);
        if (currentHealth > 0)
        {
            if (anim != null) anim.SetTrigger("IsHit");
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
        Debug.Log("Enemy chết rồi!");

        if (anim != null) anim.SetTrigger("IsDeath");
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        StartCoroutine(WaitForKillOrRespawn(5f));
    }

    IEnumerator WaitForKillOrRespawn(float delay)
    {
        float timer = 0f;

        while (timer < delay)
        {
            if (preventRespawn)
            {
                Debug.Log("Đã bị khóa còng");
                anim.SetTrigger("IsBlock");
                Destroy(enemyObj, 10f);
                yield break;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        currentHealth = Mathf.RoundToInt(startingHealth * 0.3f);
        isDead = false;
        Debug.Log("Enemy hồi sinh với máu: " + currentHealth);
        rb.velocity = Vector2.zero;
        rb.isKinematic = false;

        if (anim != null) anim.SetTrigger("IsRevive");
        Revive();
    }

    void CheckPlayer()
    {
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
        int direction = isFacingRight ? -1 : 1; // Di chuyển ngược hướng mặt enemy
        Vector2 targetPos = startPos + new Vector2(direction * distance, 0f);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            rb.MovePosition(Vector2.Lerp(startPos, targetPos, t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(targetPos); // Đảm bảo tới đúng vị trí cuối
    }

    private void CheckAndUpdateDirection()
    {
        if (player == null || rb == null) return;

        Vector2 directionToPlayer = player.position - transform.position;

        // Cập nhật hướng nhìn dựa trên vị trí của player
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
        if (isDead || isReviving) return;
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void Revive()
    {
        StartCoroutine(ReviveCooldown());
    }
    //Rito
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