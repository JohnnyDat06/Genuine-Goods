using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [Tooltip("Kéo GameObject chứa script PuzzleManager vào đây")]
    public PuzzleManager puzzleManager;

    private bool canInteract = false;

    // Hàm này được gọi khi có một đối tượng khác đi vào trigger
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem đối tượng đó có phải là Player không
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            Debug.Log("Đã ở gần cửa, nhấn F để giải đố.");
            // Có thể hiện một UI nhỏ báo "Nhấn F" ở đây cho chuyên nghiệp
        }
    }

    // Hàm này được gọi khi đối tượng đi ra khỏi trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
        }
    }

    void Update()
    {
        // Nếu người chơi đang ở trong vùng và nhấn phím F
        if (canInteract && Input.GetKeyDown(KeyCode.F))
        {
            if (puzzleManager != null)
            {
                // Gọi hàm bật panel giải đố từ Manager
                puzzleManager.ShowPuzzle();
            }
            else
            {
                Debug.LogError("Chưa kéo PuzzleManager vào script DoorInteraction kìa bro!");
            }
        }
    }
}