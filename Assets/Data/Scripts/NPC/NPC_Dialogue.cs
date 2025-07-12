using UnityEngine;

public class NPC_Dialogue : MonoBehaviour
{
    [SerializeField] private DialogueObject dialogue;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private GameObject interactionPrompt; // Ví dụ: phím 'E' hiển thị trên đầu NPC
    [SerializeField] private NPC_Controller npcController;

    private bool playerIsInRange = false;

    private void Update()
    {
        if (playerIsInRange && !DialogueManager.instance.IsDialogueActive && Input.GetKeyDown(interactionKey))
        {
            DialogueManager.instance.StartDialogue(dialogue, npcController, interactionKey);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = true;
            if (interactionPrompt != null) interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = false;
            if (interactionPrompt != null) interactionPrompt.SetActive(false);

            // Tùy chọn: nếu người chơi rời đi khi hội thoại đang diễn ra, hãy kết thúc nó
            if (DialogueManager.instance.IsDialogueActive)
            {
                DialogueManager.instance.EndDialogue();
            }
        }
    }
}