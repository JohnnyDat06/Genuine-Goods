using UnityEngine;
using System;

public class Conversational_NPC : MonoBehaviour
{
    [Header("CÁC GIAI ĐOẠN HỘI THOẠI")]
    [SerializeField] private RaidDialogueStage[] dialogueStages;

    [Header("Components & Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private NPC_Controller npcController;

    [Header("Proximity Detection (Không dùng Trigger)")]
    [Tooltip("Đánh dấu nếu NPC này dùng Collider vật lý (không phải Trigger) để tương tác.")]
    [SerializeField] private bool usePhysicsCheck = false;
    [Tooltip("Bán kính vòng tròn để phát hiện người chơi.")]
    [SerializeField] private float interactionRadius = 2f;
    [Tooltip("Chọn Layer của Người chơi để tối ưu việc kiểm tra.")]
    [SerializeField] private LayerMask playerLayer;

    private bool playerIsInRange = false;
    private bool isInteractionDisabled = false;
    private int lastAcknowledgmentRaid = -1;
    private int lastMainDialogueRaid = -1;
    private Transform playerTransform;

    private void OnEnable()
    {
        DialogueManager.RegisterNPC(this);
        // Khi được bật, chủ động tìm player
        if (playerTransform == null && playerLayer.value != 0)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
        }
    }

    private void OnDisable()
    {
        DialogueManager.UnregisterNPC(this);
    }

    private void Update()
    {
        if (usePhysicsCheck)
        {
            CheckPlayerProximity();
        }

        if (playerIsInRange && !isInteractionDisabled && !DialogueManager.instance.IsDialogueActive && Input.GetKeyDown(interactionKey))
        {
            TriggerDialogue();
        }
    }

    private void CheckPlayerProximity()
    {
        if (playerTransform == null) return;
        bool isPlayerNowInRange = Physics2D.OverlapCircle(transform.position, interactionRadius, playerLayer);

        if (isPlayerNowInRange != playerIsInRange)
        {
            playerIsInRange = isPlayerNowInRange;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(playerIsInRange && !isInteractionDisabled);
            }

            if (!playerIsInRange && DialogueManager.instance != null && DialogueManager.instance.IsDialogueActive && DialogueManager.instance.SpeakingNPCController == this.npcController)
            {
                DialogueManager.instance.EndDialogue();
            }
        }
    }

    private void TriggerDialogue()
    {
        int currentRaid = 0; // Thay thế bằng MissionManager.Instance.currentRaid nếu có
        if (currentRaid >= dialogueStages.Length) return;

        RaidDialogueStage currentStage = dialogueStages[currentRaid];

        if (lastAcknowledgmentRaid < currentRaid && currentStage.AcknowledgmentDialogue != null)
        {
            Action onFinish = () => { lastAcknowledgmentRaid = currentRaid; };
            DialogueManager.instance.StartDialogue(currentStage.AcknowledgmentDialogue, npcController, this, onFinish);
            return;
        }

        if (lastMainDialogueRaid < currentRaid && currentStage.MainDialogue != null)
        {
            Action onFinish = () => {
                lastMainDialogueRaid = currentRaid;
                lastAcknowledgmentRaid = currentRaid;
            };
            DialogueManager.instance.StartDialogue(currentStage.MainDialogue, npcController, this, onFinish);
            return;
        }

        if (currentStage.RepeatingDialogue != null)
        {
            DialogueManager.instance.StartDialogue(currentStage.RepeatingDialogue, npcController, this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (usePhysicsCheck) return;
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
        if (usePhysicsCheck) return;
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

    private void OnDrawGizmosSelected()
    {
        if (usePhysicsCheck)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
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
        if (usePhysicsCheck)
        {
            CheckPlayerProximity();
        }
        else if (playerIsInRange && interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
    }
}