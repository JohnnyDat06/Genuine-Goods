// File: RaidDialogueStage.cs
using UnityEngine;

[System.Serializable]
public class RaidDialogueStage
{
    [Header("Hội thoại khi Raid mới bắt đầu")]
    [Tooltip("Hội thoại 'cảm ơn' hoặc 'ghi nhận'. Chỉ nói 1 lần khi raid này vừa được mở khóa.")]
    public DialogueObject AcknowledgmentDialogue;

    [Header("Hội thoại điều tra chính")]
    [Tooltip("Hội thoại chứa manh mối chính. Chỉ nói 1 lần.")]
    public DialogueObject MainDialogue;

    [Header("Hội thoại lặp lại trong Raid này")]
    [Tooltip("Hội thoại sẽ lặp lại sau khi đã nói hết các thoại ở trên.")]
    public DialogueObject RepeatingDialogue;
}