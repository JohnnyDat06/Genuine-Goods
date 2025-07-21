using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance;

    [Tooltip("Kéo Panel giải đố vào đây")]
    public GameObject puzzlePanel;

    [Tooltip("Tổng số cặp dây cần nối (VD: 4 cặp đỏ, vàng, cam, lục)")]
    public int totalPairs = 4;

    [Tooltip("Tên của màn chơi tiếp theo")]
    public string nextSceneName;

    private int connectedPairs = 0;
    private bool isSolved = false; // Thêm biến cờ để biết câu đố đã giải xong chưa

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);
        }
    }

    // --- THÊM HÀM UPDATE NÀY VÀO ---
    void Update()
    {
        
    }
    // --------------------------------

    public void OnPairConnected()
    {
        if (isSolved) return; // Nếu giải rồi thì không đếm nữa

        connectedPairs++;
        Debug.Log("Đã nối thành công " + connectedPairs + "/" + totalPairs + " cặp.");

        if (connectedPairs >= totalPairs)
        {
            isSolved = true; // Đánh dấu đã giải xong
            WinPuzzle();
        }
    }

    public void ShowPuzzle()
    {
        // Chỉ hiện panel lên nếu câu đố chưa được giải
        if (puzzlePanel != null && !isSolved)
        {
            puzzlePanel.SetActive(true);
            // Chỗ này có thể thêm code để khóa di chuyển của Player nếu muốn
        }
    }

    // --- THÊM HÀM HIDEPUZZLE NÀY VÀO ---
    public void HidePuzzle()
    {
        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);
            // Chỗ này có thể thêm code để mở khóa di chuyển cho Player
        }
    }
    // -----------------------------------

    private void WinPuzzle()
    {
        Debug.Log("Yay! Đã giải xong câu đố!");

        // Chờ một chút rồi mới chuyển màn để người chơi thấy họ đã thắng
        Invoke("LoadNextScene", 1.5f); // Chờ 1.5 giây
    }

    // Hàm để chuyển màn, được gọi bởi Invoke
    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Chưa set tên cho màn kế tiếp kìa bro!");
        }
    }
}