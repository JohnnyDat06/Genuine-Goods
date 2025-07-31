using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrusherTrap : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float knockbackForce = 15f;
    [SerializeField] private Vector2 knockbackDirection = new Vector2(1, 1);
    [Header("Sound FX")]
    [SerializeField] private AudioClip hitSound;
    private List<GameObject> playersInZone = new List<GameObject>();
    
    // HÀM MỚI: Dành cho script con gọi khi player đi vào
    private AudioSource audioSource;
    private void Start()
    {
     
        audioSource = GetComponent<AudioSource>();
    }
    public void AddPlayerToList(GameObject player)
    {
        if (player.CompareTag("Player") && !playersInZone.Contains(player))
        {
            playersInZone.Add(player);
            Debug.Log("Player entered a danger zone!");
        }
    }

    // HÀM MỚI: Dành cho script con gọi khi player đi ra
    public void RemovePlayerFromList(GameObject player)
    {
        if (player.CompareTag("Player") && playersInZone.Contains(player))
        {
            playersInZone.Remove(player);
            Debug.Log("Player exited the danger zone!");
        }
    }

    // Hàm này vẫn giữ nguyên, được gọi bằng Animation Event
    public void ActivateCrush()
    {
        
        if (playersInZone.Count == 0) return;
        bool soundHasPlayed = false;
        // Tạo một bản copy của list để tránh lỗi khi player chết và bị remove khỏi list gốc
        List<GameObject> playersToDamage = new List<GameObject>(playersInZone);

        foreach (GameObject player in playersToDamage)
        {
            if (player == null) continue; // Bỏ qua nếu player đã bị hủy

            // Gây sát thương
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                DamageFlashEffect.instance.FlashScreen();
            }

            // Gây hiệu ứng đẩy lùi
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = Vector2.zero;
                float knockbackDirectionX = player.transform.position.x > transform.position.x ? 1 : -1;
                Vector2 finalKnockback = new Vector2(knockbackDirection.x * knockbackDirectionX, knockbackDirection.y).normalized;
                playerRb.AddForce(finalKnockback * knockbackForce, ForceMode2D.Impulse);
            }
            if (!soundHasPlayed && audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
                soundHasPlayed = true; // Đánh dấu là đã bật rồi để không bật lại nữa
            }
        }
    }
}