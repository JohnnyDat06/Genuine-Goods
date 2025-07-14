using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("UI Components")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private Queue<DialogueLine> sentences;
    public bool IsDialogueActive { get; private set; }

    private PlayerController playerController;
    private NPC_Controller npcControllerToDisable;
    private KeyCode currentInteractionKey;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        sentences = new Queue<DialogueLine>();
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (!IsDialogueActive) return;

        if (Input.anyKeyDown && !IsIgnoredKey())
        {
            DisplayNextSentence();
        }
    }

    private bool IsIgnoredKey()
    {
        return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
               Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
               Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
               Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow);
        // Đã xóa currentInteractionKey khỏi đây để tránh xung đột
    }

    public void StartDialogue(DialogueObject dialogue, NPC_Controller npcController)
    {
        IsDialogueActive = true;
        dialoguePanel.SetActive(true);
        // this.currentInteractionKey = interactionKey; // Dòng này không còn cần thiết

        // Vô hiệu hóa người chơi
        PlayerController playerController = FindObjectOfType<PlayerController>(); // Khai báo lại biến cục bộ
        if (playerController != null)
        {
            Animator playerAnimator = playerController.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.SetInteger("State", 0);
            }

            playerController.enabled = false;
            Rigidbody2D playerRb = playerController.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = Vector2.zero;
            }
        }

        // Vô hiệu hóa NPC
        npcControllerToDisable = npcController;
        if (npcControllerToDisable != null)
        {
            npcControllerToDisable.enabled = false;
            // Đã xóa dòng gọi FacePlayer
        }

        sentences.Clear();
        foreach (DialogueLine line in dialogue.DialogueLines)
        {
            sentences.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = sentences.Dequeue();
        nameText.text = currentLine.characterName;
        dialogueText.text = currentLine.sentence;
    }

    public void EndDialogue()
    {
        IsDialogueActive = false;
        dialoguePanel.SetActive(false);

        // Kích hoạt lại người chơi
        PlayerController playerController = FindObjectOfType<PlayerController>(); // Tìm lại để kích hoạt
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // Kích hoạt lại NPC
        if (npcControllerToDisable != null)
        {
            npcControllerToDisable.enabled = true;
        }
    }
}