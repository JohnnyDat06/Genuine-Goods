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

    // --- THAY ĐỔI: Thêm biến để biết NPC nào đang nói chuyện ---
    public NPC_Controller SpeakingNPCController { get; private set; }
    // --------------------------------------------------------

    private static List<Conversational_NPC> allNpcs = new List<Conversational_NPC>();

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
               Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D);
    }

    public static void RegisterNPC(Conversational_NPC npc)
    {
        if (!allNpcs.Contains(npc))
        {
            allNpcs.Add(npc);
        }
    }

    public static void UnregisterNPC(Conversational_NPC npc)
    {
        if (allNpcs.Contains(npc))
        {
            allNpcs.Remove(npc);
        }
    }

    public void StartDialogue(DialogueObject dialogue, NPC_Controller npcController, Conversational_NPC currentSpeaker, Action onDialogueFinished = null)
    {
        IsDialogueActive = true;
        dialoguePanel.SetActive(true);
        this.onDialogueFinishedCallback = onDialogueFinished;

        foreach (var npc in allNpcs)
        {
            if (npc != currentSpeaker)
            {
                npc.DeactivateForDialogue();
            }
        }

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
            Rigidbody2D playerRb = playerController.GetComponent<Rigidbody2D>();
            if (playerRb != null) playerRb.velocity = Vector2.zero;
            Animator playerAnimator = playerController.GetComponent<Animator>();
            if (playerAnimator != null) playerAnimator.SetInteger("State", 0);
        }

        npcControllerToDisable = npcController;
        // --- THAY ĐỔI: Lưu lại NPC đang nói chuyện ---
        SpeakingNPCController = npcControllerToDisable;
        // ------------------------------------------
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

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        if (npcControllerToDisable != null) npcControllerToDisable.enabled = true;

        // --- THAY ĐỔI: Reset lại NPC đang nói chuyện ---
        SpeakingNPCController = null;
        // -------------------------------------------

        foreach (var npc in allNpcs)
        {
            npc.ActivateAfterDialogue();
        }

        onDialogueFinishedCallback?.Invoke();
        onDialogueFinishedCallback = null;
    }
}
