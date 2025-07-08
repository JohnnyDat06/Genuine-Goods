using UnityEngine;

public class BirdController : MonoBehaviour
{
    [SerializeField] private Transform cat;
    [SerializeField] private Transform[] landingSpots;
    [SerializeField] private float triggerDistance = 2f;
    [SerializeField] private float flyCurveHeight = 2f;
    [SerializeField] private float flyDuration = 2f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform currentSpot;
    private bool isFlying = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentSpot = GetClosestLandingSpot(transform.position);
    }

    private void Update()
    {
        if (isFlying || cat == null) return;

        float distance = Vector3.Distance(cat.position, transform.position);
        if (distance < triggerDistance)
        {
            Transform nextSpot = GetRandomNewSpot();
            if (nextSpot != null)
                StartCoroutine(FlyToSpot(nextSpot));
        }
    }

    private System.Collections.IEnumerator FlyToSpot(Transform targetSpot)
    {
        isFlying = true;

        // Lật hướng khi cần
        if (targetSpot.position.x < transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        // Gọi animation bay
        if (animator != null)
        {
            animator.ResetTrigger("Idle"); // đảm bảo không bị Idle giữa chừng
            animator.SetTrigger("Fly");
        }

        Vector3 start = transform.position;
        Vector3 end = targetSpot.position;
        Vector3 peak = (start + end) / 2f + Vector3.up * flyCurveHeight;

        float timer = 0f;

        while (timer < flyDuration)
        {
            float t = timer / flyDuration;

            Vector3 a = Vector3.Lerp(start, peak, t);
            Vector3 b = Vector3.Lerp(peak, end, t);
            transform.position = Vector3.Lerp(a, b, t);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
        currentSpot = targetSpot;

        // Cho phép chuyển lại Idle
        if (animator != null)
            animator.SetTrigger("Idle");

        isFlying = false;
    }

    private Transform GetRandomNewSpot()
    {
        if (landingSpots.Length <= 1) return null;

        Transform newSpot;
        do
        {
            newSpot = landingSpots[Random.Range(0, landingSpots.Length)];
        }
        while (newSpot == currentSpot);

        return newSpot;
    }

    private Transform GetClosestLandingSpot(Vector3 position)
    {
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (var spot in landingSpots)
        {
            float dist = Vector3.Distance(position, spot.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = spot;
            }
        }
        return closest;
    }
}
