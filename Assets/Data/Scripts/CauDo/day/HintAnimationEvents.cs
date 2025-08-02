// HintAnimationEvents.cs
using UnityEngine;

public class HintAnimationEvents : MonoBehaviour
{
    // Hàm public để Animation Event có thể thấy
    public void OnCloseAnimationFinished()
    {
        // Tìm đến PuzzleManager và bảo nó làm nhiệm vụ
        PuzzleManager.instance.DeactivateHintObject();
    }
}