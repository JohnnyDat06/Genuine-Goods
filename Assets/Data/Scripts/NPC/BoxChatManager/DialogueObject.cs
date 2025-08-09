// File: DialogueObject.cs
// Phiên bản đầy đủ, hỗ trợ lựa chọn đúng/sai và chuyển scene

using UnityEngine;

[System.Serializable]
public struct DialogueChoice
{
    [Tooltip("Nội dung sẽ hiển thị trên nút lựa chọn.")]
    public string choiceText;

    [Tooltip("Đoạn hội thoại sẽ bắt đầu nếu người chơi chọn lựa chọn này (nếu chọn sai).")]
    public DialogueObject nextDialogue;

    [Header("Correct Choice Settings")]
    [Tooltip("Đánh dấu vào đây nếu đây là lựa chọn ĐÚNG.")]
    public bool isCorrectChoice;

    [Tooltip("Tên của Scene sẽ được tải nếu đây là lựa chọn đúng. (Phải giống hệt tên file Scene)")]
    public string sceneToLoad;
}

[System.Serializable]
public class ChoiceData
{
    [Tooltip("Danh sách các lựa chọn cho người chơi.")]
    public DialogueChoice[] choices;
}

[System.Serializable]
public struct DialogueLine
{
    public string characterName;
    [TextArea(3, 10)]
    public string sentence;
    public AudioClip voiceClip;

    [Tooltip("Thêm lựa chọn vào cuối dòng thoại này. Bỏ trống nếu không có.")]
    public ChoiceData choiceData;

    public bool HasChoices => choiceData != null && choiceData.choices.Length > 0;
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue")]
public class DialogueObject : ScriptableObject
{
    [SerializeField] private DialogueLine[] dialogueLines;

    [Header("Follow-Up")]
    [Tooltip("Đoạn hội thoại này sẽ tự động chạy sau khi các nhánh lựa chọn ở trên kết thúc.")]
    [SerializeField] private DialogueObject followUpDialogue;

    public DialogueLine[] DialogueLines => dialogueLines;
    public DialogueObject FollowUpDialogue => followUpDialogue;
}