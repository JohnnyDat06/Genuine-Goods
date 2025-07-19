using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using System.Collections;

[RequireComponent(typeof(AudioSource))] // Yêu cầu đối tượng phải có AudioSource
public class PulsingWarning : MonoBehaviour
{
    [Header("Detection Setup")]
    [Tooltip("Kéo đối tượng Player vào đây.")]
    [SerializeField] private Transform playerTransform;
    [Tooltip("Các đỉnh của vùng tam giác phát hiện. Bạn có thể chỉnh sửa trực tiếp trong Scene View khi chọn Gizmos.")]
    [SerializeField]
    private Vector2[] detectionVertices = new Vector2[3] {
        new Vector2(0, 3), new Vector2(-2, -1), new Vector2(2, -1)
    };

    [Header("Effects Setup")]
    [Tooltip("Component TextMeshProUGUI để hiển thị cảnh báo.")]
    [SerializeField] private TextMeshProUGUI warningTextComponent;
    [Tooltip("GameObject Global Volume chứa hiệu ứng đỏ màn hình.")]
    [SerializeField] private Volume postProcessingVolume;
    [Tooltip("File âm thanh cảnh báo sẽ phát.")]
    [SerializeField] private AudioClip warningSound;

    [Header("Animation Parameters")]
    [Tooltip("Tốc độ màn hình nhấp nháy.")]
    [SerializeField] private float blinkSpeed = 2f;
    [Tooltip("Độ đậm tối đa của hiệu ứng đỏ (từ 0 đến 1).")]
    [SerializeField][Range(0, 1)] private float maxBlinkIntensity = 0.5f;
    [Tooltip("Tốc độ dòng chữ lướt qua màn hình.")]
    [SerializeField] private float scrollSpeed = 100f;
    [Tooltip("Nội dung của dòng chữ cảnh báo.")]
    [SerializeField] private string warningMessage = "!!! DANGER ZONE !!!";

    // --- BIẾN MỚI ĐỂ SỬA THỜI GIAN BÊN NGOÀI ---
    [Tooltip("Thời gian (giây) hiệu ứng tiếp tục sau khi người chơi rời khỏi vùng.")]
    [SerializeField] private float fadeOutDelay = 2f;
    // -----------------------------------------

    private AudioSource audioSource;
    private Coroutine activeEffectsCoroutine;
    private Coroutine fadeOutCoroutine;
    private bool isPlayerInZone = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        if (warningTextComponent != null)
        {
            warningTextComponent.text = warningMessage;
            warningTextComponent.gameObject.SetActive(false);
        }
        if (postProcessingVolume != null)
        {
            postProcessingVolume.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        bool currentlyInZone = IsPointInPolygon(playerTransform.position, detectionVertices);

        if (currentlyInZone != isPlayerInZone)
        {
            isPlayerInZone = currentlyInZone;

            if (isPlayerInZone)
            {
                if (fadeOutCoroutine != null)
                {
                    StopCoroutine(fadeOutCoroutine);
                    fadeOutCoroutine = null;
                }
                StartWarningEffects();
            }
            else
            {
                // --- SỬ DỤNG BIẾN MỚI THAY VÌ SỐ CỨNG ---
                fadeOutCoroutine = StartCoroutine(FadeOutEffectsRoutine(fadeOutDelay));
            }
        }
    }

    private void StartWarningEffects()
    {
        if (warningTextComponent != null) warningTextComponent.gameObject.SetActive(true);
        if (postProcessingVolume != null) postProcessingVolume.gameObject.SetActive(true);

        if (audioSource != null && warningSound != null)
        {
            audioSource.clip = warningSound;
            audioSource.Play();
        }

        if (activeEffectsCoroutine == null)
        {
            activeEffectsCoroutine = StartCoroutine(RunWarningAnimations());
        }
    }

    private void StopWarningEffects()
    {
        if (activeEffectsCoroutine != null)
        {
            StopCoroutine(activeEffectsCoroutine);
            activeEffectsCoroutine = null;
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        if (warningTextComponent != null) warningTextComponent.gameObject.SetActive(false);
        if (postProcessingVolume != null)
        {
            postProcessingVolume.weight = 0;
            postProcessingVolume.gameObject.SetActive(false);
        }
    }

    private IEnumerator FadeOutEffectsRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopWarningEffects();
        fadeOutCoroutine = null;
    }

    public void SetWarningMessage(string newMessage)
    {
        warningMessage = newMessage;
        if (warningTextComponent != null) { warningTextComponent.text = newMessage; }
    }

    private bool IsPointInPolygon(Vector2 point, Vector2[] polygon)
    {
        int polygonLength = polygon.Length;
        if (polygonLength < 3) return false;
        bool isInside = false;
        Vector2 worldPos = transform.position;
        for (int i = 0, j = polygonLength - 1; i < polygonLength; j = i++)
        {
            Vector2 vertexA = worldPos + polygon[i];
            Vector2 vertexB = worldPos + polygon[j];
            if (((vertexA.y > point.y) != (vertexB.y > point.y)) && (point.x < (vertexB.x - vertexA.x) * (point.y - vertexA.y) / (vertexB.y - vertexA.y) + vertexA.x))
            {
                isInside = !isInside;
            }
        }
        return isInside;
    }
    private IEnumerator RunWarningAnimations()
    {
        RectTransform textRect = warningTextComponent.rectTransform;
        Canvas canvas = warningTextComponent.canvas;
        float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        textRect.pivot = new Vector2(0.5f, 0.5f);
        float textWidth, startX = 0, endX = 0, travelDistance, duration = 0;
        float scrollTimer = 999f;
        while (true)
        {
            if (postProcessingVolume != null)
            {
                float blinkValue = Mathf.PingPong(Time.time * blinkSpeed, maxBlinkIntensity);
                postProcessingVolume.weight = blinkValue;
            }
            scrollTimer += Time.deltaTime;
            if (scrollTimer >= duration)
            {
                scrollTimer = 0;
                textWidth = warningTextComponent.preferredWidth;
                startX = (canvasWidth / 2f) + (textWidth / 2f);
                endX = -(canvasWidth / 1f) - (textWidth / 1f);
                textRect.anchoredPosition = new Vector2(startX, textRect.anchoredPosition.y);
                travelDistance = startX - endX;
                duration = travelDistance / scrollSpeed;
            }
            float newX = Mathf.Lerp(startX, endX, scrollTimer / duration);
            textRect.anchoredPosition = new Vector2(newX, textRect.anchoredPosition.y);
            yield return null;
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (detectionVertices == null || detectionVertices.Length < 2) return;
        Gizmos.color = Color.yellow;
        Vector2 worldPos = transform.position;
        for (int i = 0; i < detectionVertices.Length; i++)
        {
            Vector2 currentVertex = worldPos + detectionVertices[i];
            Vector2 nextVertex = worldPos + detectionVertices[(i + 1) % detectionVertices.Length];
            Gizmos.DrawLine(currentVertex, nextVertex);
        }
    }
}
