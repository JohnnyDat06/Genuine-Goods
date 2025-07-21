using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Connections")]
    [Tooltip("Kéo đối tượng GameObject chứa script PatrollingWarningCamera vào đây.")]
    public PatrollingWarningCamera securityCamera; // Tham chiếu đến script camera

    void Update()
    {
        // Ví dụ: Khi người dùng nhấn phím "T"
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Kiểm tra xem đã kết nối với camera chưa
            if (securityCamera != null)
            {
                // Gọi hàm public của script camera để thay đổi nội dung cảnh báo
                securityCamera.SetWarningMessage("TÍN HIỆU CẢNH BÁO TỪ GAMEMANAGER!");
            }
            else
            {
                Debug.LogError("Chưa gán đối tượng Security Camera cho GameManager trong Inspector!");
            }
        }
    }
}
