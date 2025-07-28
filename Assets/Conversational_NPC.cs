using UnityEngine;
using System;

public class Conversational_NPC : MonoBehaviour
{
    [Header("CÁC GIAI ĐOẠN HỘI THOẠI")]
    [SerializeField] private RaidDialogueStage[] dialogueStages; // Mảng chứa toàn bộ hội thoại cho từng Hồi/Raid

    [Header("Components & Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private NPC_Controller npcController;

    private bool playerIsInRange = false;
    private bool isInteractionDisabled = false;

    // Các biến để lưu trạng thái hội thoại đã nói
    private int lastAcknowledgmentRaid = -1;
    private int lastMainDialogueRaid = -1;

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
        int currentRaid = MissionManager.Instance.currentRaid;

        // Thoát nếu currentRaid vượt quá số lượng giai đoạn hội thoại đã thiết lập
        if (currentRaid >= dialogueStages.Length)
        {
            return; // Dòng Debug.LogWarning đã được xóa ở đây
        }

        // Lấy dữ liệu hội thoại cho giai đoạn (Hồi) hiện tại
        RaidDialogueStage currentStage = dialogueStages[currentRaid];

        // ƯU TIÊN 1: Chạy hội thoại "CẢM ƠN / GHI NHẬN" nếu có và chưa nói
        if (lastAcknowledgmentRaid < currentRaid && currentStage.AcknowledgmentDialogue != null)
        {
            Action onFinish = () => {
                lastAcknowledgmentRaid = currentRaid; // Đánh dấu đã nói
            };
            DialogueManager.instance.StartDialogue(currentStage.AcknowledgmentDialogue, npcController, this, onFinish);
            return;
        }

        // ƯU TIÊN 2: Chạy hội thoại "CHÍNH / ĐIỀU TRA" nếu có và chưa nói
        if (lastMainDialogueRaid < currentRaid && currentStage.MainDialogue != null)
        {
            Action onFinish = () => {
                lastMainDialogueRaid = currentRaid; // Đánh dấu đã nói
                // Đồng thời cũng đánh dấu đã qua giai đoạn cảm ơn
                lastAcknowledgmentRaid = currentRaid;
            };
            DialogueManager.instance.StartDialogue(currentStage.MainDialogue, npcController, this, onFinish);
            return;
        }

        // ƯU TIÊN 3: Nếu đã nói hết các thoại trên, chạy hội thoại "LẶP LẠI"
        if (currentStage.RepeatingDialogue != null)
        {
            DialogueManager.instance.StartDialogue(currentStage.RepeatingDialogue, npcController, this);
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