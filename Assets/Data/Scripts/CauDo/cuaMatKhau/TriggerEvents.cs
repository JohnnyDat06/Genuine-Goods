using UnityEngine;

public class TriggerEvents : MonoBehaviour
{
    // Kéo object PasswordDoor vào đây trong Inspector
    public PasswordDoorController doorController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            doorController.OnPlayerEnterTrigger();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            doorController.OnPlayerExitTrigger();
        }
    }
}