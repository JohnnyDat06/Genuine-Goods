using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System; // THAY ĐỔI: Thêm thư viện System để sử dụng Action

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

    // THAY ĐỔI: Thêm một biến để lưu trữ hành động callback
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

    // THAY ĐỔI: Thêm tham số 'Action onDialogueFinished' vào hàm StartDialogue
    public void StartDialogue(DialogueObject dialogue, NPC_Controller npcController, Action onDialogueFinished = null)
    {
        IsDialogueActive = true;
        dialoguePanel.SetActive(true);
        this.onDialogueFinishedCallback = onDialogueFinished; // Lưu lại callback

        // Vô hiệu hóa người chơi
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            // ... (phần code vô hiệu hóa người chơi giữ nguyên)
            playerController.enabled = false;
            // ...
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

        // Kích hoạt lại người chơi
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null) playerController.enabled = true;

        // Kích hoạt lại NPC
        if (npcControllerToDisable != null) npcControllerToDisable.enabled = true;

        // THAY ĐỔI: Gọi callback nếu nó tồn tại
        onDialogueFinishedCallback?.Invoke();
    }
}