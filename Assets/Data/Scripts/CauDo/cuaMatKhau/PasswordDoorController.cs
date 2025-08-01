using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PasswordDoorController : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("Mật khẩu đúng để mở cửa")]
    [SerializeField] private string correctPassword = "1234";

    [Header("Object References")]
    [Tooltip("Kéo Panel chứa ô nhập mật khẩu vào đây")]
    [SerializeField] private GameObject passwordPanel;
    [Tooltip("Kéo ô Input Field vào đây")]
    [SerializeField] private TMP_InputField passwordInputField;
    [Tooltip("Kéo nút bấm xác nhận vào đây")]
    [SerializeField] private Button submitButton;

    // Các biến private để tự quản lý
    private Animator doorAnimator;
    private Animator panelAnimator;
    private Collider2D doorCollider;
    private bool isDoorOpen = false;

    private void Awake()
    {
        // Lấy các component cần thiết lúc bắt đầu
        doorAnimator = GetComponent<Animator>();
        doorCollider = GetComponent<BoxCollider2D>();

        if (passwordPanel != null)
        {
            panelAnimator = passwordPanel.GetComponent<Animator>();
        }

        // Thêm listener cho nút bấm, khi bấm sẽ gọi hàm CheckPassword
        submitButton.onClick.AddListener(CheckPassword);
        // Tắt panel lúc bắt đầu để đảm bảo nó luôn ẩn
        passwordPanel.SetActive(false);
    }

    // Hàm được gọi bởi TriggerZone khi có vật thể đi vào
    public void OnPlayerEnterTrigger()
    {
        if (!isDoorOpen)
        {
            // Luôn SetActive(true) trước khi chạy animation
            passwordPanel.SetActive(true);
            // Dùng SetBool thay cho SetTrigger
            panelAnimator.SetBool("isOpen", true);
        }
    }

    // Hàm được gọi bởi TriggerZone khi có vật thể đi ra
    public void OnPlayerExitTrigger()
    {
        if (!isDoorOpen && passwordPanel.activeSelf) // Thêm kiểm tra panel có đang bật ko
        {
            // Dùng SetBool thay cho SetTrigger
            panelAnimator.SetBool("isOpen", false);
        }
    }

    // Hàm kiểm tra mật khẩu, được gọi bởi nút bấm
    private void CheckPassword()
    {
        if (passwordInputField.text == correctPassword)
        {
            Debug.Log("Mật khẩu chính xác! Mở cửa.");
            isDoorOpen = true; // Đánh dấu cửa đã mở

            // Ra lệnh cho cửa và panel chạy animation
            doorAnimator.SetTrigger("OpenDoor");
            panelAnimator.SetBool("isOpen", false);

            // Không cho phép tương tác với nút bấm và vùng trigger nữa
            submitButton.interactable = false;
            // Tắt hẳn TriggerZone đi để không hiện lại bảng nữa
            transform.Find("TriggerZone").gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Mật khẩu sai!");
            // Thêm hiệu ứng báo sai ở đây, ví dụ:
            passwordInputField.text = ""; // Xóa chữ đã nhập
            // Có thể làm animation rung lắc cho InputField
        }
    }

    // Hàm này sẽ được gọi bởi Animation Event ở cuối animation mở cửa
    public void DisableDoorCollider()
    {
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }
    }
    public void DeactivatePasswordPanel()
    {
        if (passwordPanel != null)
        {
            passwordPanel.SetActive(false);
        }
    }
}