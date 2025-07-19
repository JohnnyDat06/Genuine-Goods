using UnityEngine;

public class NPC_Dialogue : MonoBehaviour
{
    [SerializeField] private DialogueObject dialogue;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private NPC_Controller npcController;

    private bool playerIsInRange = false;

    private void Update()
    {
        if (playerIsInRange && !DialogueManager.instance.IsDialogueActive && Input.GetKeyDown(interactionKey))
        {
            // Bắt đầu hội thoại và truyền vào NPC controller để vô hiệu hóa nó
            DialogueManager.instance.StartDialogue(dialogue, npcController);
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

            // Nếu người chơi rời đi khi đang hội thoại, kết thúc hội thoại
            if (DialogueManager.instance != null && DialogueManager.instance.IsDialogueActive)
            {
                DialogueManager.instance.EndDialogue();
            }
        }
    }
}
