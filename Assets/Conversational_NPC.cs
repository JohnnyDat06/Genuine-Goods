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
    private bool hasBeenSpokenTo = false; // Cờ để kiểm tra đã nói chuyện lần nào chưa

    private void Update()
    {
        if (playerIsInRange && !DialogueManager.instance.IsDialogueActive && Input.GetKeyDown(interactionKey))
        {
            TriggerDialogue();
        }
    }

    private void TriggerDialogue()
    {
        // Nếu chưa nói chuyện lần nào
        if (!hasBeenSpokenTo)
        {
            // Tạo một hành động (callback) để thực hiện SAU KHI hội thoại kết thúc
            Action onFinish = () =>
            {
                hasBeenSpokenTo = true; // Đặt cờ thành true
            };

            // Bắt đầu hội thoại lần đầu và truyền callback vào
            DialogueManager.instance.StartDialogue(initialDialogue, npcController, onFinish);
        }
        // Nếu đã nói chuyện rồi
        else
        {
            // Bắt đầu hội thoại lặp lại (không cần callback)
            DialogueManager.instance.StartDialogue(repeatingDialogue, npcController);
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