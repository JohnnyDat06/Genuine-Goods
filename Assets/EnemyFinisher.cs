using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EnemyHealth))] // Đảm bảo đối tượng luôn có script EnemyHealth
public class EnemyFinisher : MonoBehaviour
{
    [Header("UI Tương tác")]
    [Tooltip("Đối tượng GameObject chứa chữ E và vòng tròn.")]
    [SerializeField] private GameObject interactionPrompt;
    [Tooltip("Image của vòng tròn tiến trình.")]
    [SerializeField] private Image progressRingImage;
    // --- BIẾN MỚI: ĐỂ ĐIỀU CHỈNH VỊ TRÍ UI ---
    [Tooltip("Vị trí của UI so với đầu của kẻ địch.")]
    [SerializeField] private Vector3 promptOffset = new Vector3(0, 1.5f, 0);

    [Header("Thiết lập Tương tác")]
    [Tooltip("Phím để thực hiện hành động.")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [Tooltip("Thời gian cần giữ phím để hoàn tất.")]
    [SerializeField] private float holdDuration = 1.5f;
    [Tooltip("Khoảng cách tối đa để có thể tương tác.")]
    [SerializeField] private float interactionDistance = 2f;

    // Các biến private
    private EnemyHealth enemyHealth;
    private Transform playerTransform;
    private float holdTimer = 0f;
    private bool canInteract = false;

    void Awake()
    {
        // Lấy các component cần thiết
        enemyHealth = GetComponent<EnemyHealth>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        // Ẩn UI khi bắt đầu
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        if (progressRingImage != null)
        {
            progressRingImage.fillAmount = 0;
        }
    }

    void Update()
    {
        // Kiểm tra xem có thể tương tác được không
        CheckInteractionPossibility();

        // Nếu có thể, xử lý input của người chơi và cập nhật vị trí UI
        if (canInteract)
        {
            UpdatePromptPosition(); // <-- GỌI HÀM CẬP NHẬT VỊ TRÍ
            HandlePlayerInput();
        }
    }

    // --- HÀM MỚI: CẬP NHẬT VỊ TRÍ CỦA UI TRÊN MÀN HÌNH ---
    private void UpdatePromptPosition()
    {
        if (interactionPrompt != null)
        {
            // Lấy vị trí thế giới của kẻ địch, cộng thêm một khoảng lệch
            Vector3 worldPosition = transform.position + promptOffset;
            // Chuyển đổi vị trí thế giới đó sang vị trí trên màn hình
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            // Gán vị trí màn hình cho UI
            interactionPrompt.transform.position = screenPosition;
        }
    }

    private void CheckInteractionPossibility()
    {
        // Điều kiện: Kẻ địch đã chết VÀ người chơi ở đủ gần
        if (enemyHealth.isDead && Vector2.Distance(transform.position, playerTransform.position) < interactionDistance)
        {
            // Nếu chưa thể tương tác, hãy bật nó lên
            if (!canInteract)
            {
                canInteract = true;
                if (interactionPrompt != null)
                {
                    interactionPrompt.SetActive(true);
                }
            }
        }
        else
        {
            // Nếu không đủ điều kiện, tắt tương tác
            if (canInteract)
            {
                canInteract = false;
                holdTimer = 0f; // Reset timer nếu người chơi đi ra xa
                if (interactionPrompt != null)
                {
                    interactionPrompt.SetActive(false);
                }
                if (progressRingImage != null)
                {
                    progressRingImage.fillAmount = 0;
                }
            }
        }
    }

    private void HandlePlayerInput()
    {
        // Nếu người chơi đang giữ phím E
        if (Input.GetKey(interactKey))
        {
            holdTimer += Time.deltaTime;

            // Cập nhật vòng tròn tiến trình
            if (progressRingImage != null)
            {
                progressRingImage.fillAmount = holdTimer / holdDuration;
            }

            // Nếu giữ đủ lâu
            if (holdTimer >= holdDuration)
            {
                PerformFinisher();
            }
        }

        // Nếu người chơi thả phím E ra
        if (Input.GetKeyUp(interactKey))
        {
            // Reset lại tiến trình
            holdTimer = 0f;
            if (progressRingImage != null)
            {
                progressRingImage.fillAmount = 0;
            }
        }
    }

    private void PerformFinisher()
    {
        Debug.Log("Thực hiện khóa tay thành công!");

        // Tắt UI
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // Ra lệnh cho script EnemyHealth "khóa" kẻ địch lại
        enemyHealth.ConfirmFinisher();

        // Vô hiệu hóa script này để không thể tương tác được nữa
        this.enabled = false;
    }
}
