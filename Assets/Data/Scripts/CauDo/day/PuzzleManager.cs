//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class PuzzleManager : MonoBehaviour
//{
//    public static PuzzleManager instance;

//    [Tooltip("Kéo Panel giải đố vào đây")]
//    public GameObject puzzlePanel;

//    [Tooltip("Tổng số cặp dây cần nối (VD: 4 cặp đỏ, vàng, cam, lục)")]
//    public int totalPairs = 4;

//    [Tooltip("Tên của màn chơi tiếp theo")]
//    public string nextSceneName;

//    private int connectedPairs = 0;
//    private bool isSolved = false; //  biết câu đố đã giải xong chưa

//    void Awake()
//    {
//        if (instance == null)
//        {
//            instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    void Start()
//    {
//        if (puzzlePanel != null)
//        {
//            puzzlePanel.SetActive(false);
//        }
//    }


//    void Update()
//    {

//    }


//    public void OnPairConnected()
//    {
//        if (isSolved) return; // Nếu giải rồi thì không đếm nữa

//        connectedPairs++;
//        Debug.Log("Đã nối thành công " + connectedPairs + "/" + totalPairs + " cặp.");

//        if (connectedPairs >= totalPairs)
//        {
//            isSolved = true; // Đánh dấu đã giải xong
//            WinPuzzle();
//        }
//    }

//    public void ShowPuzzle()
//    {
//        // Chỉ hiện panel lên nếu câu đố chưa được giải
//        if (puzzlePanel != null && !isSolved)
//        {
//            puzzlePanel.SetActive(true);

//        }
//    }


//    public void HidePuzzle()
//    {
//        if (puzzlePanel != null)
//        {
//            puzzlePanel.SetActive(false);

//        }
//    }


//    private void WinPuzzle()
//    {
//        Debug.Log("Yay! Đã giải xong câu đố!");


//        Invoke("LoadNextScene", 1.5f); 
//    }


//    private void LoadNextScene()
//    {
//        if (!string.IsNullOrEmpty(nextSceneName))
//        {
//            SceneManager.LoadScene(nextSceneName);
//        }
//        else
//        {
//            Debug.LogWarning("Chưa set tên cho màn kế tiếp kìa bro!");
//        }
//    }
//    //public void ResetPuzzle()
//    //{
//    //    Debug.Log("Sai rồi! Nối lại từ đầu nhé.");

//    //    // 1. Reset lại bộ đếm
//    //    connectedPairs = 0;

//    //    // 2. Tìm và PHÁ HỦY hết tất cả các dây đã vẽ
//    //    GameObject[] lines = GameObject.FindGameObjectsWithTag("PuzzleLine");
//    //    foreach (GameObject line in lines)
//    //    {
//    //        Destroy(line); // << QUAY LẠI DÙNG DESTROY
//    //    }

//    //    // 3. Tìm và bật lại tất cả các điểm nối (giữ nguyên)
//    //    MatchItem[] items = GetComponentsInChildren<MatchItem>(true);
//    //    foreach (MatchItem item in items)
//    //    {
//    //        item.enabled = true;
//    //    }
//    //}
//    public void ResetPuzzle()
//    {
//        Debug.Log("--- BẮT ĐẦU RESET ---");


//        connectedPairs = 0;

//        // 2. Tìm và PHÁ HỦY hết tất cả các dây đã vẽ
//        GameObject[] lines = GameObject.FindGameObjectsWithTag("PuzzleLine");
//        Debug.Log("Đang phá hủy " + lines.Length + " dây nối.");
//        foreach (GameObject line in lines)
//        {
//            Destroy(line);
//        }

//        // 3. Tìm và bật lại tất cả các điểm nối
//        MatchItem[] items = GetComponentsInChildren<MatchItem>(true);
//        Debug.Log("Tìm thấy " + items.Length + " điểm nối để reset.");
//        foreach (MatchItem item in items)
//        {
//            Debug.Log("Đang bật lại điểm nối: " + item.gameObject.name);
//            item.enabled = true;
//        }
//        Debug.Log("--- RESET HOÀN TẤT ---");
//    }

//}
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance;

    public int totalPairs = 4;
    public string nextSceneName;

    private Animator puzzleAnimator;
    private int connectedPairs = 0;
    private bool isSolved = false;

    void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
        puzzleAnimator = GetComponent<Animator>();
    }

    // --- HÀM BẬT/TẮT MỚI ---
    public void TogglePuzzle()
    {
        if (puzzleAnimator != null && !isSolved)
        {
            // Lấy trạng thái hiện tại của biến isShown
            bool currentState = puzzleAnimator.GetBool("isShown");
            // Đặt giá trị ngược lại để bật hoặc tắt
            puzzleAnimator.SetBool("isShown", !currentState);
        }
    }

    // --- HÀM MỚI ĐỂ KIỂM TRA TRẠNG THÁI PANEL ---
    public bool IsPuzzleShown()
    {
        // Kiểm tra xem panel có đang trong trạng thái hiện hay không
        return puzzleAnimator.GetBool("isShown");
    }

    // --- Các hàm logic game khác giữ nguyên ---
    public void OnPairConnected() { if (isSolved) return; connectedPairs++; if (connectedPairs >= totalPairs) { isSolved = true; WinPuzzle(); } }
    private void WinPuzzle() { Invoke("LoadNextScene", 1.5f); }
    private void LoadNextScene() { if (!string.IsNullOrEmpty(nextSceneName)) { SceneManager.LoadScene(nextSceneName); } }
    public void ResetPuzzle() { connectedPairs = 0; GameObject[] lines = GameObject.FindGameObjectsWithTag("PuzzleLine"); foreach (GameObject line in lines) { Destroy(line); } MatchItem[] items = GetComponentsInChildren<MatchItem>(true); foreach (MatchItem item in items) { item.enabled = true; } }
}