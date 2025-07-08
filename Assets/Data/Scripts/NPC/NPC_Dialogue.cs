using UnityEngine;

public class NPC_Dialogue : MonoBehaviour
{
    [Header("Dialogue Data")]
    [SerializeField] private DialogueObject dialogue; // Kéo file hội thoại (ScriptableObject) vào đây

    [Header("Interaction")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E; // Phím để tương tác
    [SerializeField] private GameObject interactionPrompt; // UI nhỏ (ví dụ: hình chữ 'E') để báo hiệu có thể tương tác

    // Tham chiếu đến script điều khiển của chính NPC này để DialogueManager có thể vô hiệu hóa nó
    [SerializeField] private MonoBehaviour npcControllerScript;

    private bool playerIsInRange = false;

    private void Awake()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        // Nếu người chơi trong tầm và bấm phím tương tác
        if (playerIsInRange && Input.GetKeyDown(interactionKey))
        {
            DialogueManager.instance.StartDialogue(dialogue, npcControllerScript);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Chỉ kích hoạt khi đối tượng va chạm có tag là "Player"
        if (other.CompareTag("Player"))
        {
            playerIsInRange = true;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = false;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }
}