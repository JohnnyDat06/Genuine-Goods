using UnityEngine;

// Định nghĩa một cấu trúc để chứa cả tên, lời thoại và âm thanh
[System.Serializable]
public struct DialogueLine
{
    public string characterName;
    [TextArea(3, 10)]
    public string sentence;

    // DÒNG MỚI: Thêm trường để chứa âm thanh giọng nói
    public AudioClip voiceClip;
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue")]
public class DialogueObject : ScriptableObject
{
    // Sử dụng một mảng các DialogueLine để chứa toàn bộ hội thoại
    [SerializeField] private DialogueLine[] dialogueLines;

    // Property để các script khác có thể truy cập vào mảng hội thoại
    public DialogueLine[] DialogueLines => dialogueLines;
}