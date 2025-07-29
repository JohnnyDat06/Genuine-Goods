using System.Collections;
using UnityEngine;
using Cinemachine;
public class AllyAI : MonoBehaviour
{
    [Header("AI Settings")]
    [Tooltip("Tốc độ bỏ chạy của Ally")]
    public float runSpeed = 8f;

    [Tooltip("Điểm mà Ally sẽ chạy tới và dừng lại")]
    public Transform stopPoint;

    [Tooltip("Cửa sắt mà Ally cần chạy qua")]
    public IronGateHealth ironGate;
    [Header("Cinemachine Settings")]
    [Tooltip("Kéo Camera ảo của Đồng Minh vào đây")]
    public CinemachineVirtualCamera vcamAlly;

    // Các biến private để quản lý trạng thái
    private Animator anim;
    private Rigidbody2D rb;
    private bool hasBeenTriggered = false;
    private bool isFacingRight = true;
    private Transform player;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // --- PHẦN CODE "TỰ SỬA LỖI" QUAN TRỌNG NHẤT ---

        // 1. Tự tìm Player bằng Tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("LỖI: Không tìm thấy object có tag 'Player'!", this.gameObject);
        }

        // 2. Kiểm tra và tự tìm Stop Point nếu trong Inspector bị rỗng
        if (stopPoint == null)
        {
            Debug.LogWarning("Stop Point chưa được gán, đang tự động tìm bằng tên 'AllyStopPoint'...", this.gameObject);
            GameObject stopPointObject = GameObject.Find("AllyStopPoint");
            if (stopPointObject != null)
            {
                stopPoint = stopPointObject.transform;
            }
            else
            {
                Debug.LogError("LỖI: Không tìm thấy object tên là 'AllyStopPoint' trong Scene!", this.gameObject);
            }
        }

        // 3. Kiểm tra và tự tìm Cửa Sắt nếu trong Inspector bị rỗng
        if (ironGate == null)
        {
            Debug.LogWarning("Iron Gate chưa được gán, đang tự động tìm bằng script 'IronGateHealth'...", this.gameObject);
            ironGate = FindObjectOfType<IronGateHealth>();
            if (ironGate == null)
            {
                Debug.LogError("LỖI: Không tìm thấy object nào có script 'IronGateHealth' trong Scene!", this.gameObject);
            }
        }
        // --- KẾT THÚC PHẦN CODE TỰ SỬA LỖI ---
    }

    void Update()
    {
        if (hasBeenTriggered)
        {
            Flee();
        }
        else if (player != null) // Chỉ nhìn player nếu đã tìm thấy
        {
            FacePlayer();
        }
    }

    // Va chạm với Player để kích hoạt
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasBeenTriggered && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Ally sợ hãi và bắt đầu bỏ chạy! Kích hoạt camera cinematic.");
            hasBeenTriggered = true;
            anim.SetTrigger("Scared");

            // --- THÊM DÒNG NÀY ĐỂ CHUYỂN CAMERA ---
            if (vcamAlly != null)
            {
                vcamAlly.Priority = 11; // Tăng lên 11, cao hơn 10 của VCam_Player
            }
        }
    }

    // Hàm xử lý logic bỏ chạy
    //private void Flee()
    //{
    //    // Thêm kiểm tra null ở đây để tuyệt đối an toàn
    //    if (stopPoint == null)
    //    {
    //        Debug.LogError("Vẫn không có Stop Point để chạy tới! Vô hiệu hóa AI.", this.gameObject);
    //        this.enabled = false;
    //        return;
    //    }

    //    if (Vector2.Distance(transform.position, stopPoint.position) > 1.5f)
    //    {
    //        Vector2 direction = (stopPoint.position - transform.position).normalized;
    //        rb.velocity = new Vector2(direction.x * runSpeed, rb.velocity.y);

    //        if (direction.x > 0 && !isFacingRight) Flip();
    //        else if (direction.x < 0 && isFacingRight) Flip();

    //        // Chỉ thực hiện khi ironGate đã được gán VÀ khoảng cách đủ gần
    //        if (ironGate != null && Vector2.Distance(transform.position, ironGate.transform.position) < 4f)
    //        {
    //            ironGate.OpenForAlly();
    //            ironGate = null; // Đảm bảo chỉ gọi 1 lần
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Ally đã đến nơi an toàn và dừng lại.");
    //        rb.velocity = Vector2.zero;
    //        anim.SetTrigger("Idle");
    //        this.enabled = false;
    //    }
    //}
    // Hàm xử lý logic bỏ chạy
    private void Flee()
    {
        // --- BẮT ĐẦU PHẦN NÂNG CẤP ---

        // Lấy thông tin về trạng thái animation hiện tại trên layer 0
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // Chỉ thực hiện logic di chuyển NẾU animation hiện tại là "Run"
        if (stateInfo.IsName("Run")) // <-- Tên "Run" phải khớp với tên state trong Animator
        {
            // --- Toàn bộ code di chuyển cũ sẽ nằm trong cái if này ---

            if (stopPoint == null)
            {
                Debug.LogError("Vẫn không có Stop Point để chạy tới! Vô hiệu hóa AI.", this.gameObject);
                this.enabled = false;
                return;
            }

            if (Vector2.Distance(transform.position, stopPoint.position) > 1.5f)
            {
                Vector2 direction = (stopPoint.position - transform.position).normalized;
                rb.velocity = new Vector2(direction.x * runSpeed, rb.velocity.y);

                if (direction.x > 0 && !isFacingRight) Flip();
                else if (direction.x < 0 && isFacingRight) Flip();

                if (ironGate != null && Vector2.Distance(transform.position, ironGate.transform.position) < 4f)
                {
                    ironGate.OpenForAlly();
                    ironGate = null;
                }
            }
            else
            {
                Debug.Log("Ally đã đến nơi an toàn và dừng lại. Trả camera về bình thường.");
                rb.velocity = Vector2.zero;
                anim.SetTrigger("Idle");

                // --- THÊM DÒNG NÀY ĐỂ TRẢ CAMERA LẠI ---
                if (vcamAlly != null)
                {
                    vcamAlly.Priority = 9; // Hạ xuống 9, thấp hơn 10 của VCam_Player
                }

                this.enabled = false;
            }
            // --- Kết thúc phần code di chuyển ---
        }
        else
        {
            // Nếu không phải animation "Run" (tức là đang "Scare"), thì bắt nó đứng im
            rb.velocity = Vector2.zero;
        }

        // --- KẾT THÚC PHẦN NÂNG CẤP ---
    }

    // Hàm để Ally luôn nhìn về phía người chơi
    private void FacePlayer()
    {
        if (player.position.x > transform.position.x && !isFacingRight) Flip();
        else if (player.position.x < transform.position.x && isFacingRight) Flip();
    }

    // Hàm lật mặt nhân vật
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}