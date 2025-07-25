using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyFinisher : MonoBehaviour
{
    [Header("UI Tương tác")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private Image progressRingImage;
    [SerializeField] private Vector3 promptOffset = new Vector3(0, 1.5f, 0);

    [Header("Thiết lập Tương tác")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float holdDuration = 1.5f;
    [SerializeField] private float interactionDistance = 2f;

    // Các biến private
    private EnemyHealth enemyHealth;
    private Transform playerTransform;
    private float holdTimer = 0f;
    private bool canInteract = false;
    private bool isHoldingKey = false; // Biến trạng thái mới để theo dõi việc giữ phím

    void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        // Luôn đảm bảo UI được tắt và reset khi bắt đầu
        DeactivateAndResetUI();
    }

    void Update()
    {
        // Bước 1: Kiểm tra xem có thể tương tác được không
        bool isPossible = enemyHealth.isDead && Vector2.Distance(transform.position, playerTransform.position) < interactionDistance;

        // Nếu trạng thái "có thể tương tác" thay đổi
        if (isPossible != canInteract)
        {
            canInteract = isPossible;
            if (canInteract)
            {
                // Khi vừa có thể tương tác, bật UI và reset mọi thứ
                ActivateUI();
            }
            else
            {
                // Khi không thể tương tác nữa, tắt UI
                DeactivateAndResetUI();
            }
        }

        // Bước 2: Nếu có thể tương tác, xử lý input và vị trí UI
        if (canInteract)
        {
            UpdatePromptPosition();
            HandlePlayerInput();
        }
    }

    private void HandlePlayerInput()
    {
        // Khi người chơi BẮT ĐẦU nhấn phím
        if (Input.GetKeyDown(interactKey))
        {
            isHoldingKey = true;
            holdTimer = 0f; // Reset timer ngay khi bắt đầu giữ
        }
        // Khi người chơi THẢ phím ra
        else if (Input.GetKeyUp(interactKey))
        {
            isHoldingKey = false;
            holdTimer = 0f; // Reset timer khi thả phím
        }

        // CHỈ khi người chơi đang thực sự giữ phím, chúng ta mới tăng timer
        if (isHoldingKey)
        {
            holdTimer += Time.deltaTime;
            if (holdTimer >= holdDuration)
            {
                PerformFinisher();
            }
        }

        // Luôn cập nhật vòng tròn để hiển thị tiến trình
        if (progressRingImage != null)
        {
            progressRingImage.fillAmount = holdTimer / holdDuration;
        }
    }

    private void PerformFinisher()
    {
        Debug.Log("Thực hiện khóa tay thành công!");
        enemyHealth.ConfirmFinisher();
        DeactivateAndResetUI();
        this.enabled = false; // Vô hiệu hóa script này trên kẻ địch đã bị khóa
    }

    private void ActivateUI()
    {
        holdTimer = 0f;
        isHoldingKey = false;
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
        if (progressRingImage != null)
        {
            progressRingImage.fillAmount = 0;
        }
    }

    private void DeactivateAndResetUI()
    {
        holdTimer = 0f;
        isHoldingKey = false;
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        if (progressRingImage != null)
        {
            progressRingImage.fillAmount = 0;
        }
    }

    private void UpdatePromptPosition()
    {
        if (interactionPrompt != null)
        {
            Vector3 worldPosition = transform.position + promptOffset;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            interactionPrompt.transform.position = screenPosition;
        }
    }
}
