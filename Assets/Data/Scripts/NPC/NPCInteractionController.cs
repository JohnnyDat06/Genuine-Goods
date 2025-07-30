using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using TMPro;
using UnityEngine.UI;

// --- Cấu trúc dữ liệu để giao tiếp với Server (Giữ nguyên) ---

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

[RequireComponent(typeof(EnemyHealth))] // Đảm bảo luôn có component EnemyHealth
public class NPCInteractionController : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Khoảng cách tối đa để người chơi có thể bắt đầu cuộc trò chuyện.")]
    public float interactionDistance = 3.0f;
    [Tooltip("Phím để bắt đầu/gửi tin nhắn.")]
    public KeyCode interactionKey = KeyCode.E;
    [Tooltip("Phím để kết thúc cuộc trò chuyện.")]
    public KeyCode exitKey = KeyCode.Escape;

    [Header("UI Elements")]
    [Tooltip("Panel chứa toàn bộ giao diện chat.")]
    public GameObject chatPanel;
    [Tooltip("Vùng hiển thị nội dung chat.")]
    public TextMeshProUGUI chatLogText;
    [Tooltip("Ô để người chơi nhập tin nhắn.")]
    public TMP_InputField playerInputField;
    [Tooltip("Thanh cuộn của vùng chat để tự động cuộn xuống.")]
    public ScrollRect chatScrollRect;

    [Header("Backend Settings")]
    [Tooltip("Địa chỉ URL của server chat backend.")]
    public string serverUrl = "http://localhost:3000/api/chat";

    // --- Biến nội bộ ---
    private Transform playerTransform;
    private MonoBehaviour playerMovementScript;
    private EnemyHealth enemyHealth;

    private bool isChatting = false;
    private List<ChatMessage> chatHistory = new List<ChatMessage>();

    void Awake()
    {
        // Tự động lấy các component cần thiết
        enemyHealth = GetComponent<EnemyHealth>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            // Giả sử script di chuyển của bạn tên là "PlayerController" hoặc tương tự
            // Hãy thay đổi "PlayerController" thành tên script di chuyển chính xác của bạn
            playerMovementScript = playerObj.GetComponent<PlayerController>() as MonoBehaviour;
        }
        else
        {
            Debug.LogError("Không tìm thấy đối tượng có tag 'Player'. Vui lòng kiểm tra lại scene.", this);
        }
    }

    void Start()
    {
        if (chatPanel != null)
        {
            chatPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Chưa gán Chat Panel vào Inspector!", this);
        }
    }

    void Update()
    {
        if (isChatting)
        {
            // Nếu đang chat, chỉ lắng nghe phím thoát
            if (Input.GetKeyDown(exitKey))
            {
                EndChat();
            }
        }
        else
        {
            // Nếu chưa chat, kiểm tra điều kiện để bắt đầu
            if (CanStartChat())
            {
                // Có thể thêm một icon nhỏ để báo hiệu cho người chơi biết họ có thể tương tác
                if (Input.GetKeyDown(interactionKey))
                {
                    StartChat();
                }
            }
        }
    }

    private bool CanStartChat()
    {
        if (playerTransform == null || enemyHealth == null) return false;

        // Điều kiện để bắt đầu chat: Boss đã bị kết liễu VÀ người chơi ở trong tầm tương tác
        bool isBossDefeatedAndReady = enemyHealth.isFinishedAndTalkable;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        bool isPlayerInRange = (distanceToPlayer <= interactionDistance);

        return isBossDefeatedAndReady && isPlayerInRange;
    }

    public void StartChat()
    {
        isChatting = true;
        Debug.Log("Bắt đầu cuộc trò chuyện với Quản lý đã gục ngã...");

        // Vô hiệu hóa điều khiển của người chơi
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        // Hiển thị và thiết lập lại UI
        chatPanel.SetActive(true);
        chatLogText.text = "";
        chatHistory.Clear();
        // DÒNG THOẠI KHỞI ĐẦU ĐÃ ĐƯỢC CẬP NHẬT
        StartCoroutine(AddMessageToLog("<b>Quản lý:</b> Hah... hah... là cậu..."));

        playerInputField.text = "";
        playerInputField.ActivateInputField();
        playerInputField.Select();
        playerInputField.onEndEdit.RemoveAllListeners(); // Xóa listener cũ để tránh trùng lặp
        playerInputField.onEndEdit.AddListener(OnPlayerSendMessage);
    }

    public void EndChat()
    {
        isChatting = false;
        Debug.Log("Cuộc trò chuyện kết thúc. Quản lý đã bị bắt giữ...");

        // Kích hoạt lại điều khiển của người chơi
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        // Ẩn UI và hủy đối tượng Boss
        chatPanel.SetActive(false);
        Destroy(gameObject, 0.5f); // Hủy boss sau khi nói chuyện xong
    }

    private void OnPlayerSendMessage(string message)
    {
        // Chỉ gửi khi nhấn Enter và tin nhắn không rỗng
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && !string.IsNullOrWhiteSpace(message))
        {
            // Thêm tin nhắn của người chơi vào log và lịch sử
            StartCoroutine(AddMessageToLog("<b>Cảnh sát:</b> " + message)); // Đổi tên người nói thành "Cảnh sát"
            chatHistory.Add(new ChatMessage { type = "player", message = message });

            // Gửi yêu cầu tới server
            StartCoroutine(SendRequestToServer(message));

            // Xóa và kích hoạt lại ô nhập liệu
            playerInputField.text = "";
            playerInputField.ActivateInputField();
        }
    }

    private IEnumerator SendRequestToServer(string playerInput)
    {
        StartCoroutine(AddMessageToLog("<b>Quản lý:</b> <i>...thở dốc...</i>"));

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

            RemoveLastLineFromLog(); // Xóa dòng "...thở dốc..."

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                ChatResponse response = JsonUtility.FromJson<ChatResponse>(jsonResponse);
                string npcMessage = response.answer;

                StartCoroutine(AddMessageToLog("<b>Quản lý:</b> " + npcMessage));
                chatHistory.Add(new ChatMessage { type = "npc", message = npcMessage });
            }
            else
            {
                Debug.LogError("Error from server: " + request.error + " | " + request.downloadHandler.text);
                StartCoroutine(AddMessageToLog("<b>Quản lý:</b> <i>(Hắn ta lịm đi vì kiệt sức.)</i>"));
            }
        }
    }

    private IEnumerator AddMessageToLog(string message)
    {
        if (string.IsNullOrEmpty(chatLogText.text))
        {
            chatLogText.text = message;
        }
        else
        {
            chatLogText.text += "\n" + message;
        }

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
        if (lastNewLine > 0)
        {
            chatLogText.text = chatLogText.text.Substring(0, lastNewLine);
        }
        else
        {
            chatLogText.text = ""; // Nếu chỉ có một dòng
        }
    }
}