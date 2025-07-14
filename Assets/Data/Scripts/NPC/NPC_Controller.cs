using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float moveDistance = 3f;

    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 currentTarget; // Mục tiêu hiện tại mà NPC đang hướng tới

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Xác định 2 điểm A và B dựa trên vị trí ban đầu và khoảng cách di chuyển
        pointA = transform.position;
        pointB = new Vector3(transform.position.x + moveDistance, transform.position.y, transform.position.z);

        // Ban đầu, cho NPC di chuyển về phía điểm B
        currentTarget = pointB;
        // Đảm bảo NPC quay mặt đúng hướng khi bắt đầu
        FlipTowardsTarget();
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
        // Nếu không có animator thì không cần chạy animation
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }

        // Di chuyển NPC về phía mục tiêu hiện tại
        // Vector3.MoveTowards sẽ không bao giờ đi vượt quá mục tiêu, giải quyết triệt để vấn đề "overshoot"
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);

        // Kiểm tra xem NPC đã đến gần mục tiêu chưa
        // Chúng ta không dùng == vì sai số số thực, dùng một khoảng nhỏ để kiểm tra là tốt nhất
        if (Vector3.Distance(transform.position, currentTarget) < 0.01f)
        {
            // Nếu đã đến nơi, đổi mục tiêu
            if (currentTarget == pointA)
            {
                currentTarget = pointB;
            }
            else
            {
                currentTarget = pointA;
            }
            // Sau khi đổi mục tiêu, lật hình ảnh của NPC
            FlipTowardsTarget();
        }
    }

    private void FlipTowardsTarget()
    {
        // Xác định hướng của mục tiêu so với vị trí hiện tại
        float directionToTarget = currentTarget.x - transform.position.x;

        // Lật scale của NPC cho phù hợp
        if (directionToTarget > 0 && transform.localScale.x < 0)
        {
            // Đang đi sang phải nhưng mặt quay sang trái -> lật lại
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (directionToTarget < 0 && transform.localScale.x > 0)
        {
            // Đang đi sang trái nhưng mặt quay sang phải -> lật lại
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}