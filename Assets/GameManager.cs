using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Kết nối các thành phần")]
    [Tooltip("Kéo đối tượng GameObject có chứa script PatrollingWarningCamera vào đây từ cửa sổ Hierarchy.")]
    public PatrollingWarningCamera securityCamera; // Tham chiếu đến script camera

    void Update()
    {
        // Ví dụ: Khi người dùng nhấn phím "T" trên bàn phím
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Luôn kiểm tra xem đã kết nối với camera chưa để tránh lỗi
            if (securityCamera != null)
            {
                // Gọi hàm public của script camera để thay đổi nội dung cảnh báo
                Debug.Log("Đã nhấn phím T, gửi lệnh thay đổi tin nhắn cảnh báo!");
                securityCamera.SetWarningMessage("!!! CẢNH BÁO TỪ TRUNG TÂM ĐIỀU KHIỂN !!!");
            }
            else
            {
                // Báo lỗi nếu bạn quên gán camera trong Inspector
                Debug.LogError("Chưa gán đối tượng Security Camera cho GameManager trong Inspector!");
            }
        }
    }
}