using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    #region Singleton
    public static DialogueManager instance;
    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
    }
    #endregion

    [Header("UI Components")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    // DÒNG MỚI: Thêm tham chiếu đến UI Text cho tên nhân vật
    [SerializeField] private TextMeshProUGUI nameText;

    private Queue<string> sentences;
    private bool isDialogueActive = false;
    private PlayerController playerController;
    private MonoBehaviour npcControllerToDisable;

    void Start()
    {
        sentences = new Queue<string>();
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (isDialogueActive && Input.anyKeyDown)
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(DialogueObject dialogue, MonoBehaviour npcController)
    {
        isDialogueActive = true;
        dialoguePanel.SetActive(true);

        // THAY ĐỔI: Lấy tên từ DialogueObject và hiển thị lên NameText
        // Đảm bảo NameText không bị null
        if (nameText != null)
        {
            nameText.text = dialogue.CharacterName;
        }

        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            playerController.enabled = false;
        }

        npcControllerToDisable = npcController;
        if (npcControllerToDisable != null)
        {
            npcControllerToDisable.enabled = false;
        }

        sentences.Clear();
        foreach (string sentence in dialogue.DialogueLines)
        {
            sentences.Enqueue(sentence);
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
        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        if (npcControllerToDisable != null)
        {
            npcControllerToDisable.enabled = true;
        }
    }
}