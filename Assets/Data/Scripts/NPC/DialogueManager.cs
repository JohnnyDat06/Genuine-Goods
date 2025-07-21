using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("UI Components")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Typing Effect")]
    [SerializeField] private float typingSpeed = 0.02f;

    private Queue<DialogueLine> sentences;
    public bool IsDialogueActive { get; private set; }

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentFullSentence;
    private NPC_Controller npcControllerToDisable;
    private Action onDialogueFinishedCallback;

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
            if (isTyping)
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                dialogueText.text = currentFullSentence;
                isTyping = false;
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }

    private bool IsIgnoredKey()
    {
        return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
               Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
               Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
               Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow);
    }

    public void StartDialogue(DialogueObject dialogue, NPC_Controller npcController, Action onDialogueFinished = null)
    {
        IsDialogueActive = true;
        dialoguePanel.SetActive(true);
        this.onDialogueFinishedCallback = onDialogueFinished;

        // --- KHÓA NGƯỜI CHƠI VÀ CHUYỂN VỀ IDLE ---
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            // 1. Vô hiệu hóa script để không nhận input
            playerController.enabled = false;

            // 2. Triệt tiêu vận tốc còn lại
            Rigidbody2D playerRb = playerController.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = Vector2.zero;
            }

            // 3. Ra lệnh cho Animator chuyển về trạng thái Idle
            Animator playerAnimator = playerController.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                // Dựa theo PlayerController.cs, trạng thái Idle là 0
                playerAnimator.SetInteger("State", 0);
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
        currentFullSentence = currentLine.sentence;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentence(currentFullSentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    public void EndDialogue()
    {
        IsDialogueActive = false;
        dialoguePanel.SetActive(false);

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        isTyping = false;

        // --- KÍCH HOẠT LẠI NGƯỜI CHƠI ---
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            // Chỉ cần kích hoạt lại script. Script sẽ tự điều khiển Animator
            playerController.enabled = true;
        }

        // Kích hoạt lại NPC
        if (npcControllerToDisable != null) npcControllerToDisable.enabled = true;

        // Gọi callback nếu nó tồn tại
        onDialogueFinishedCallback?.Invoke();
    }
}