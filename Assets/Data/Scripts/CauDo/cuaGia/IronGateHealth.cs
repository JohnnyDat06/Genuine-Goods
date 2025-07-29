using UnityEngine;

public class IronGateHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("Máu tối đa của cửa sắt")]
    public int maxHealth = 200;

    // Các biến private để script tự quản lý
    private int currentHealth;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private bool isDestroyed = false; // Cờ để đánh dấu cửa đã bị phá

    void Start()
    {
        // Khởi tạo các giá trị ban đầu
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Hàm public để các object khác (như đòn đánh của player) có thể gọi
    public void TakeDamage(int damage)
    {
        // Nếu cửa đã bị phá thì không làm gì nữa
        if (isDestroyed) return;

        currentHealth -= damage;
        Debug.Log("Cửa nhận sát thương, máu còn lại: " + currentHealth);

        // Optional: Nếu bro có animation bị đánh, hãy kích hoạt nó ở đây
        // animator.SetTrigger("Hit"); 

        if (currentHealth <= 0)
        {
            isDestroyed = true; // Đánh dấu đã phá, không nhận thêm sát thương
            OpenGate();
        }
    }

    void OpenGate()
    {
        Debug.Log("Cửa sắt đã hết máu và đang mở...");

        // Nhiệm vụ duy nhất của code là kích hoạt trigger trong Animator
        // Mọi thứ còn lại (đổi sprite, co collider) Animator sẽ lo hết
        animator.SetTrigger("Open");

        // Note: Mình không cần tắt collider ở đây nữa, vì animation đã lo việc đó rồi!
    }
}