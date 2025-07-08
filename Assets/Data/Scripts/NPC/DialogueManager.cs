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

    private Queue<string> sentences;
    public bool IsDialogueActive { get; private set; } // THAY ĐỔI Ở ĐÂY
    private PlayerController playerController;
    private MonoBehaviour npcControllerToDisable;

    private KeyCode currentInteractionKey;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        sentences = new Queue<string>();
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (!IsDialogueActive) return; // THAY ĐỔI Ở ĐÂY

        if (Input.anyKeyDown)
        {
            if (IsIgnoredKey())
            {
                return;
            }
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

    public void StartDialogue(DialogueObject dialogue, MonoBehaviour npcController, KeyCode interactionKey)
    {
        IsDialogueActive = true; // THAY ĐỔI Ở ĐÂY
        dialoguePanel.SetActive(true);
        this.currentInteractionKey = interactionKey;

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

        if (nameText != null) nameText.text = dialogue.CharacterName;
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
        IsDialogueActive = false; 
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