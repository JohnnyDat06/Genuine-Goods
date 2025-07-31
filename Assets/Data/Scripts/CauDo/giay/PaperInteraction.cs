using UnityEngine;

public class PaperInteraction : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Kéo Animator của PaperUI vào đây")]
    [SerializeField] private Animator paperAnimator;
    [Tooltip("UI hiện chữ 'Nhấn F để đọc' (Tùy chọn)")]
    [SerializeField] private GameObject interactionPrompt;

    [Header("Player Settings")]
    [Tooltip("Kéo GameObject của Player vào đây")]
    [SerializeField] private GameObject playerObject;

    
    private MonoBehaviour playerMovementScript;

    private bool playerInRange = false;
    private bool isPaperOpen = false;

    void Start()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // Tự động tìm script di chuyển trên Player
        if (playerObject != null)
        {
            
            playerMovementScript = playerObject.GetComponent("PlayerController") as MonoBehaviour;
            if (playerMovementScript == null)
            {
                Debug.LogError("LỖI: Không tìm thấy script 'PlayerMovement' trên Player. Hãy kiểm tra lại tên script trong code nhé!");
            }
        }
        else
        {
            Debug.LogError("LỖI: Chưa kéo Player vào ô 'Player Object' trong Inspector!");
        }
    }

    void Update()
    {
        // Chỉ check input khi người chơi ở trong tầm và không có giấy nào đang mở
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (!isPaperOpen)
            {
                OpenPaper();
            }
            else
            {
                ClosePaper();
            }
        }
    }

    void OpenPaper()
    {
        isPaperOpen = true;

        // Vô hiệu hóa di chuyển của người chơi
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        paperAnimator.gameObject.SetActive(true);
        paperAnimator.SetTrigger("Open");
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
    }

    void ClosePaper()
    {
        isPaperOpen = false;

        // Kích hoạt lại di chuyển của người chơi
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        paperAnimator.SetTrigger("Close");
        // Hiện lại thông báo nếu người chơi vẫn còn trong tầm
        if (interactionPrompt != null && playerInRange)
        {
            interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Dùng gameObject để so sánh trực tiếp, an toàn hơn dùng Tag
        if (other.gameObject == playerObject)
        {
            playerInRange = true;
            if (interactionPrompt != null && !isPaperOpen)
            {
                interactionPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == playerObject)
        {
            playerInRange = false;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }

            // Nếu người chơi đi xa mà giấy vẫn mở, tự động đóng lại
            if (isPaperOpen)
            {
                ClosePaper();
            }
        }
    }
}
