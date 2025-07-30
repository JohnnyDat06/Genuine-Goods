// File: DialogueObject.cs
using UnityEngine;

[System.Serializable]
public struct DialogueChoice
{
    [Tooltip("Nội dung sẽ hiển thị trên nút lựa chọn.")]
    public string choiceText;

    [Tooltip("Đoạn hội thoại sẽ bắt đầu nếu người chơi chọn lựa chọn này.")]
    public DialogueObject nextDialogue;
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

    // --- TRƯỜNG MỚI ---
    [Header("Follow-Up")]
    [Tooltip("Đoạn hội thoại này sẽ tự động chạy sau khi các nhánh lựa chọn ở trên kết thúc.")]
    [SerializeField] private DialogueObject followUpDialogue;
    // -----------------

    public DialogueLine[] DialogueLines => dialogueLines;
    public DialogueObject FollowUpDialogue => followUpDialogue; // Thêm property
}