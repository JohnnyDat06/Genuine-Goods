using UnityEngine;
using UnityEngine.Events; // Quan trọng: Phải có dòng này

public class AnimationSignal : MonoBehaviour
{
    // Tạo ra một sự kiện có thể tùy chỉnh trong Inspector
    public UnityEvent onAnimationEvent;

    // Hàm này sẽ được gọi bởi Animation Event
    public void SendSignal()
    {
        // Bắn tín hiệu ra cho bất kỳ ai đang lắng nghe
        onAnimationEvent.Invoke();
    }
}