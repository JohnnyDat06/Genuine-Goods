using System.Collections; // Cần dùng thư viện này cho Coroutine
using UnityEngine;

public class IronGateHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("Máu tối đa của cửa sắt")]
    public int maxHealth = 200;

    [Header("Shake Effect Settings")]
    [Tooltip("Độ rung mạnh hay yếu")]
    public float shakeMagnitude = 0.05f;

    [Tooltip("Rung trong bao lâu")]
    public float shakeDuration = 0.15f;

    // Các biến private để script tự quản lý
    private int currentHealth;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private bool isDestroyed = false;
    private Vector3 originalPosition; // Để lưu vị trí gốc của cửa

    void Start()
    {
        // Khởi tạo các giá trị ban đầu
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        originalPosition = transform.position; // Lưu vị trí gốc
    }

    // Hàm public để các object khác có thể gọi
    public void TakeDamage(int damage)
    {
        // Nếu cửa đã bị phá thì không làm gì nữa
        if (isDestroyed) return;

        currentHealth -= damage;
        Debug.Log("Cửa nhận sát thương, máu còn lại: " + currentHealth);

        // Kích hoạt hiệu ứng rung lắc
        StartCoroutine(Shake());

        if (currentHealth <= 0)
        {
            isDestroyed = true; // Đánh dấu đã phá, không nhận thêm sát thương
            OpenGate();
        }
    }

    void OpenGate()
    {
        Debug.Log("Cửa sắt đã hết máu và đang mở...");
        // Kích hoạt trigger trong Animator, mọi thứ còn lại Animator sẽ lo
        animator.SetTrigger("Open");
    }

    // Coroutine để tạo hiệu ứng rung lắc
    IEnumerator Shake()
    {
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            // Tạo ra một vị trí ngẫu nhiên nho nhỏ xung quanh vị trí gốc
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.position = originalPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;

            // Đợi đến frame tiếp theo rồi mới chạy tiếp
            yield return null;
        }

        // Hết thời gian rung, trả cửa về vị trí ban đầu cho nó "ngoan"
        transform.position = originalPosition;
    }
}