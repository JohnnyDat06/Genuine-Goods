using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/New Dialogue")]
public class DialogueObject : ScriptableObject
{
    // DÒNG MỚI: Thêm biến để lưu tên nhân vật
    [SerializeField] private string characterName;

    [SerializeField]
    [TextArea(3, 10)]
    private string[] dialogueLines;

    // DÒNG MỚI: Tạo một property để truy cập tên nhân vật từ bên ngoài
    public string CharacterName => characterName;
    public string[] DialogueLines => dialogueLines;
}