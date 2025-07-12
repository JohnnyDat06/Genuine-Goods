using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float moveDistance = 3f;

    private Vector3 startPosition;
    private int direction = 1;
    private Animator animator;

    void Awake()
    {
        // Lấy component Animator từ chính đối tượng NPC
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        startPosition = transform.position;
    }

    // Được gọi tự động khi script bị vô hiệu hóa (bắt đầu hội thoại)
    private void OnDisable()
    {
        // Buộc NPC về trạng thái Idle
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }
    }

    void Update()
    {
        // Báo cho Animator rằng NPC đang đi bộ
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }

        // Di chuyển qua lại
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // Kiểm tra và đảo chiều khi đi đủ khoảng cách
        if (Vector3.Distance(startPosition, transform.position) >= moveDistance)
        {
            direction *= -1;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}