using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Connections")]
    [Tooltip("Kéo đối tượng GameObject chứa script PulsingWarning vào đây.")]
    public PulsingWarning warningZone;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (warningZone != null)
            {
                warningZone.SetWarningMessage("TÍN HIỆU CẢNH BÁO TỪ GAMEMANAGER!");
            }
            else
            {
                Debug.LogError("Chưa gán đối tượng Warning Zone cho GameManager trong Inspector!");
            }
        }
    }
}