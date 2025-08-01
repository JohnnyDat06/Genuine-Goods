using System.Collections;
using UnityEngine;
using Cinemachine;

public class AllyAI : MonoBehaviour
{
    [Header("AI Settings")]
    [Tooltip("Khoảng cách để Ally phát hiện Player và bắt đầu chạy")]
    public float detectionRange = 10f;
    [Tooltip("Tốc độ bỏ chạy của Ally")]
    public float runSpeed = 8f;
    [Tooltip("Ban đầu, Ally sẽ quay mặt sang phải?")]
    public bool startFacingRight = false;
    [Tooltip("Thời gian Ally nhìn Player trước khi bỏ chạy")]
    public float turnDelay = 1f;

    [Header("Effects")]
    [Tooltip("Kéo object dấu chấm hỏi vào đây")]
    public GameObject questionMarkObject;

    [Header("Sequence Points")]
    [Tooltip("Điểm mà Ally sẽ chạy tới và dừng lại")]
    public Transform stopPoint;
    [Tooltip("Cửa sắt mà Ally cần chạy qua")]
    public IronGateHealth ironGate;
    [Tooltip("Vị trí cuối cùng Ally sẽ dịch chuyển tới sau khi xong việc")]
    public Transform finalTeleportPoint;

    [Header("Animation & Camera")]
    [Tooltip("Thời lượng của animation sợ hãi (tính bằng giây)")]
    public float scareAnimationDuration = 1.5f;
    [Tooltip("Kéo Camera ảo của Đồng Minh vào đây")]
    public CinemachineVirtualCamera vcamAlly;

    // Các biến private để quản lý
    private Animator anim;
    private Rigidbody2D rb;
    private Transform player;
    private bool sequenceStarted = false;
    private bool isFacingRight = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // --- BẮT ĐẦU PHẦN NÂNG CẤP ---

        // 1. Đồng bộ trạng thái logic 'isFacingRight' với cài đặt trong Inspector
        isFacingRight = startFacingRight;

        // 2. Dựa vào 'isFacingRight' để set hướng nhìn ban đầu một cách chính xác
        float initialDirection = isFacingRight ? 1f : -1f;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * initialDirection, transform.localScale.y, transform.localScale.z);

        // --- KẾT THÚC PHẦN NÂNG CẤP ---

        // Tự động tìm các đối tượng cần thiết
        FindRequiredObjects();
    }

    void Update()
    {
        // Nếu chuỗi hành động chưa bắt đầu và đã tìm thấy người chơi
        if (!sequenceStarted && player != null)
        {
            // Chỉ kiểm tra khoảng cách tới người chơi
            if (Vector2.Distance(transform.position, player.position) < detectionRange)
            {
                // Bắt đầu chuỗi hành động
                sequenceStarted = true;
                StartCoroutine(AllySequence());
            }
        }
    }

    // Coroutine chứa toàn bộ kịch bản của Ally
    //private IEnumerator AllySequence()
    //{
    //    Debug.Log("Phát hiện Player! Bắt đầu chạy...");
    //    // 1. BẮT ĐẦU CHẠY VÀ CHUYỂN CAMERA
    //    anim.SetTrigger("Run");
    //    if (vcamAlly != null) vcamAlly.Priority = 11;

    //    // Vòng lặp di chuyển tới điểm dừng
    //    while (Vector2.Distance(transform.position, stopPoint.position) > 1.5f)
    //    {
    //        Vector2 direction = (stopPoint.position - transform.position).normalized;
    //        rb.velocity = new Vector2(direction.x * runSpeed, rb.velocity.y);
    //        FaceDirection(direction.x);

    //        // Mở cửa khi đến gần
    //        if (ironGate != null && Vector2.Distance(transform.position, ironGate.transform.position) < 4f)
    //        {
    //            ironGate.OpenForAlly();
    //            ironGate = null; // Chỉ gọi 1 lần
    //        }
    //        yield return null; // Đợi đến frame tiếp theo
    //    }

    //    Debug.Log("Đã đến điểm dừng. Bắt đầu sợ hãi.");
    //    // 2. DỪNG LẠI VÀ THỰC HIỆN ANIMATION SỢ HÃI
    //    rb.velocity = Vector2.zero;
    //    anim.SetTrigger("Scared");

    //    // 3. ĐỢI ANIMATION SỢ HÃI CHẠY XONG
    //    yield return new WaitForSeconds(scareAnimationDuration);

    //    Debug.Log("Sợ hãi xong. Trả camera về Player.");
    //    // 4. TRẢ CAMERA VỀ PLAYER
    //    if (vcamAlly != null) vcamAlly.Priority = 9;
    //    vcamAlly.Follow = null;

    //    // Đợi một chút cho camera chuyển cảnh mượt mà
    //    yield return new WaitForSeconds(1f);

    //    Debug.Log("Dịch chuyển đến vị trí cuối cùng.");
    //    // 5. DỊCH CHUYỂN (TELEPORT)
    //    if (finalTeleportPoint != null)
    //    {
    //        transform.position = finalTeleportPoint.position;
    //    }

    //    // Chuyển về trạng thái Idle và vô hiệu hóa
    //    anim.SetTrigger("Idle");
    //    this.enabled = false;
    //}
    private IEnumerator AllySequence()
    {
        Debug.Log("Phát hiện Player! Đang quay lại nhìn...");
        // 1. QUAY MẶT VỀ PHÍA PLAYER
        FaceDirection(player.position.x - transform.position.x);
        if (questionMarkObject != null) questionMarkObject.SetActive(true);
        
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<Animator>().enabled = false;
        player.GetComponent<PlayerController>().playerRigidbody.velocity = new Vector2(0f, 0f);
        
        // 2. ĐỢI MỘT CHÚT (TẠO CĂNG THẲNG)
        yield return new WaitForSeconds(turnDelay);

        Debug.Log("Bắt đầu chạy!");
        // 3. BẮT ĐẦU CHẠY VÀ CHUYỂN CAMERA
        if (questionMarkObject != null) questionMarkObject.SetActive(false);
        anim.SetTrigger("Run");
        if (vcamAlly != null) vcamAlly.Priority = 11;

        // Vòng lặp di chuyển tới điểm dừng (logic này không đổi)
        while (Vector2.Distance(transform.position, stopPoint.position) > 1.5f)
        {
            Vector2 direction = (stopPoint.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * runSpeed, rb.velocity.y);
            FaceDirection(direction.x);

            if (ironGate != null && Vector2.Distance(transform.position, ironGate.transform.position) < 4f)
            {
                ironGate.OpenForAlly();
                ironGate = null;
            }
            yield return null;
        }

        // Các bước còn lại từ 4 đến 6 không có gì thay đổi
        Debug.Log("Đã đến điểm dừng. Bắt đầu sợ hãi.");
        // 4. DỪNG LẠI VÀ THỰC HIỆN ANIMATION SỢ HÃI
        rb.velocity = Vector2.zero;
        anim.SetTrigger("Scared");

        // 5. ĐỢI ANIMATION SỢ HÃI CHẠY XONG
        yield return new WaitForSeconds(scareAnimationDuration);

        Debug.Log("Sợ hãi xong. Trả camera về Player.");
        // 6. TRẢ CAMERA, "THẢ" MỤC TIÊU VÀ DỊCH CHUYỂN
        if (vcamAlly != null)
        {
            vcamAlly.Priority = 9;
            vcamAlly.Follow = null;
        }
        
        player.GetComponent<PlayerController>().enabled = true;
        player.GetComponent<Animator>().enabled = true;
        
        yield return new WaitForSeconds(1f);

        if (finalTeleportPoint != null)
        {
            transform.position = finalTeleportPoint.position;
        }

        anim.SetTrigger("Idle");
        this.enabled = false;
    }

    // Hàm lật mặt theo hướng di chuyển
    private void FaceDirection(float moveDirection)
    {
        if (moveDirection > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moveDirection < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // Hàm tự tìm kiếm để tránh lỗi
    private void FindRequiredObjects()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (stopPoint == null) stopPoint = GameObject.Find("AllyStopPoint")?.transform;
        if (ironGate == null) ironGate = FindObjectOfType<IronGateHealth>();
        // Biến vcamAlly và finalTeleportPoint cần phải gán thủ công trong Inspector
    }
}