//using UnityEngine;

//public class PaperInteraction : MonoBehaviour
//{
//    [Header("UI Elements")]
//    [Tooltip("Kéo Animator của PaperUI vào đây")]
//    [SerializeField] private Animator paperAnimator;
//    [Tooltip("UI hiện chữ 'Nhấn F để đọc' (Tùy chọn)")]
//    [SerializeField] private GameObject interactionPrompt;

//    [Header("Player Settings")]
//    [Tooltip("Kéo GameObject của Player vào đây")]
//    [SerializeField] private GameObject playerObject;
//    [Header("Sound Effects")]
//    [Tooltip("Âm thanh khi mở tờ giấy")]
//    [SerializeField] private AudioClip openSound;
//    [Tooltip("Âm thanh khi đóng tờ giấy (tùy chọn)")]
//    [SerializeField] private AudioClip closeSound;
//    private AudioSource paperAudioSource;
//    // Tên script điều khiển di chuyển của Player. Bro hãy đổi "PlayerMovement"
//    // thành tên script thực tế của mình ở dòng 40 nhé.
//    private MonoBehaviour playerMovementScript;

//    private bool playerInRange = false;
//    private bool isPaperOpen = false;

//    void Start()
//    {
//        if (paperAnimator != null)
//        {
//            paperAudioSource = paperAnimator.GetComponent<AudioSource>();
//        }
//        if (interactionPrompt != null)
//        {
//            interactionPrompt.SetActive(false);
//        }

//        // Tự động tìm script di chuyển trên Player
//        if (playerObject != null)
//        {
//            // !!! THAY "PlayerMovement" BẰNG TÊN SCRIPT DI CHUYỂN CỦA BRO !!!
//            playerMovementScript = playerObject.GetComponent("PlayerController") as MonoBehaviour;
//            if (playerMovementScript == null)
//            {
//                Debug.LogError("LỖI: Không tìm thấy script 'PlayerMovement' trên Player. Hãy kiểm tra lại tên script trong code nhé!");
//            }
//        }
//        else
//        {
//            Debug.LogError("LỖI: Chưa kéo Player vào ô 'Player Object' trong Inspector!");
//        }
//    }

//    void Update()
//    {
//        // Chỉ check input khi người chơi ở trong tầm và không có giấy nào đang mở
//        if (playerInRange && Input.GetKeyDown(KeyCode.F))
//        {
//            if (!isPaperOpen)
//            {
//                OpenPaper();
//            }
//            else
//            {
//                ClosePaper();
//            }
//        }
//    }

//    void OpenPaper()
//    {
//        isPaperOpen = true;
//        if (paperAudioSource != null && openSound != null)
//        {
//            paperAudioSource.PlayOneShot(openSound);
//        }


//        // Vô hiệu hóa di chuyển của người chơi
//        if (playerMovementScript != null)
//        {
//            playerMovementScript.enabled = false;
//        }

//        paperAnimator.gameObject.SetActive(true);
//        paperAnimator.SetTrigger("Open");
//        if (interactionPrompt != null) interactionPrompt.SetActive(false);
//    }

//    void ClosePaper()
//    {
//        isPaperOpen = false;
//        if (paperAudioSource != null && closeSound != null)
//        {
//            paperAudioSource.PlayOneShot(closeSound);
//        }


//        // Kích hoạt lại di chuyển của người chơi
//        if (playerMovementScript != null)
//        {
//            playerMovementScript.enabled = true;
//        }

//        paperAnimator.SetTrigger("Close");
//        // Hiện lại thông báo nếu người chơi vẫn còn trong tầm
//        if (interactionPrompt != null && playerInRange)
//        {
//            interactionPrompt.SetActive(true);
//        }
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        // Dùng gameObject để so sánh trực tiếp, an toàn hơn dùng Tag
//        if (other.gameObject == playerObject)
//        {
//            playerInRange = true;
//            if (interactionPrompt != null && !isPaperOpen)
//            {
//                interactionPrompt.SetActive(true);
//            }
//        }
//    }

//    private void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.gameObject == playerObject)
//        {
//            playerInRange = false;
//            if (interactionPrompt != null)
//            {
//                interactionPrompt.SetActive(false);
//            }

//            // Nếu người chơi đi xa mà giấy vẫn mở, tự động đóng lại
//            if (isPaperOpen)
//            {
//                ClosePaper();
//            }
//        }
//    }
//}

////////////////////////////////////////////////////////////////////////////////////////////////////////
//using UnityEngine;

//public class PaperInteraction : MonoBehaviour
//{
//    private static PaperInteraction currentlyOpenPaper;

//    [Header("UI Elements")]
//    [SerializeField] private Animator paperAnimator;
//    [SerializeField] private GameObject interactionPrompt;

//    [Header("Player Settings")]
//    [SerializeField] private GameObject playerObject;

//    [Header("Sound Effects")]
//    [SerializeField] private AudioClip openSound;
//    [SerializeField] private AudioClip closeSound;

//    private AudioSource paperAudioSource;
//    private MonoBehaviour playerMovementScript;
//    private bool playerInRange = false;
//    private bool isPaperOpen = false;
//    private bool hasBeenCollected = false;

//    void Start()
//    {
//        if (paperAnimator != null) paperAudioSource = paperAnimator.GetComponent<AudioSource>();
//        if (interactionPrompt != null) interactionPrompt.SetActive(false);
//        if (playerObject != null) playerMovementScript = playerObject.GetComponent("PlayerController") as MonoBehaviour;
//    }

//    void Update()
//    {
//        // --- ĐÂY LÀ PHẦN SỬA LỖI QUAN TRỌNG NHẤT ---
//        if (playerInRange && Input.GetKeyDown(KeyCode.F))
//        {
//            // Điều kiện để MỞ: Phải chưa mở VÀ chưa từng được thu thập.
//            if (!isPaperOpen && !hasBeenCollected)
//            {
//                OpenPaper();
//            }
//            // Điều kiện để ĐÓNG: Chỉ cần nó đang mở là được.
//            else if (isPaperOpen)
//            {
//                ClosePaper();
//            }
//        }
//        // ------------------------------------------
//    }

//    void OpenPaper()
//    {
//        if (currentlyOpenPaper != null && currentlyOpenPaper != this)
//        {
//            currentlyOpenPaper.ClosePaper();
//        }

//        isPaperOpen = true;
//        currentlyOpenPaper = this;

//        if (paperAudioSource != null && openSound != null) paperAudioSource.PlayOneShot(openSound);
//        if (playerMovementScript != null) playerMovementScript.enabled = false;

//        paperAnimator.gameObject.SetActive(true);
//        paperAnimator.SetTrigger("Open");
//        if (interactionPrompt != null) interactionPrompt.SetActive(false);

//        if (!hasBeenCollected)
//        {
//            DocumentManager.instance.CollectPaper(this);
//            hasBeenCollected = true;
//        }
//    }

//    void ClosePaper()
//    {
//        isPaperOpen = false;
//        currentlyOpenPaper = null;

//        if (paperAudioSource != null && closeSound != null) paperAudioSource.PlayOneShot(closeSound);
//        if (playerMovementScript != null) playerMovementScript.enabled = true;

//        paperAnimator.SetTrigger("Close");

//        // Sửa logic nhỏ: Chỉ hiện lại prompt nếu người chơi còn trong tầm VÀ chưa thu thập
//        if (interactionPrompt != null && playerInRange && !hasBeenCollected)
//        {
//            interactionPrompt.SetActive(true);
//        }
//    }

//    public void ShowFromManager()
//    {
//        isPaperOpen = true;
//        if (playerMovementScript != null) playerMovementScript.enabled = false;
//        paperAnimator.gameObject.SetActive(true);
//        paperAnimator.SetTrigger("Open");
//    }

//    public void CloseFromManager()
//    {
//        isPaperOpen = false;
//        if (playerMovementScript != null) playerMovementScript.enabled = true;
//        paperAnimator.SetTrigger("Close");
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        if (other.gameObject == playerObject)
//        {
//            playerInRange = true;
//            // Sửa logic nhỏ: Chỉ hiện prompt nếu chưa mở VÀ chưa thu thập
//            if (interactionPrompt != null && !isPaperOpen && !hasBeenCollected)
//            {
//                interactionPrompt.SetActive(true);
//            }
//        }
//    }

//    private void OnTriggerExit2D(Collider2D other)
//    {
//        if (other.gameObject == playerObject)
//        {
//            playerInRange = false;
//            if (interactionPrompt != null)
//            {
//                interactionPrompt.SetActive(false);
//            }
//            if (isPaperOpen)
//            {
//                ClosePaper();
//            }
//        }
//    }
//}
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PaperInteraction : MonoBehaviour
{
    private static PaperInteraction currentlyOpenPaper;
    private SpriteRenderer inGameSprite;

    [Header("UI Elements")]
    [SerializeField] private Animator paperAnimator;
    [SerializeField] private GameObject interactionPrompt;

    [Header("Player Settings")]
    [SerializeField] private GameObject playerObject;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    private AudioSource paperAudioSource;
    private MonoBehaviour playerMovementScript;
    private bool playerInRange = false;
    private bool isPaperOpen = false;
    private bool hasBeenCollected = false;

    void Start()
    {
        inGameSprite = GetComponent<SpriteRenderer>();
        if (paperAnimator != null) paperAudioSource = paperAnimator.GetComponent<AudioSource>();
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
        if (playerObject != null) playerMovementScript = playerObject.GetComponent("PlayerController") as MonoBehaviour;
    }

    void Update()
    {
        // --- LOGIC ĐÃ SỬA LẠI ĐỂ TRÁNH LỖI ---
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            // Nếu giấy đang mở, hành động duy nhất là ĐÓNG.
            if (isPaperOpen)
            {
                ClosePaper();
            }
            // Ngược lại, nếu giấy chưa mở VÀ chưa được thu thập, hành động là MỞ.
            else if (!hasBeenCollected)
            {
                OpenPaper();
            }
        }
        // ------------------------------------
    }

    void OpenPaper()
    {
        if (currentlyOpenPaper != null && currentlyOpenPaper != this)
        {
            currentlyOpenPaper.ClosePaper();
        }

        isPaperOpen = true;
        currentlyOpenPaper = this;
        PlayOpenSound();

        if (playerMovementScript != null) playerMovementScript.enabled = false;
        paperAnimator.gameObject.SetActive(true);
        paperAnimator.SetTrigger("Open");
        if (interactionPrompt != null) interactionPrompt.SetActive(false);

        // Chỉ đánh dấu là đã thu thập, chưa làm gì khác.
        if (!hasBeenCollected)
        {
            DocumentManager.instance.CollectPaper(this);
            hasBeenCollected = true;
        }
    }

    void ClosePaper()
    {
        isPaperOpen = false;
        currentlyOpenPaper = null;
        PlayCloseSound();

        if (playerMovementScript != null) playerMovementScript.enabled = true;
        paperAnimator.SetTrigger("Close");

        // --- LOGIC ẨN VẬT THỂ ĐƯỢC CHUYỂN VỀ ĐÂY ---
        // Nếu tờ giấy này đã được thu thập, đây là lúc làm nó biến mất vĩnh viễn.
        if (hasBeenCollected)
        {
            // Tắt hình ảnh
            if (inGameSprite != null)
            {
                inGameSprite.enabled = false;
            }
            // Tắt va chạm để không tương tác lại được nữa
            GetComponent<Collider2D>().enabled = false;
            // Tắt luôn cả prompt để chắc chắn
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
        // -------------------------------------------
    }

    // Các hàm public cho DocumentManager gọi vẫn giữ nguyên...
    public void PlayOpenSound()
    {
        if (paperAudioSource != null && openSound != null) paperAudioSource.PlayOneShot(openSound);
    }
    public void PlayCloseSound()
    {
        if (paperAudioSource != null && closeSound != null) paperAudioSource.PlayOneShot(closeSound);
    }
    public void ShowFromManager()
    {
        isPaperOpen = true;
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        paperAnimator.gameObject.SetActive(true);
        paperAnimator.SetTrigger("Open");
    }
    public void CloseFromManager()
    {
        isPaperOpen = false;
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        paperAnimator.SetTrigger("Close");
    }

    // Các hàm OnTrigger vẫn giữ nguyên...
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == playerObject)
        {
            playerInRange = true;
            if (interactionPrompt != null && !isPaperOpen && !hasBeenCollected)
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
            if (isPaperOpen)
            {
                ClosePaper();
            }
        }
    }
}
