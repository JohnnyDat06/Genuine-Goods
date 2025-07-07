using UnityEngine;

public class CatController : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float idleTime = 5f;

    [Header("Bird Interaction")]
    [SerializeField] private Transform bird;
    [SerializeField] private float detectRange = 2f;

    private Vector3 targetPoint;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isIdle = false;
    private bool isAttackingBird = false;

    private void Start()
    {
        targetPoint = pointB.position;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Nếu đang idle hoặc đang tấn công chim thì không đi tuần
        if (isIdle || isAttackingBird) return;

        // Kiểm tra nếu gần chim
        if (Vector3.Distance(transform.position, bird.position) < detectRange)
        {
            StartCoroutine(AttackBird());
            return;
        }

        // Di chuyển tuần tra
        animator.SetBool("isRunning", true);
        MoveTowardsTarget(targetPoint);

        // Đến nơi thì chuyển Idle
        if (Vector3.Distance(transform.position, targetPoint) < 0.05f)
        {
            StartCoroutine(IdleAndSwitchTarget());
        }
    }

    private void MoveTowardsTarget(Vector3 point)
    {
        Vector3 direction = (point - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Lật hình theo hướng
        if (direction.x != 0)
            spriteRenderer.flipX = direction.x < 0;
    }

    private System.Collections.IEnumerator IdleAndSwitchTarget()
    {
        isIdle = true;
        animator.SetBool("isRunning", false);

        yield return new WaitForSeconds(idleTime);

        // Chuyển hướng
        targetPoint = (targetPoint == pointA.position) ? pointB.position : pointA.position;

        isIdle = false;
    }

    private System.Collections.IEnumerator AttackBird()
    {
        isAttackingBird = true;

        // Lật theo hướng bird
        Vector3 dir = (bird.position - transform.position).normalized;
        if (dir.x != 0)
            spriteRenderer.flipX = dir.x < 0;

        animator.SetBool("isRunning", true);

        // Di chuyển đến gần bird
        while (Vector3.Distance(transform.position, bird.position) > 0.5f)
        {
            transform.position += dir * speed * Time.deltaTime;
            yield return null;
        }

        // Dừng và tấn công
        animator.SetBool("isRunning", false);
        animator.SetTrigger("isAttacking");

        yield return new WaitForSeconds(1f); // thời gian animation đánh

        // Idle ngẫu nhiên 3 - 5 giây
        float randomIdle = Random.Range(3f, 5f);
        isIdle = true;
        yield return new WaitForSeconds(randomIdle);
        isIdle = false;

        isAttackingBird = false;
    }
}
