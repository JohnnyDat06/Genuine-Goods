using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageFlashEffect : MonoBehaviour
{
    // === THÊM ĐOẠN NÀY VÀO ===
    public static DamageFlashEffect instance; // Biến static để có thể gọi từ bất cứ đâu
    // ===========================

    public Image flashImage;
    public float flashDuration = 0.15f;

    // === THÊM HÀM AWAKE() NÀY VÀO ===
    private void Awake()
    {
        // Gán chính nó vào biến instance để các script khác có thể truy cập
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Nếu đã tồn tại, hủy bản sao để đảm bảo chỉ có 1
        }
    }
    // ===================================

    public void FlashScreen()
    {
        if (flashImage != null)
        {
            StopAllCoroutines();
            StartCoroutine(FlashCoroutine());
        }
        else
        {
            Debug.LogError("Damage Flash Image is not assigned in the Inspector!");
        }
    }

    private IEnumerator FlashCoroutine()
    {
        
        flashImage.gameObject.SetActive(true); 

        yield return new WaitForSeconds(flashDuration);

        
        flashImage.gameObject.SetActive(false); 
    }
}