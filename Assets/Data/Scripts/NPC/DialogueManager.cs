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
               Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) ||
               Input.GetKeyDown(currentInteractionKey);
    }

    public void StartDialogue(DialogueObject dialogue, NPC_Controller npcController, KeyCode interactionKey)
    {
        IsDialogueActive = true;
        dialoguePanel.SetActive(true);
        this.currentInteractionKey = interactionKey;

        // Vô hiệu hóa người chơi
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            // 1. Ngừng script nhận input mới
            playerController.enabled = false;

            // 2. DÒNG MỚI: Lấy Rigidbody2D của Player và dừng ngay lập tức
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