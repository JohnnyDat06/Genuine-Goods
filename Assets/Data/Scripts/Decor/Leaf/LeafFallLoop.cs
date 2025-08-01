using UnityEngine;
using System.Collections;

public class LeafFallLoop : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float animationDuration = 0.5f; // Thời gian thật của anim (tính tự động)

    // Thời gian ngẫu nhiên giữa các lần xuất hiện lại
    [SerializeField] private float minDelay = 3f;
    [SerializeField] private float maxDelay = 7f;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Lấy thời gian của animation (nếu animator đã active)
        animationDuration = animator.runtimeAnimatorController.animationClips[0].length;

        StartCoroutine(PlayFallLoop());
    }

    IEnumerator PlayFallLoop()
    {
        while (true)
        {
            // Bật SpriteRenderer
            spriteRenderer.enabled = true;

            // Reset & phát lại animation từ đầu
            animator.Rebind();
            animator.Update(0f);
            animator.Play("Leaf_Fall", 0, 0f);

            // Đợi animation chạy xong
            yield return new WaitForSeconds(animationDuration);

            // Ẩn lá sau khi rơi xong
            spriteRenderer.enabled = false;

            // Random thời gian đợi trước lần tiếp theo
            float randomDelay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(randomDelay);
        }
    }
}
