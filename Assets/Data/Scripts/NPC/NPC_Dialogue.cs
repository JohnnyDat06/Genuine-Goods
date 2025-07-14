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
            // Trả lời gọi hàm về như cũ, không cần truyền transform của Player
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

            if (DialogueManager.instance != null && DialogueManager.instance.IsDialogueActive)
            {
                DialogueManager.instance.EndDialogue();
            }
        }
    }
}