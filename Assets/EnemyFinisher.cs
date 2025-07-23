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
        enemyHealth = GetComponent<EnemyHealth>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
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
        CheckInteractionPossibility();

        if (canInteract)
        {
            UpdatePromptPosition();
            HandlePlayerInput();
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

    private void CheckInteractionPossibility()
    {
        if (enemyHealth.isDead && Vector2.Distance(transform.position, playerTransform.position) < interactionDistance)
        {
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
            if (canInteract)
            {
                canInteract = false;
                holdTimer = 0f;
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
        if (Input.GetKey(interactKey))
        {
            holdTimer += Time.deltaTime;

            if (progressRingImage != null)
            {
                progressRingImage.fillAmount = holdTimer / holdDuration;
            }

            if (holdTimer >= holdDuration)
            {
                PerformFinisher();
            }
        }

        if (Input.GetKeyUp(interactKey))
        {
            holdTimer = 0f;
            if (progressRingImage != null)
            {
                progressRingImage.fillAmount = 0;
            }
        }
    }

    // --- HÀM NÀY ĐÃ ĐƯỢC CẬP NHẬT ---
    private void PerformFinisher()
    {
        Debug.Log("Thực hiện khóa tay thành công!");

        // Tắt UI và reset vòng tròn
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        if (progressRingImage != null)
        {
            progressRingImage.fillAmount = 0; // <-- THÊM DÒNG NÀY ĐỂ RESET
        }

        // Ra lệnh cho script EnemyHealth "khóa" kẻ địch lại
        enemyHealth.ConfirmFinisher();

        // Vô hiệu hóa script này để không thể tương tác được nữa
        this.enabled = false;
    }
}
