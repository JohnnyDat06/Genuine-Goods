using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using TMPro;
using UnityEngine.UI;

// --- Cấu trúc dữ liệu để giao tiếp với Server ---

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


public class NPCInteractionController : MonoBehaviour
{
    [Header("Interaction Settings")]
    public Transform playerTransform;
    public float interactionDistance = 3.0f;
    public KeyCode interactionKey = KeyCode.E;
    public KeyCode exitKey = KeyCode.Escape;

    [Header("UI Elements")]
    public GameObject chatPanel;
    public TextMeshProUGUI chatLogText;
    public TMP_InputField playerInputField;
    public ScrollRect chatScrollRect; // Biến cho thanh cuộn

    [Header("Backend Settings")]
    public string serverUrl = "http://localhost:3000/api/chat";

    [Header("Player Components")]
    public MonoBehaviour playerMovementScript;

    private bool isChatting = false;
    private List<ChatMessage> chatHistory = new List<ChatMessage>();

    void Start()
    {
        if (chatPanel != null)
        {
            chatPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool isPlayerInRange = (distance <= interactionDistance);

        if (isPlayerInRange && Input.GetKeyDown(interactionKey) && !isChatting)
        {
            StartChat();
        }
        else if (isChatting && Input.GetKeyDown(exitKey))
        {
            EndChat();
        }
    }

    public void StartChat()
    {
        isChatting = true;
        chatPanel.SetActive(true);

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        // Thiết lập lại cuộc trò chuyện
        chatLogText.text = ""; // Xóa trắng trước
        chatHistory.Clear();
        StartCoroutine(AddMessageToLog("<b>NPC:</b> Xin chào! Tôi có thể giúp gì cho bạn?"));

        playerInputField.ActivateInputField();
        playerInputField.Select();
        playerInputField.onEndEdit.AddListener(OnPlayerSendMessage);
    }

    public void EndChat()
    {
        isChatting = false;
        chatPanel.SetActive(false);

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
        playerInputField.onEndEdit.RemoveListener(OnPlayerSendMessage);
    }

    private void OnPlayerSendMessage(string message)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!string.IsNullOrEmpty(message))
            {
                StartCoroutine(AddMessageToLog("<b>You:</b> " + message));
                chatHistory.Add(new ChatMessage { type = "player", message = message });
                StartCoroutine(SendRequestToServer(message));

                playerInputField.text = "";
                playerInputField.ActivateInputField();
            }
        }
    }

    private IEnumerator SendRequestToServer(string playerInput)
    {
        StartCoroutine(AddMessageToLog("<b>NPC:</b> <i>...đang suy nghĩ...</i>"));

        ChatPayload payload = new ChatPayload
        {
            input = playerInput,
            history = this.chatHistory
        };

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
                string jsonResponse = request.downloadHandler.text;
                ChatResponse response = JsonUtility.FromJson<ChatResponse>(jsonResponse);
                string npcMessage = response.answer;

                StartCoroutine(AddMessageToLog("<b>NPC:</b> " + npcMessage));
                chatHistory.Add(new ChatMessage { type = "npc", message = npcMessage });
            }
            else
            {
                Debug.LogError("Error from server: " + request.error + " | " + request.downloadHandler.text);
                StartCoroutine(AddMessageToLog("<b>NPC:</b> <i>(Xin lỗi, tôi đang gặp sự cố kết nối.)</i>"));
            }
        }
    }

    private IEnumerator AddMessageToLog(string message)
    {
        chatLogText.text += message + "\n";

        // Chờ đến cuối frame để UI cập nhật xong kích thước
        yield return new WaitForEndOfFrame();

        // Buộc thanh cuộn di chuyển xuống dưới cùng
        if (chatScrollRect != null)
        {
            chatScrollRect.verticalNormalizedPosition = 0f;
        }
    }

    private void RemoveLastLineFromLog()
    {
        if (string.IsNullOrEmpty(chatLogText.text)) return;

        int lastNewLine = chatLogText.text.LastIndexOf("\n");
        if (lastNewLine == chatLogText.text.Length - 1)
        {
            int secondToLastNewLine = chatLogText.text.LastIndexOf("\n", lastNewLine - 1);
            if (secondToLastNewLine != -1)
            {
                chatLogText.text = chatLogText.text.Substring(0, secondToLastNewLine + 1);
            }
            else
            {
                // Nếu chỉ có một dòng
                chatLogText.text = "";
            }
        }
    }
}