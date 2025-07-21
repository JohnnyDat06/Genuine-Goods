using UnityEngine;
using TMPro; // << THÊM DÒNG NÀY VÀO ĐẦU SCRIPT

public class ProximityInteractor : MonoBehaviour
{
    [Header("Đối Tượng Cần Kéo Vào")]
    [Tooltip("Kéo GameObject của Player vào đây")]
    public Transform playerTransform;

    [Tooltip("Kéo GameObject chứa PuzzleManager vào đây")]
    public PuzzleManager puzzleManager;

    [Tooltip("Kéo đối tượng Text UI vào đây")] // Dòng mới
    public GameObject interactionTextUI; // << THÊM BIẾN NÀY

    [Header("Thiết Lập Tương Tác")]
    [Tooltip("Khoảng cách tối đa để có thể tương tác (tính bằng mét)")]
    public float interactionDistance = 3f;

    private bool canInteract = false;

    // Tắt text đi khi game bắt đầu để cho chắc
    void Start()
    {
        if (interactionTextUI != null)
        {
            interactionTextUI.SetActive(false);
        }
    }

    void Update()
    {
        // Kiểm tra null để tránh lỗi
        if (playerTransform == null || puzzleManager == null)
        {
            return;
        }

        // 1. Tính khoảng cách và xác định có thể tương tác không
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        canInteract = (distance <= interactionDistance);

        // 2. LOGIC MỚI: Chỉ hiện text "Nhấn F" khi ở trong tầm VÀ panel câu đố đang tắt
        if (canInteract && !puzzleManager.puzzlePanel.activeSelf)
        {
            interactionTextUI.SetActive(true);
        }
        else
        {
            interactionTextUI.SetActive(false);
        }

        // 3. LOGIC MỚI: Bật/tắt panel bằng phím F
        if (canInteract && Input.GetKeyDown(KeyCode.F))
        {
            // Kiểm tra xem panel đang bật hay tắt
            if (puzzleManager.puzzlePanel.activeSelf)
            {
                // Nếu đang bật -> thì gọi lệnh tắt nó đi
                puzzleManager.HidePuzzle();
            }
            else
            {
                // Nếu đang tắt -> thì gọi lệnh bật nó lên
                puzzleManager.ShowPuzzle();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}