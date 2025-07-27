using UnityEngine;
using System;

public class Conversational_NPC : MonoBehaviour
{
    // --- THAY ĐỔI: Sử dụng mảng để chứa hội thoại theo tiến trình ---
    [Header("Hội thoại theo tiến trình")]
    [SerializeField] private DialogueObject[] progressionDialogues;

    [Header("Hội thoại lặp lại")]
    [SerializeField] private DialogueObject repeatingDialogue;
    // -----------------------------------------------------------------

    [Header("Components & Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private NPC_Controller npcController;

    private bool playerIsInRange = false;
    private bool isInteractionDisabled = false;

    // --- THAY ĐỔI: Lưu lại cấp raid đã nói chuyện ---
    private int lastSpokenRaid = -1;
    // ---------------------------------------------

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

    // --- THAY ĐỔI: Logic chọn hội thoại dựa trên MissionManager.Instance.currentRaid ---
    private void TriggerDialogue()
    {
        // Lấy tiến trình raid hiện tại từ MissionManager
        int currentRaid = MissionManager.Instance.currentRaid;

        // Kiểm tra xem có hội thoại tiến trình MỚI cho raid hiện tại không
        if (lastSpokenRaid < currentRaid && currentRaid < progressionDialogues.Length)
        {
            // Lấy đúng đoạn hội thoại cho raid này
            DialogueObject dialogueForThisRaid = progressionDialogues[currentRaid];

            // Tạo một hành động (Action) sẽ được thực thi sau khi hội thoại kết thúc
            Action onFinish = () =>
            {
                // Đánh dấu rằng người chơi đã nói chuyện ở cấp raid này rồi
                lastSpokenRaid = currentRaid;
            };

            // Bắt đầu hội thoại tiến trình, và truyền vào hành động onFinish
            DialogueManager.instance.StartDialogue(dialogueForThisRaid, npcController, this, onFinish);
        }
        else
        {
            // Nếu không có hội thoại tiến trình mới, sử dụng hội thoại lặp lại
            DialogueManager.instance.StartDialogue(repeatingDialogue, npcController, this);
        }
    }
    // -------------------------------------------------------------------------------------

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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInRange = false;
            if (interactionPrompt != null) interactionPrompt.SetActive(false);

            if (DialogueManager.instance != null && DialogueManager.instance.IsDialogueActive && DialogueManager.instance.SpeakingNPCController == this.npcController)
            {
                DialogueManager.instance.EndDialogue();
            }
        }
    }

    public void DeactivateForDialogue()
    {
        isInteractionDisabled = true;
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    public void ActivateAfterDialogue()
    {
        isInteractionDisabled = false;
        if (playerIsInRange && interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
    }
}