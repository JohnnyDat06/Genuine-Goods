using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // <-- THÊM DÒNG NÀY ĐỂ DÙNG EVENT SYSTEM

[RequireComponent(typeof(Button))]
public class CollectedPaperButton : MonoBehaviour
{
    private PaperInteraction associatedPaper;

    public void Setup(PaperInteraction paper)
    {
        associatedPaper = paper;
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        if (associatedPaper != null)
        {
            DocumentManager.instance.ShowPaperFromInventory(associatedPaper);

            // --- DÒNG CODE SỬA LỖI NẰM Ở ĐÂY ---
            // Sau khi nhấn nút, bỏ chọn nó ngay lập tức để phím Space không kích hoạt lại.
            EventSystem.current.SetSelectedGameObject(null);
            // ------------------------------------
        }
    }
}
