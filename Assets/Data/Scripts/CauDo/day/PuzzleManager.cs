// PuzzleManager.cs (phiên bản InputField có hiệu ứng gõ chữ)
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using TMPro; // Vẫn cần dùng cái này cho InputField
//using System.Collections;

//public class PuzzleManager : MonoBehaviour
//{
//    public static PuzzleManager instance;

//    [Header("Cài Đặt Puzzle")]
//    public int totalPairs = 4;
//    public string nextSceneName;

//    // --- PHẦN NÀY ĐƯỢC THAY ĐỔI ---
//    [Header("Cài Đặt Khung Gợi Ý")]
//    [Tooltip("Kéo đối tượng InputField - TextMeshPro vào đây")]
//    public TMP_InputField hintInputField;
//    [Tooltip("Tốc độ gõ chữ (số càng nhỏ chữ chạy càng nhanh)")]
//    [SerializeField] private float typingSpeed = 0.05f;
//    // --- HẾT PHẦN THAY ĐỔI ---

//    [Header("Thiết Lập Gợi Ý")]
//    [Tooltip("Nội dung gợi ý KHI ĐÃ GẶP thợ điện")]
//    [TextArea] public string hintWithMessage = "Gợi ý: Hình như ông thợ điện có nhắc gì đó về màu sắc thì phải...";
//    [Tooltip("Nội dung gợi ý KHI CHƯA GẶP thợ điện")]
//    [TextArea] public string hintWithoutMessage = "Gợi ý: Cần phải tìm mật mã hoặc manh mối ở đâu đó trước.";
//    [Tooltip("Số lần nối sai tối đa trước khi hiện gợi ý")]
//    public int wrongAttemptLimit = 3;
//    [Tooltip("Thời gian không tương tác (giây) trước khi hiện gợi ý")]
//    public float idleTimeLimit = 10f;

//    // Biến nội bộ
//    private Animator puzzleAnimator;
//    private Coroutine typingCoroutine;
//    private int connectedPairs = 0;
//    private bool isSolved = false;
//    private int wrongAttempts = 0;
//    private float idleTimer;

//    void Awake()
//    {
//        if (instance == null) { instance = this; }
//        else { Destroy(gameObject); }
//        puzzleAnimator = GetComponent<Animator>();
//    }

//    void Start()
//    {
//        // Bắt đầu thì tắt InputField đi
//        if (hintInputField != null)
//        {
//            hintInputField.gameObject.SetActive(false);
//        }
//        ResetIdleTimer();
//    }

//    void Update()
//    {
//        if (IsPuzzleShown() && !isSolved)
//        {
//            idleTimer -= Time.deltaTime;
//            if (idleTimer <= 0)
//            {
//                ShowHint();
//                ResetIdleTimer();
//            }
//        }
//    }

//    private void ShowHint()
//    {
//        if (hintInputField == null) return;

//        // Xác định nội dung gợi ý
//        string hintToDisplay = (GameManagers.instance != null && GameManagers.instance.hasMetElectrician)
//                                ? hintWithMessage
//                                : hintWithoutMessage;

//        // Bật InputField lên và bắt đầu gõ chữ
//        hintInputField.gameObject.SetActive(true);

//        // Dừng coroutine cũ nếu nó đang chạy để bắt đầu cái mới
//        if (typingCoroutine != null)
//        {
//            StopCoroutine(typingCoroutine);
//        }
//        typingCoroutine = StartCoroutine(TypeHint(hintToDisplay));
//    }

//    // Coroutine để tạo hiệu ứng gõ chữ
//    private IEnumerator TypeHint(string textToType)
//    {
//        hintInputField.text = ""; // Xóa trắng nội dung cũ
//        foreach (char letter in textToType.ToCharArray())
//        {
//            hintInputField.text += letter;
//            yield return new WaitForSeconds(typingSpeed);
//        }

//        // Sau khi gõ xong, chờ 5 giây rồi ẩn đi
//        yield return new WaitForSeconds(5f);
//        HideHint();
//    }

//    // Hàm này giờ sẽ tắt InputField
//    private void HideHint()
//    {
//        if (hintInputField != null)
//        {
//            hintInputField.gameObject.SetActive(false);
//        }
//        // Dừng coroutine luôn để chắc ăn
//        if (typingCoroutine != null)
//        {
//            StopCoroutine(typingCoroutine);
//            typingCoroutine = null;
//        }
//    }

//    private void ResetIdleTimer()
//    {
//        idleTimer = idleTimeLimit;
//    }

//    public void OnPairConnected()
//    {
//        if (isSolved) return;
//        connectedPairs++;
//        wrongAttempts = 0;
//        ResetIdleTimer();
//        HideHint(); // Nối đúng thì tắt gợi ý
//        if (connectedPairs >= totalPairs)
//        {
//            isSolved = true;
//            WinPuzzle();
//        }
//    }

//    public void ResetPuzzle()
//    {
//        wrongAttempts++;
//        ResetIdleTimer();
//        HideHint();
//        connectedPairs = 0;
//        GameObject[] lines = GameObject.FindGameObjectsWithTag("PuzzleLine");
//        foreach (GameObject line in lines) { Destroy(line); }
//        MatchItem[] items = GetComponentsInChildren<MatchItem>(true);
//        foreach (MatchItem item in items) { item.enabled = true; }
//        if (wrongAttempts >= wrongAttemptLimit)
//        {
//            ShowHint();
//            wrongAttempts = 0;
//        }
//    }

//    public void TogglePuzzle()
//    {
//        if (puzzleAnimator != null && !isSolved)
//        {
//            bool currentState = puzzleAnimator.GetBool("isShown");
//            puzzleAnimator.SetBool("isShown", !currentState);
//            if (!currentState)
//            {
//                ResetIdleTimer();
//                HideHint();
//                wrongAttempts = 0;
//            }
//        }
//    }

//    public bool IsPuzzleShown() => puzzleAnimator.GetBool("isShown");
//    private void WinPuzzle() => Invoke("LoadNextScene", 1.5f);
//    private void LoadNextScene() { if (!string.IsNullOrEmpty(nextSceneName)) { SceneManager.LoadScene(nextSceneName); } }
//}
// PuzzleManager.cs (phiên bản có animation popup)
// PuzzleManager.cs (phiên bản sửa lỗi logic đếm & animation)
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance;

    [Header("Cài Đặt Puzzle")]
    public int totalPairs = 4;
    public string nextSceneName;

    [Header("Cài Đặt Khung Gợi Ý")]
    public TMP_InputField hintInputField;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float popupAnimDuration = 0.3f;

    [Header("Thiết Lập Gợi Ý")]
    [TextArea] public string hintWithMessage = "Gợi ý: Hình như ông thợ điện có nhắc gì đó về màu sắc thì phải...";
    [TextArea] public string hintWithoutMessage = "Gợi ý: Cần phải tìm mật mã hoặc manh mối ở đâu đó trước.";
    public int wrongAttemptLimit = 3;
    public float idleTimeLimit = 10f;

    // Biến nội bộ
    private Animator puzzleAnimator;
    private Animator hintAnimator;
    private Coroutine typingCoroutine;
    private int connectedPairs = 0;
    private bool isSolved = false;
    private int wrongAttempts = 0;
    private float idleTimer;

    void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
        puzzleAnimator = GetComponent<Animator>();
        if (hintInputField != null) { hintAnimator = hintInputField.GetComponent<Animator>(); }
    }

    void Start()
    {
        if (hintInputField != null) { hintInputField.gameObject.SetActive(false); }
        ResetIdleTimer();
    }

    void Update()
    {
        if (IsPuzzleShown() && !isSolved && !hintInputField.gameObject.activeSelf)
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
            {
                ShowHint();
                ResetIdleTimer();
            }
        }
    }

    // --- HÀM MỚI ĐỂ ANIMATION GỌI ---
    // Hàm này sẽ được gọi ở frame cuối của animation Close
    public void DeactivateHintObject()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        hintInputField.gameObject.SetActive(false);
    }

    private void ShowHint()
    {
        if (hintInputField == null || hintAnimator == null) return;
        string hintToDisplay = (GameManagers.instance != null && GameManagers.instance.hasMetElectrician) ? hintWithMessage : hintWithoutMessage;

        hintInputField.gameObject.SetActive(true);
        hintAnimator.SetTrigger("Show");

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeHint(hintToDisplay));
    }

    private IEnumerator TypeHint(string textToType)
    {
        yield return new WaitForSeconds(popupAnimDuration);
        hintInputField.text = "";
        foreach (char letter in textToType.ToCharArray())
        {
            hintInputField.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        yield return new WaitForSeconds(5f);
        HideHint();
    }

    private void HideHint()
    {
        if (hintInputField != null && hintInputField.gameObject.activeSelf)
        {
            hintAnimator.SetTrigger("Hide");
        }
    }

    private void ResetIdleTimer() { idleTimer = idleTimeLimit; }

    public void OnPairConnected()
    {
        if (isSolved) return;
        connectedPairs++;
        wrongAttempts = 0; // Reset số lần sai khi nối đúng
        ResetIdleTimer();
        HideHint();
        if (connectedPairs >= totalPairs) { isSolved = true; WinPuzzle(); }
    }

    // --- HÀM RESETPUZZLE ĐƯỢC SỬA LẠI HOÀN TOÀN ---
    public void ResetPuzzle()
    {
        // 1. Dọn dẹp các dây cũ
        connectedPairs = 0;
        GameObject[] lines = GameObject.FindGameObjectsWithTag("PuzzleLine");
        foreach (GameObject line in lines) { Destroy(line); }
        MatchItem[] items = GetComponentsInChildren<MatchItem>(true);
        foreach (MatchItem item in items) { item.enabled = true; }

        // 2. Tăng số lần sai và reset timer
        wrongAttempts++;
        ResetIdleTimer();

        // 3. Chỉ kiểm tra và hành động nếu đủ 3 lần sai
        if (wrongAttempts >= wrongAttemptLimit)
        {
            // Nếu đủ 3 lần, hiện gợi ý và reset bộ đếm về 0
            ShowHint();
            wrongAttempts = 0;
        }
    }

    public void TogglePuzzle()
    {
        if (puzzleAnimator != null && !isSolved)
        {
            bool currentState = puzzleAnimator.GetBool("isShown");
            puzzleAnimator.SetBool("isShown", !currentState);
            if (!currentState)
            {
                ResetIdleTimer();
                HideHint();
                wrongAttempts = 0;
            }
        }
    }

    public bool IsPuzzleShown() => puzzleAnimator.GetBool("isShown");
    private void WinPuzzle() => Invoke("LoadNextScene", 1.0f);
    private void LoadNextScene() { if (!string.IsNullOrEmpty(nextSceneName)) { SceneManager.LoadScene(nextSceneName); } }
}