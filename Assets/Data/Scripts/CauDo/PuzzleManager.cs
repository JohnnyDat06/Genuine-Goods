using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện để chuyển màn

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance; // Dùng singleton để các script khác dễ gọi

    [Tooltip("Kéo Panel giải đố vào đây")]
    public GameObject puzzlePanel;

    [Tooltip("Tổng số cặp dây cần nối (VD: 4 cặp đỏ, vàng, cam, lục)")]
    public int totalPairs = 4;

    [Tooltip("Tên của màn chơi tiếp theo")]
    public string nextSceneName;

    private int connectedPairs = 0;

    void Awake()
    {
        // Thiết lập singleton
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
        // Mặc định ẩn panel giải đố đi khi bắt đầu game
        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);
        }
    }

    // Hàm này sẽ được gọi từ script MatchItem mỗi khi nối thành công 1 cặp
    public void OnPairConnected()
    {
        connectedPairs++;
        Debug.Log("Đã nối thành công " + connectedPairs + "/" + totalPairs + " cặp.");

        // Kiểm tra xem đã thắng chưa
        if (connectedPairs >= totalPairs)
        {
            WinPuzzle();
        }
    }

    // Hàm để bật panel giải đố lên
    public void ShowPuzzle()
    {
        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(true);
            // Có thể thêm logic khóa di chuyển của người chơi ở đây nếu muốn
        }
    }

    // Xử lý khi thắng
    private void WinPuzzle()
    {
        Debug.Log("Yay! Đã giải xong câu đố!");

        // Ẩn panel đi
        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);
        }

        // Chuyển sang màn tiếp theo
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