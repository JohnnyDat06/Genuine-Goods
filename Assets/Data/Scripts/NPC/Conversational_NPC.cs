using UnityEngine;
using System;

public class Conversational_NPC : MonoBehaviour
{
    [Header("Dialogue Objects")]
    [SerializeField] private DialogueObject initialDialogue; // Hội thoại lần đầu
    [SerializeField] private DialogueObject repeatingDialogue; // Hội thoại lặp lại

    [Header("Components & Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private NPC_Controller npcController;

    private bool playerIsInRange = false;
    private bool hasBeenSpokenTo = false;
    private bool isInteractionDisabled = false; // Cờ để vô hiệu hóa tạm thời

    private void OnEnable()
    {
        DialogueManager.RegisterNPC(this);
    }

    private void OnDisable()
    {
        DialogueManager.UnregisterNPC(this);
    }

    private void Update()
    {
        if (playerIsInRange && !isInteractionDisabled && !DialogueManager.instance.IsDialogueActive && Input.GetKeyDown(interactionKey))
        {
            TriggerDialogue();
        }
    }

    private void TriggerDialogue()
    {
        if (!hasBeenSpokenTo)
        {
            Action onFinish = () =>
            {
                hasBeenSpokenTo = true;
            };
            DialogueManager.instance.StartDialogue(initialDialogue, npcController, this, onFinish);
        }
        else
        {
            DialogueManager.instance.StartDialogue(repeatingDialogue, npcController, this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = true;
            if (!isInteractionDisabled && interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
        }
    }

    // --- HÀM ONTRIGGEREXIT2D ĐÃ ĐƯỢC CẬP NHẬT ---
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = false;
            if (interactionPrompt != null) interactionPrompt.SetActive(false);

            // THÊM ĐIỀU KIỆN MỚI: Chỉ kết thúc hội thoại nếu người chơi đang nói chuyện VỚI CHÍNH NPC NÀY.
            if (DialogueManager.instance != null && DialogueManager.instance.IsDialogueActive && DialogueManager.instance.SpeakingNPCController == this.npcController)
            {
                DialogueManager.instance.EndDialogue();
            }
        }
    }
    // -------------------------------------------

    public void DeactivateForDialogue()
    {
        isInteractionDisabled = true;
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        // Giữ nguyên logic này: các NPC khác vẫn di chuyển
    }

    public void ActivateAfterDialogue()
    {
        isInteractionDisabled = false;
        if (playerIsInRange)
        {
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
        }
    }
}
