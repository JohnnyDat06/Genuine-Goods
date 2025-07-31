// File: SpecialEncounterNPC.cs
// Phiên bản cuối cùng, đã sửa lỗi logic tự vô hiệu hóa

using UnityEngine;

public class SpecialEncounterNPC : MonoBehaviour
{
    // Enum để quản lý các trạng thái của NPC
    private enum State
    {
        Inactive,       // Trạng thái chờ, chưa xuất hiện
        Approaching,    // Đang di chuyển đến điểm gặp gỡ
        WaitingForPlayer, // Đang đứng ở điểm gặp gỡ, chờ người chơi
        InDialogue,     // Đang trong cuộc hội thoại
        Departing       // Đang di chuyển đến điểm kết thúc để biến mất
    }

    [Header("Mission Settings")]
    [Tooltip("NPC sẽ chỉ xuất hiện khi Current Raid bằng giá trị này.")]
    [SerializeField] private int requiredRaid = 2;

    [Header("Movement Settings")]
    [Tooltip("Tốc độ di chuyển của NPC.")]
    [SerializeField] private float moveSpeed = 3f;
    [Tooltip("Điểm NPC sẽ xuất hiện ban đầu.")]
    [SerializeField] private Transform spawnPoint;
    [Tooltip("Điểm NPC sẽ di chuyển đến và chờ người chơi.")]
    [SerializeField] private Transform meetingPoint;
    [Tooltip("Điểm NPC sẽ đi đến và biến mất.")]
    [SerializeField] private Transform exitPoint;

    [Header("Interaction Settings")]
    [Tooltip("Hội thoại sẽ diễn ra khi người chơi tương tác.")]
    [SerializeField] private DialogueObject dialogue;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private GameObject interactionPrompt;

    // --- Components & State Variables ---
    private State currentState = State.Inactive;
    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col; // Thêm tham chiếu đến Collider
    private SpriteRenderer spriteRenderer; // Thêm tham chiếu đến SpriteRenderer
    private NPC_Controller npcController;
    private bool isPlayerInRange = false;
    private bool isFacingRight = true;

    private void Awake()
    {
        // Lấy tất cả các component cần thiết
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        npcController = GetComponent<NPC_Controller>();
    }

    private void Start()
    {
        // --- LOGIC MỚI: Chỉ ẩn NPC đi, không tắt cả GameObject ---
        // Bằng cách này, script vẫn có thể chạy hàm Update()
        spriteRenderer.enabled = false; // Ẩn hình ảnh
        col.enabled = false;            // Vô hiệu hóa va chạm
        rb.simulated = false;           // Vô hiệu hóa vật lý
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
    }

    private void Update()
    {
        // 1. LUÔN KIỂM TRA ĐIỀU KIỆN XUẤT HIỆN KHI ĐANG Ở TRẠNG THÁI INACTIVE
        if (currentState == State.Inactive)
        {
            if (MissionManager.Instance != null && MissionManager.Instance.currentRaid == requiredRaid)
            {
                Activate();
            }
            // Nếu chưa đủ điều kiện, không làm gì cả
            return;
        }

        // 2. BỘ NÃO ĐIỀU KHIỂN THEO TRẠNG THÁI (CHỈ CHẠY KHI ĐÃ ACTIVE)
        switch (currentState)
        {
            case State.Approaching:
                HandleMovement(meetingPoint);
                if (Vector2.Distance(transform.position, meetingPoint.position) < 0.1f)
                {
                    StopMovement();
                    currentState = State.WaitingForPlayer;
                }
                break;

            case State.WaitingForPlayer:
                if (isPlayerInRange && Input.GetKeyDown(interactionKey))
                {
                    StartInteraction();
                }
                break;

            case State.InDialogue:
                // DialogueManager đang kiểm soát
                break;

            case State.Departing:
                HandleMovement(exitPoint);
                if (Vector2.Distance(transform.position, exitPoint.position) < 0.1f)
                {
                    Destroy(gameObject);
                }
                break;
        }
    }

    private void Activate()
    {
        // Đặt NPC ở điểm xuất hiện và kích hoạt lại các component
        transform.position = spawnPoint.position;
        spriteRenderer.enabled = true; // Hiện lại hình ảnh
        col.enabled = true;            // Bật lại va chạm
        rb.simulated = true;           // Bật lại vật lý

        currentState = State.Approaching;
    }

    private void HandleMovement(Transform target)
    {
        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        if (anim != null) anim.SetBool("isWalking", true);

        if (direction.x > 0 && !isFacingRight) Flip();
        else if (direction.x < 0 && isFacingRight) Flip();
    }

    private void StopMovement()
    {
        rb.velocity = Vector2.zero;
        if (anim != null) anim.SetBool("isWalking", false);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void StartInteraction()
    {
        currentState = State.InDialogue;
        if (interactionPrompt != null) interactionPrompt.SetActive(false);

        System.Action onFinish = () => {
            currentState = State.Departing;
        };

        DialogueManager.instance.StartDialogue(dialogue, npcController, null, onFinish);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && currentState == State.WaitingForPlayer)
        {
            isPlayerInRange = true;
            if (interactionPrompt != null) interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactionPrompt != null) interactionPrompt.SetActive(false);
        }
    }
}