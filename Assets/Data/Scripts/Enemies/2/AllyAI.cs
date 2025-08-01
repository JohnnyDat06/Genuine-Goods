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

        isFacingRight = startFacingRight;
        float initialDirection = isFacingRight ? 1f : -1f;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * initialDirection, transform.localScale.y, transform.localScale.z);

        FindRequiredObjects();
    }

    void Update()
    {
        if (!sequenceStarted && player != null)
        {
            if (Vector2.Distance(transform.position, player.position) < detectionRange)
            {
                sequenceStarted = true;
                StartCoroutine(AllySequence());
            }
        }
    }

    private IEnumerator AllySequence()
    {
        Debug.Log("Phát hiện Player! Đang quay lại nhìn...");
        // 1. DỪNG PLAYER, QUAY MẶT LẠI
        FaceDirection(player.position.x - transform.position.x);
        if (questionMarkObject != null) questionMarkObject.SetActive(true);

        // Tạm thời vô hiệu hóa điều khiển của người chơi
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
            playerController.GetComponent<Animator>().enabled = false;
            playerController.playerRigidbody.velocity = Vector2.zero;
        }

        // 2. ĐỢI MỘT CHÚT
        yield return new WaitForSeconds(turnDelay);

        Debug.Log("Bắt đầu chạy!");
        // 3. BẮT ĐẦU CHẠY VÀ CHUYỂN CAMERA
        if (questionMarkObject != null) questionMarkObject.SetActive(false);
        anim.SetTrigger("Run");
        if (vcamAlly != null) vcamAlly.Priority = 11;

        // 4. VÒNG LẶP DI CHUYỂN TỚI ĐIỂM DỪNG
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

        // 5. DỪNG LẠI VÀ SỢ HÃI
        Debug.Log("Đã đến điểm dừng. Bắt đầu sợ hãi.");
        rb.velocity = Vector2.zero;
        anim.SetTrigger("Scared");
        yield return new WaitForSeconds(scareAnimationDuration);

        // 6. TRẢ CAMERA VÀ ĐIỀU KHIỂN CHO PLAYER
        Debug.Log("Sợ hãi xong. Trả camera về Player.");
        if (vcamAlly != null)
        {
            vcamAlly.Priority = 9;
            vcamAlly.Follow = null;
        }
        if (playerController != null)
        {
            playerController.enabled = true;
            playerController.GetComponent<Animator>().enabled = true;
        }
        yield return new WaitForSeconds(1f);

        // 7. DỊCH CHUYỂN ĐẾN VỊ TRÍ CUỐI CÙNG
        if (finalTeleportPoint != null)
        {
            transform.position = finalTeleportPoint.position;
        }
        anim.SetTrigger("Idle");

        // Rito has edited
        Debug.Log("Sequence finished. Handing over to conversational script.");

        // Get the Conversational_NPC component on the same GameObject
        Conversational_NPC conversationScript = GetComponent<Conversational_NPC>();
        if (conversationScript != null)
        {
            // Enable the conversation script so it can start working
            conversationScript.enabled = true;
            Debug.Log("Conversational_NPC script has been enabled.");
        }
        else
        {
            Debug.LogWarning("Could not find the Conversational_NPC script on this NPC.");
        }

        // Disable this script (AllyAI) so it stops running
        this.enabled = false;
        // Rito has finished editing
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
    }
}