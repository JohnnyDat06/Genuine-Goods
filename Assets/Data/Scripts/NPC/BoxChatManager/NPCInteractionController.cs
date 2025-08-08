using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class ChatMessage
{
    public string type;
    public string message;
}

[System.Serializable]
public class ChatPayload
{
    public string input;
    public List<ChatMessage> history;
}

[System.Serializable]
public class ChatResponse
{
    public string answer;
}

[RequireComponent(typeof(EnemyHealth))]
public class NPCInteractionController : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3.0f;
    public KeyCode interactionKey = KeyCode.E;
    public KeyCode exitKey = KeyCode.Escape;

    [Header("Main Chat UI")]
    public GameObject chatPanel;
    public TextMeshProUGUI chatLogText;
    public TMP_InputField playerInputField;
    public ScrollRect chatScrollRect;

    [Header("Ally Communication UI")]
    public Button allyCommunicateButton;
    public GameObject allyChatPanel;
    public TextMeshProUGUI allyChatLogText;

    [Header("UI Effects")]
    public float fadeDuration = 0.3f;

    [Header("Backend Settings")]
    public string serverUrl = "http://localhost:3000/api/chat";

    private Transform playerTransform;
    private MonoBehaviour playerMovementScript;
    private EnemyHealth enemyHealth;
    private bool isChatting = false;
    private List<ChatMessage> chatHistory = new List<ChatMessage>();
    private Coroutine allyMessageCoroutine;
    private Coroutine allyPanelFadeCoroutine;
    private CanvasGroup allyPanelCanvasGroup;
    private bool isAllyPanelVisible = false;

    private List<string> masterAllyDialogues = new List<string>
    {
        "Hỏi thẳng về kho hàng chính của hắn.",
        "Hắn đang tỏ ra kiêu ngạo. Đừng nao núng, hãy giữ vững áp lực.",
        "Chất vấn về các công ty vỏ bọc mà hắn dùng để rửa tiền.",
        "Yêu cầu hắn khai ra đối tác lớn nhất là ai.",
        "Đội của ta báo về có hoạt động đáng ngờ tại một chuỗi nhà hàng. Hỏi hắn về chuyện đó.",
        "Giả vờ tỏ ra thông cảm với quá khứ 'hai bàn tay trắng' của hắn để hắn mất cảnh giác.",
        "Phân tích cho thấy giọng điệu hắn đã thay đổi. Có vẻ hắn bắt đầu dao động rồi đấy.",
        "Chất vấn về mối quan hệ với các quan chức cấp cao. Yêu cầu một cái tên cụ thể.",
        "Hắn đang lảng tránh. Lặp lại câu hỏi về 'Lão Tam'. Đừng để hắn dẫn dắt câu chuyện.",
        "Thông tin tình báo cho thấy 'Lão Tam' chưa bao giờ lộ mặt. Hỏi hắn làm cách nào để liên lạc.",
        "Đưa ra một lời đề nghị cuối cùng: hợp tác để được khoan hồng hoặc nhận bản án cao nhất."
    };

    private int currentDialogueIndex;

    void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerMovementScript = playerObj.GetComponent<PlayerController>() as MonoBehaviour;
        }
        else
        {
            Debug.LogError("Không tìm thấy đối tượng có tag 'Player'.", this);
        }
    }

    void Start()
    {
        if (chatPanel != null) chatPanel.SetActive(false);
        if (allyCommunicateButton != null)
        {
            allyCommunicateButton.gameObject.SetActive(false);
            allyCommunicateButton.onClick.AddListener(ToggleAllyChat);
        }
        if (allyChatPanel != null)
        {
            allyPanelCanvasGroup = allyChatPanel.GetComponent<CanvasGroup>();
            if (allyPanelCanvasGroup == null)
            {
                allyPanelCanvasGroup = allyChatPanel.AddComponent<CanvasGroup>();
            }
            allyPanelCanvasGroup.alpha = 0f;
            allyPanelCanvasGroup.interactable = false;
            allyPanelCanvasGroup.blocksRaycasts = false;
            allyChatPanel.SetActive(true);
        }
    }

    void Update()
    {
        if (isChatting)
        {
            if (Input.GetKeyDown(exitKey))
            {
                EndChat();
            }
        }
        else
        {
            if (CanStartChat() && Input.GetKeyDown(interactionKey))
            {
                StartChat();
            }
        }
    }

    private bool CanStartChat()
    {
        if (playerTransform == null || enemyHealth == null) return false;
        return enemyHealth.isFinishedAndTalkable && (Vector3.Distance(transform.position, playerTransform.position) <= interactionDistance);
    }

    public void StartChat()
    {
        isChatting = true;
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        chatPanel.SetActive(true);
        chatLogText.text = "";
        chatHistory.Clear();
        StartCoroutine(AddMessageToLog("<b>Trần Lực:</b> Khá lắm... *khụ*... Cậu cảnh sát. Cuối cùng cũng tìm được đến đây."));
        chatHistory.Add(new ChatMessage { type = "npc", message = "Khá lắm... *khụ*... Cậu cảnh sát. Cuối cùng cũng tìm được đến đây." });

        playerInputField.text = "";
        playerInputField.ActivateInputField();
        playerInputField.Select();
        playerInputField.onEndEdit.RemoveAllListeners();
        playerInputField.onEndEdit.AddListener(OnPlayerSendMessage);

        if (allyCommunicateButton != null) allyCommunicateButton.gameObject.SetActive(true);

        currentDialogueIndex = 0;
        allyChatLogText.text = "";
        isAllyPanelVisible = false;
    }

    public void EndChat()
    {
        isChatting = false;
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        chatPanel.SetActive(false);
        if (allyCommunicateButton != null) allyCommunicateButton.gameObject.SetActive(false);

        if (isAllyPanelVisible)
        {
            if (allyPanelFadeCoroutine != null) StopCoroutine(allyPanelFadeCoroutine);
            if (allyPanelCanvasGroup != null) allyPanelCanvasGroup.alpha = 0f;
        }

        if (allyMessageCoroutine != null) StopCoroutine(allyMessageCoroutine);
    }

    private void ToggleAllyChat()
    {
        isAllyPanelVisible = !isAllyPanelVisible;

        if (allyPanelFadeCoroutine != null) StopCoroutine(allyPanelFadeCoroutine);

        if (isAllyPanelVisible)
        {
            if (allyChatLogText.text == "")
            {
                allyChatLogText.text = "<i>[Kênh liên lạc mở - Gợi ý từ chỉ huy]</i>";
            }
            if (allyMessageCoroutine == null)
            {
                allyMessageCoroutine = StartCoroutine(ShowAllyMessagesRoutine());
            }
        }
        else
        {
            if (allyMessageCoroutine != null)
            {
                StopCoroutine(allyMessageCoroutine);
                allyMessageCoroutine = null;
            }
        }
        allyPanelFadeCoroutine = StartCoroutine(FadeAllyPanel(isAllyPanelVisible));
    }

    private IEnumerator FadeAllyPanel(bool fadeIn)
    {
        float startAlpha = allyPanelCanvasGroup.alpha;
        float endAlpha = fadeIn ? 1f : 0f;
        float time = 0f;

        allyPanelCanvasGroup.interactable = fadeIn;
        allyPanelCanvasGroup.blocksRaycasts = fadeIn;

        while (time < fadeDuration)
        {
            allyPanelCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        allyPanelCanvasGroup.alpha = endAlpha;
    }

    private IEnumerator ShowAllyMessagesRoutine()
    {
        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            if (currentDialogueIndex >= masterAllyDialogues.Count)
            {
                if (!allyChatLogText.text.Contains("[Hết gợi ý]"))
                {
                    AddAllyMessageToLog("<i>[Hết gợi ý]</i>");
                }
                break;
            }

            string nextMessage = masterAllyDialogues[currentDialogueIndex];
            currentDialogueIndex++;

            AddAllyMessageToLog(nextMessage);

            yield return new WaitForSeconds(1.0f);
        }
    }

    private void AddAllyMessageToLog(string message)
    {
        if (allyChatLogText != null)
        {
            string formattedMessage;
            if (message.StartsWith("<i>"))
            {
                formattedMessage = message;
            }
            else
            {
                formattedMessage = $"<b>Gợi ý:</b> {message}";
            }

            allyChatLogText.text += "\n" + formattedMessage;
        }
    }

    private void OnPlayerSendMessage(string message)
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && !string.IsNullOrWhiteSpace(message))
        {
            StartCoroutine(AddMessageToLog("<b>Hùng:</b> " + message));
            chatHistory.Add(new ChatMessage { type = "player", message = message });
            StartCoroutine(SendRequestToServer(message));
            playerInputField.text = "";
            playerInputField.ActivateInputField();
            playerInputField.Select();
        }
    }

    private IEnumerator SendRequestToServer(string playerInput)
    {
        StartCoroutine(AddMessageToLog("<b>Trần Lực:</b> <i>...suy nghĩ...</i>"));
        ChatPayload payload = new ChatPayload { input = playerInput, history = this.chatHistory };
        string jsonPayload = JsonUtility.ToJson(payload);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            RemoveLastLineFromLog();
            if (request.result == UnityWebRequest.Result.Success)
            {
                ChatResponse response = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
                StartCoroutine(AddMessageToLog("<b>Trần Lực:</b> " + response.answer));
                chatHistory.Add(new ChatMessage { type = "npc", message = response.answer });
            }
            else
            {
                Debug.LogError("Error from server: " + request.error + " | " + request.downloadHandler.text);
                StartCoroutine(AddMessageToLog("<b>Trần Lực:</b> <i>(Hắn ta gục xuống, kiệt sức.)</i>"));
            }
        }
    }

    private IEnumerator AddMessageToLog(string message)
    {
        chatLogText.text += (string.IsNullOrEmpty(chatLogText.text) ? "" : "\n") + message;
        yield return new WaitForEndOfFrame();
        if (chatScrollRect != null) chatScrollRect.verticalNormalizedPosition = 0f;
    }

    private void RemoveLastLineFromLog()
    {
        if (string.IsNullOrEmpty(chatLogText.text)) return;
        int lastNewLine = chatLogText.text.LastIndexOf("\n");
        chatLogText.text = (lastNewLine >= 0) ? chatLogText.text.Substring(0, lastNewLine) : "";
    }
}