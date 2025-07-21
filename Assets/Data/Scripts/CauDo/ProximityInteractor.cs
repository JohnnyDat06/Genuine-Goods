using UnityEngine;

public class ProximityInteractor : MonoBehaviour
{
    [Header("Đối Tượng Cần Kéo Vào")]
    [Tooltip("Kéo GameObject của Player vào đây")]
    public Transform playerTransform;

    [Tooltip("Kéo GameObject chứa PuzzleManager vào đây")]
    public PuzzleManager puzzleManager;

    [Header("Thiết Lập Tương Tác")]
    [Tooltip("Khoảng cách tối đa để có thể tương tác (tính bằng mét)")]
    public float interactionDistance = 3f;

    private bool canInteract = false;

    void Update()
    {
        // Luôn luôn kiểm tra null để tránh lỗi
        if (playerTransform == null || puzzleManager == null)
        {
            return; // Nếu chưa kéo Player hoặc PuzzleManager vào thì không làm gì cả
        }

        // 1. Tính toán khoảng cách giữa object này (cửa) và người chơi
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // 2. Kiểm tra xem người chơi có ở trong tầm tương tác không
        if (distance <= interactionDistance)
        {
            canInteract = true;
            // Chỗ này bro có thể hiện lên text "Nhấn F" để người chơi biết
        }
        else
        {
            canInteract = false;
            // Ẩn text "Nhấn F" đi
        }

        // 3. Nếu có thể tương tác và người chơi nhấn F
        if (canInteract && Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Đã nhấn F trong tầm! Mở câu đố...");
            puzzleManager.ShowPuzzle();
        }
    }

    // (Optional) Vẽ một vòng tròn gizmo trong Scene view để thấy tầm tương tác
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}