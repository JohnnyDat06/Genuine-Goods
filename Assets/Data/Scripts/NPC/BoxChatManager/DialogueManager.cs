// File: DialogueManager.cs
// Phiên bản đầy đủ, đã thêm chức năng chuyển scene khi chọn đúng

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Thư viện để quản lý Scene
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("UI Components")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Typing Effect")]
    [SerializeField] private float typingSpeed = 0.02f;

    [Header("Choice Components")]
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button[] choiceButtons;

    [Header("Player Control")]
    [Tooltip("Kéo script điều khiển của người chơi (PlayerController hoặc HungController) vào đây.")]
    [SerializeField] private MonoBehaviour playerControlScript;

    private MonoBehaviour controllerToDisable;
    public MonoBehaviour SpeakingNPCController { get; private set; }
    private Queue<DialogueLine> sentences;
    public bool IsDialogueActive { get; private set; }
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentFullSentence;
    private Action onDialogueFinishedCallback;
    private static List<Conversational_NPC> allNpcs = new List<Conversational_NPC>();
    private AudioSource audioSource;
    private DialogueLine currentLine;
    private DialogueObject currentDialogue;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        sentences = new Queue<DialogueLine>();
        dialoguePanel.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);
    }

    public void StartDialogue(DialogueObject dialogue, MonoBehaviour controllerToDisable, Conversational_NPC currentSpeaker, Action onDialogueFinished = null)
    {
        IsDialogueActive = true;
        dialoguePanel.SetActive(true);
        if (choicePanel != null) choicePanel.SetActive(false);
        this.onDialogueFinishedCallback = onDialogueFinished;
        this.currentDialogue = dialogue;

        foreach (var npc in new List<Conversational_NPC>(allNpcs))
        {
            if (npc != null && npc != currentSpeaker) npc.DeactivateForDialogue();
        }

        if (playerControlScript != null)
        {
            playerControlScript.enabled = false;
            Rigidbody2D playerRb = playerControlScript.GetComponentInChildren<Rigidbody2D>();
            if (playerRb != null) playerRb.velocity = Vector2.zero;
            Animator playerAnimator = playerControlScript.GetComponentInChildren<Animator>();
            if (playerAnimator != null) playerAnimator.SetInteger("State", 0);
        }

        this.controllerToDisable = controllerToDisable;
        SpeakingNPCController = this.controllerToDisable;
        if (this.controllerToDisable != null) this.controllerToDisable.enabled = false;

        sentences.Clear();
        foreach (DialogueLine line in dialogue.DialogueLines)
        {
            sentences.Enqueue(line);
        }
        DisplayNextSentence();
    }

    public void EndDialogue()
    {
        IsDialogueActive = false;
        dialoguePanel.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        isTyping = false;
        audioSource.Stop();
        audioSource.loop = false;

        if (playerControlScript != null) playerControlScript.enabled = true;
        if (controllerToDisable != null) controllerToDisable.enabled = true;
        SpeakingNPCController = null;

        foreach (var npc in new List<Conversational_NPC>(allNpcs))
        {
            if (npc != null) npc.ActivateAfterDialogue();
        }

        onDialogueFinishedCallback?.Invoke();
        onDialogueFinishedCallback = null;
    }

    public void SelectChoice(DialogueChoice choice)
    {
        if (choicePanel != null) choicePanel.SetActive(false);
        sentences.Clear();

        if (choice.isCorrectChoice)
        {
            Debug.Log("Lựa chọn đúng! Đang tải scene: " + choice.sceneToLoad);

            if (playerControlScript != null)
            {
                playerControlScript.enabled = true;
            }

            // Kiểm tra xem tên scene có rỗng không trước khi tải
            if (!string.IsNullOrEmpty(choice.sceneToLoad))
            {
                SceneManager.LoadScene(choice.sceneToLoad);
            }
            else
            {
                Debug.LogWarning("Lựa chọn đúng nhưng không có tên scene để tải!");
                EndDialogue();
            }
            return;
        }

        if (choice.nextDialogue != null)
        {
            foreach (DialogueLine line in choice.nextDialogue.DialogueLines)
            {
                sentences.Enqueue(line);
            }
        }

        if (currentDialogue != null && currentDialogue.FollowUpDialogue != null)
        {
            foreach (DialogueLine line in currentDialogue.FollowUpDialogue.DialogueLines)
            {
                sentences.Enqueue(line);
            }
        }

        if (sentences.Count > 0)
        {
            DisplayNextSentence();
        }
        else
        {
            EndDialogue();
        }
    }

    void Update()
    {
        if (!IsDialogueActive) return;

        if (Input.anyKeyDown && !IsIgnoredKey() && (choicePanel == null || !choicePanel.activeSelf))
        {
            if (isTyping)
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                dialogueText.text = currentFullSentence;
                isTyping = false;
                audioSource.Stop();
                audioSource.loop = false;

                if (currentLine.HasChoices)
                {
                    PresentChoices(currentLine.choiceData);
                }
            }
            else
            {
                DisplayNextSentence();
            }
        }
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        currentLine = sentences.Dequeue();
        nameText.text = currentLine.characterName;
        currentFullSentence = currentLine.sentence;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentence(currentLine));
    }

    private IEnumerator TypeSentence(DialogueLine line)
    {
        isTyping = true;
        dialogueText.text = "";
        if (line.voiceClip != null) { audioSource.clip = line.voiceClip; audioSource.loop = true; audioSource.Play(); }
        foreach (char letter in line.sentence.ToCharArray()) { dialogueText.text += letter; yield return new WaitForSeconds(typingSpeed); }
        if (line.voiceClip != null) { audioSource.Stop(); audioSource.loop = false; }
        if (line.HasChoices) { PresentChoices(line.choiceData); } else { isTyping = false; }
    }

    private void PresentChoices(ChoiceData data)
    {
        if (choicePanel == null) return;
        choicePanel.SetActive(true);
        isTyping = false;
        for (int i = 0; i < choiceButtons.Length; i++) { choiceButtons[i].gameObject.SetActive(false); }
        for (int i = 0; i < data.choices.Length; i++)
        {
            if (i < choiceButtons.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = data.choices[i].choiceText;
                choiceButtons[i].onClick.RemoveAllListeners();
                DialogueChoice choice = data.choices[i];
                choiceButtons[i].onClick.AddListener(() => SelectChoice(choice));
            }
        }
    }

    private bool IsIgnoredKey()
    {
        return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
               Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D);
    }
    public static void RegisterNPC(Conversational_NPC npc) { if (!allNpcs.Contains(npc)) allNpcs.Add(npc); }
    public static void UnregisterNPC(Conversational_NPC npc) { if (allNpcs.Contains(npc)) allNpcs.Remove(npc); }
}