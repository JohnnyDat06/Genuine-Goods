using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PushableBox : MonoBehaviour
{
    [Header("Tương tác")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float holdDuration = 1f;

    [Header("Di chuyển & Vật lý")]
    [Tooltip("Tốc độ di chuyển khi người chơi đang đẩy/kéo hộp.")]
    [SerializeField] private float pushPullSpeed = 3f;

    [Header("UI Tương tác")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private Image progressRingImage;
    [SerializeField] private Vector3 promptOffset = new Vector3(0, 1.5f, 0);

    [Header("Âm thanh")]
    [SerializeField] private AudioClip draggingSound;
    private AudioSource audioSource;

    // Các biến private
    private PlayerController playerController;
    private Rigidbody2D playerRigidbody;
    private Rigidbody2D boxRigidbody;
    private bool playerIsNearby = false;
    private bool isBeingHeld = false;
    private float holdTimer = 0f;
    private float horizontalInput;
    private bool didFlipPlayer = false; // <-- BIẾN MỚI: Để nhớ nếu đã lật người chơi

    void Awake()
    {
        boxRigidbody = GetComponent<Rigidbody2D>();
        boxRigidbody.bodyType = RigidbodyType2D.Dynamic;
        // Luôn khóa xoay để hộp không bị lật
        boxRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Thiết lập khối lượng lớn để người chơi không thể đẩy bằng thân mình
        // Bạn có thể chỉnh giá trị này trong Inspector
        if (boxRigidbody.mass < 100)
        {
            boxRigidbody.mass = 1000;
        }

        if (TryGetComponent(out audioSource) && draggingSound != null)
        {
            audioSource.clip = draggingSound;
            audioSource.loop = true;
        }

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

    void FixedUpdate()
    {
        if (isBeingHeld)
        {
            // Trực tiếp điều khiển vận tốc của cả hai đối tượng
            Vector2 moveVelocity = new Vector2(horizontalInput * pushPullSpeed, boxRigidbody.velocity.y);
            playerRigidbody.velocity = moveVelocity;
            boxRigidbody.velocity = moveVelocity;

            HandleSound();
        }
    }

    private void HandleInput()
    {
        if (!isBeingHeld)
        {
            if (Input.GetKey(interactKey))
            {
                holdTimer += Time.deltaTime;
                if (progressRingImage != null) progressRingImage.fillAmount = holdTimer / holdDuration;
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
            if (Input.GetKeyDown(interactKey) || Input.GetMouseButtonDown(1))
            {
                Release();
            }
        }
    }

    private void Grab()
    {
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
        isBeingHeld = true;
        playerController.enabled = false;

        // --- BỔ SUNG: LOGIC LẬT NGƯỜI CHƠI ---
        // Lật người chơi nếu cần để đối mặt với hộp
        Transform playerTransform = playerController.transform;
        bool playerIsOnTheLeft = playerTransform.position.x < transform.position.x;
        // Giả sử người chơi quay mặt sang phải khi transform.right.x > 0
        bool playerIsFacingRight = playerTransform.right.x > 0;
        didFlipPlayer = false;

        // Nếu người chơi ở bên trái nhưng đang quay mặt sang trái -> lật lại
        if (playerIsOnTheLeft && !playerIsFacingRight)
        {
            FlipPlayer(playerTransform);
            didFlipPlayer = true;
        }
        // Nếu người chơi ở bên phải nhưng đang quay mặt sang phải -> lật lại
        else if (!playerIsOnTheLeft && playerIsFacingRight)
        {
            FlipPlayer(playerTransform);
            didFlipPlayer = true;
        }
        // ------------------------------------
    }

    private void Release()
    {
        isBeingHeld = false;
        horizontalInput = 0;

        // --- BỔ SUNG: LOGIC LẬT NGƯỜI CHƠI VỀ HƯỚNG CŨ ---
        // Lật người chơi lại vị trí cũ nếu cần
        if (didFlipPlayer)
        {
            FlipPlayer(playerController.transform);
            didFlipPlayer = false;
        }
        // -----------------------------------------------

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (interactionPrompt != null && playerIsNearby) interactionPrompt.SetActive(true);
        if (progressRingImage != null) progressRingImage.fillAmount = 0;
    }

    // --- HÀM MỚI: ĐỂ LẬT NGƯỜI CHƠI ---
    private void FlipPlayer(Transform playerTransform)
    {
        // Xoay người chơi 180 độ quanh trục Y
        playerTransform.Rotate(0f, 180f, 0f);
    }
    // ------------------------------------

    private void HandleSound()
    {
        if (audioSource == null) return;
        if (Mathf.Abs(horizontalInput) > 0.1f && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else if (Mathf.Abs(horizontalInput) < 0.1f && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void UpdatePromptPosition()
    {
        if (interactionPrompt != null && interactionPrompt.activeSelf)
        {
            Vector3 worldPosition = transform.position + promptOffset;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            interactionPrompt.transform.position = screenPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = true;
            playerController = other.GetComponent<PlayerController>();
            playerRigidbody = other.GetComponent<Rigidbody2D>();
            if (interactionPrompt != null && !isBeingHeld)
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
            }
        }
    }
}
