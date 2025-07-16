using UnityEngine;
using UnityEngine.UI;

public class PushableBox : MonoBehaviour
{
    [Header("Tương tác")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float holdDuration = 2f;

    [Header("Di chuyển & Vị trí")]
    [SerializeField] private float pushPullSpeed = 2f;
    [SerializeField] private Vector2 carryOffset = new Vector2(1f, 0.5f);

    [Header("UI Tương tác")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private Image progressRingImage;
    [SerializeField] private Vector3 promptOffset = new Vector3(0, 1.5f, 0);

    // --- BIẾN MỚI CHO ÂM THANH ---
    [Header("Âm thanh")]
    [SerializeField] private AudioClip draggingSound;
    private AudioSource audioSource;
    // ----------------------------

    // Các biến private
    private PlayerController playerController;
    private Rigidbody2D playerRigidbody;
    private Rigidbody2D boxRigidbody;
    private bool playerIsNearby = false;
    private bool isBeingHeld = false;
    private float holdTimer = 0f;
    private float horizontalInput;
    private bool didFlipPlayer = false;

    void Awake()
    {
        boxRigidbody = GetComponent<Rigidbody2D>();
        boxRigidbody.bodyType = RigidbodyType2D.Static;

        // --- LẤY COMPONENT AUDIOSOURCE ---
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null && draggingSound != null)
        {
            audioSource.clip = draggingSound;
        }
        // -------------------------------

        if (interactionPrompt != null) interactionPrompt.SetActive(false);
    }

    void Update()
    {
        UpdatePromptPosition();
        if (playerIsNearby)
        {
            HandleInput();
        }
    }

    void UpdatePromptPosition()
    {
        if (interactionPrompt != null && interactionPrompt.activeSelf)
        {
            Vector3 worldPosition = transform.position + promptOffset;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            interactionPrompt.transform.position = screenPosition;
        }
    }

    void FixedUpdate()
    {
        if (isBeingHeld)
        {
            // Di chuyển người chơi
            playerRigidbody.velocity = new Vector2(horizontalInput * pushPullSpeed, 0f);

            // --- LOGIC ĐIỀU KHIỂN ÂM THANH ---
            if (audioSource != null)
            {
                // Nếu người chơi đang di chuyển và âm thanh chưa phát -> Bật âm thanh
                if (horizontalInput != 0 && !audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                // Nếu người chơi đã dừng và âm thanh vẫn đang phát -> Tắt âm thanh
                else if (horizontalInput == 0 && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }
    }

    private void HandleInput()
    {
        if (!isBeingHeld)
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
                    Grab();
                    holdTimer = 0f;
                }
            }
            if (Input.GetKeyUp(interactKey))
            {
                holdTimer = 0f;
                if (progressRingImage != null) progressRingImage.fillAmount = 0;
            }
        }
        else
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            if (Input.GetMouseButtonDown(1))
            {
                Release();
            }
        }
    }

    private void Grab()
    {
        // ... (Code của Grab giữ nguyên)
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
        Transform playerTransform = playerController.transform;
        bool playerIsOnTheLeft = playerTransform.position.x < transform.position.x;
        bool playerIsFacingRight = playerTransform.right.x > 0;
        didFlipPlayer = false;
        if (playerIsOnTheLeft && !playerIsFacingRight) { FlipPlayer(playerTransform); didFlipPlayer = true; }
        else if (!playerIsOnTheLeft && playerIsFacingRight) { FlipPlayer(playerTransform); didFlipPlayer = true; }
        isBeingHeld = true;
        boxRigidbody.bodyType = RigidbodyType2D.Kinematic;
        boxRigidbody.velocity = Vector2.zero;
        playerController.enabled = false;
        float finalFacingDirection = playerTransform.right.x > 0 ? 1 : -1;
        Vector3 targetPosition = playerTransform.position + new Vector3(carryOffset.x * finalFacingDirection, carryOffset.y, 0);
        transform.position = targetPosition;
        transform.SetParent(playerTransform);
    }

    private void Release()
    {
        // --- DỪNG ÂM THANH KHI THẢ THÙNG ---
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        // ---------------------------------

        // ... (Code còn lại của Release giữ nguyên)
        if (interactionPrompt != null && playerIsNearby) interactionPrompt.SetActive(true);
        if (progressRingImage != null) progressRingImage.fillAmount = 0;
        isBeingHeld = false;
        horizontalInput = 0;
        transform.SetParent(null);
        boxRigidbody.bodyType = RigidbodyType2D.Static;
        if (didFlipPlayer)
        {
            FlipPlayer(playerController.transform);
            didFlipPlayer = false;
        }
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }

    private void FlipPlayer(Transform playerTransform)
    {
        playerTransform.Rotate(0f, 180f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = true;
            playerController = other.GetComponent<PlayerController>();
            playerRigidbody = other.GetComponent<Rigidbody2D>();
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
                if (progressRingImage != null) progressRingImage.fillAmount = 0;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isBeingHeld)
            {
                Release();
            }
            playerIsNearby = false;
            playerController = null;
            playerRigidbody = null;
            holdTimer = 0f;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
                if (progressRingImage != null) progressRingImage.fillAmount = 0;
            }
        }
    }
}