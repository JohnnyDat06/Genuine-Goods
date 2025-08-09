// Enemy3Controller.cs
using UnityEngine;

public class Enemy3Controller : MonoBehaviour
{
    private enum State { Idle, Chasing, Attacking }
    private State currentState;

    [Header("Target Settings")]
    [Tooltip("Kéo object Ông Thợ Điện vào đây (có thể để trống)")]
    [SerializeField] private Transform electricianTarget;
    [Tooltip("Khoảng cách mà Enemy sẽ phát hiện và chuyển mục tiêu sang Player")]
    [SerializeField] private float playerDetectionRange = 15f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackDamage = 30f;
    [SerializeField] private float attackCoolDown = 2f;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;

    // Biến private
    private Transform player;
    private Transform currentTarget;
    private Animator anim;
    private float cooldownTimer = Mathf.Infinity;
    private EnemyHealth enemyHealth;
    private Collider2D myCollider;
    private Collider2D electricianCollider;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        myCollider = GetComponent<Collider2D>();

        if (electricianTarget != null)
        {
            electricianCollider = electricianTarget.GetComponent<Collider2D>();
        }

        float multiplier = 1f;
        if (MissionManager.Instance != null)
        {
            multiplier += 0.5f * MissionManager.Instance.failCount;
        }
        attackDamage *= multiplier;
    }

    void Start()
    {
        // ===== THAY ĐỔI 1: XỬ LÝ KHI KHÔNG CÓ THỢ ĐIỆN =====
        // Nếu có mục tiêu Thợ điện, đuổi theo ông ta.
        if (electricianTarget != null)
        {
            currentTarget = electricianTarget;
            currentState = State.Chasing;
        }
        // Nếu không có Thợ điện nhưng có Player, đuổi theo Player ngay lập tức.
        else if (player != null)
        {
            currentTarget = player;
            currentState = State.Chasing;
            Debug.Log("Không có mục tiêu Thợ Điện, chuyển sang đuổi theo Player.");
        }
        // Nếu không có cả hai, đứng yên.
        else
        {
            currentState = State.Idle;
            Debug.LogWarning("Enemy không có mục tiêu nào để đuổi theo (cả Thợ Điện và Player).");
        }
        // ====================================================
    }

    private void Update()
    {
        if (enemyHealth.currentHealth <= 0 || enemyHealth.isReviving) return;

        UpdateTarget();

        if (currentTarget == null)
        {
            currentState = State.Idle;
        }

        cooldownTimer += Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                anim.SetBool("IsMove", false);
                if (currentTarget != null)
                {
                    currentState = State.Chasing;
                }
                break;

            case State.Chasing:
                if (currentTarget == null) { currentState = State.Idle; break; }
                anim.SetBool("IsMove", true);
                MoveTowards(currentTarget.position);
                if (Vector2.Distance(transform.position, currentTarget.position) <= attackRange)
                {
                    currentState = State.Attacking;
                }
                break;

            case State.Attacking:
                if (currentTarget == null) { currentState = State.Idle; break; }
                anim.SetBool("IsMove", false);
                FaceTarget();
                if (cooldownTimer >= attackCoolDown)
                {
                    cooldownTimer = 0;
                    Attack();
                }
                if (Vector2.Distance(transform.position, currentTarget.position) > attackRange)
                {
                    currentState = State.Chasing;
                }
                break;
        }
    }

    private void UpdateTarget()
    {
        // Nếu không có Thợ điện, mục tiêu luôn là Player (nếu có)
        if (electricianTarget == null)
        {
            if (player != null) currentTarget = player;
            return; // Bỏ qua logic chuyển đổi mục tiêu phức tạp bên dưới
        }

        // Logic cũ vẫn giữ nguyên nếu có Thợ điện
        if (myCollider == null || electricianCollider == null) return;

        if (player != null && Vector2.Distance(transform.position, player.position) <= playerDetectionRange)
        {
            if (currentTarget != player)
            {
                currentTarget = player;
                currentState = State.Chasing;
                Physics2D.IgnoreCollision(myCollider, electricianCollider, true);
                Debug.Log("Đã bật ignore collision: Enemy có thể đi xuyên qua Thợ điện.");
            }
        }
        else
        {
            if (currentTarget != electricianTarget)
            {
                currentTarget = electricianTarget;
                currentState = State.Chasing;
                Physics2D.IgnoreCollision(myCollider, electricianCollider, false);
                Debug.Log("Đã tắt ignore collision: Enemy sẽ va chạm với Thợ điện.");
            }
        }
    }

    private void Attack()
    {
        anim.SetTrigger("IsAtk");
    }

    public void AttackDamage()
    {
        if (currentTarget == null) return;

        if (currentTarget.CompareTag("Player"))
        {
            PlayerController pc = currentTarget.GetComponent<PlayerController>();
            if (pc != null && pc.isParrying)
            {
                GetComponent<EnemyHealth>().BeAttack(0.5f, 0.1f);
                anim.SetTrigger("IsHit");
                if (AudioManager.Instance != null) AudioManager.Instance.PlaySound(AudioManager.Instance.parryClip);
                cooldownTimer = -0.5f;
                return;
            }
            PlayerHealth playerHealth = currentTarget.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    private void FaceTarget()
    {
        if (currentTarget == null) return;
        if (currentTarget.position.x > transform.position.x)
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
