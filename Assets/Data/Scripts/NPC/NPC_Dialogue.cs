using UnityEngine;

public class NPC_Dialogue : MonoBehaviour
{
    [SerializeField] private DialogueObject dialogue;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private MonoBehaviour npcControllerScript;

    private bool playerIsInRange = false;

    private void Update()
    {
        if (playerIsInRange && !DialogueManager.instance.IsDialogueActive && Input.GetKeyDown(interactionKey))
        {
            DialogueManager.instance.StartDialogue(dialogue, npcControllerScript, interactionKey);
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
        }
    }

}