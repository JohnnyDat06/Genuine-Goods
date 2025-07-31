using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    // Kéo thả asset con trỏ chuột của bro vào đây trong Inspector
    public Texture2D cursorTexture;

    // "Điểm nóng" của con trỏ, tức là cái điểm dùng để click á.
    // Ví dụ: ngay đầu mũi tên. (0,0) là góc trên bên trái của ảnh.
    public Vector2 hotSpot = Vector2.zero;

    // Dùng Awake() hoặc Start() đều được, nó sẽ chạy 1 lần khi game bắt đầu
    void Start()
    {
        // Dùng hàm này để set con trỏ mới
        // CursorMode.Auto sẽ để Unity tự quyết định dùng hardware hay software cursor cho tối ưu
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }

    // Bonus: Khi object này bị hủy hoặc đổi scene, trả lại con trỏ mặc định
    void OnDestroy()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}