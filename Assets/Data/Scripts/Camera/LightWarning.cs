using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using System.Collections;
using UnityEngine.Rendering.Universal; // Cần thiết để điều khiển Light2D

[RequireComponent(typeof(AudioSource))]
public class PatrollingWarningCamera : MonoBehaviour
{
    [Header("Patrol Points")]
    [Tooltip("Đối tượng Transform định vị điểm bắt đầu (A).")]
    [SerializeField] private Transform pointA;
    [Tooltip("Đối tượng Transform định vị điểm kết thúc (B).")]
    [SerializeField] private Transform pointB;

    [Header("Movement Settings")]
    [Tooltip("Tốc độ di chuyển của camera.")]
    [SerializeField] private float moveSpeed = 2f;
    [Tooltip("Thời gian (giây) camera sẽ tắt ở mỗi điểm.")]
    [SerializeField] private float waitTime = 3f;

    [Header("Detection Setup")]
    [SerializeField] private Transform playerTransform;
    [SerializeField]
    private Vector2[] detectionVertices = new Vector2[3] {
        new Vector2(0, 0), new Vector2(-2, -4), new Vector2(2, -4)
    };
    [Tooltip("Các layer vật thể có thể chặn tầm nhìn của camera (ví dụ: Ground, Obstacles).")]
    [SerializeField] private LayerMask lineOfSightMask;

    [Header("Effects Setup")]
    [SerializeField] private TextMeshProUGUI warningTextComponent;
    [SerializeField] private Volume postProcessingVolume;
    [SerializeField] private AudioClip warningSound;
    [Tooltip("(Tùy chọn) Ánh sáng 2D để thể hiện camera đang bật/tắt.")]
    [SerializeField] private Light2D cameraLight;

    [Header("Animation Parameters")]
    [SerializeField] private float blinkSpeed = 2f;
    [SerializeField][Range(0, 1)] private float maxBlinkIntensity = 0.5f;
    [SerializeField] private float scrollSpeed = 100f;
    [SerializeField] private string warningMessage = "!!! DANGER ZONE !!!";
    [SerializeField] private float fadeOutDelay = 2f;

    private AudioSource audioSource;
    private Coroutine activeEffectsCoroutine;
    private Coroutine fadeOutCoroutine;
    private bool isPlayerInZone = false;
    private bool isCameraActive = false;

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

        StartCoroutine(PatrolRoutine());
    }

    void Update()
    {
        if (!isCameraActive || playerTransform == null) return;

        Vector2 playerLocalPosition = transform.InverseTransformPoint(playerTransform.position);
        bool isInPolygon = IsPointInPolygon(playerLocalPosition, detectionVertices);

        bool hasLineOfSight = false;

        if (isInPolygon)
        {
            Vector2 directionToPlayer = playerTransform.position - transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, lineOfSightMask);

            if (hit.collider == null)
            {
                hasLineOfSight = true;
            }
        }

        bool currentlyDetected = isInPolygon && hasLineOfSight;

        if (currentlyDetected != isPlayerInZone)
        {
            isPlayerInZone = currentlyDetected;
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
                fadeOutCoroutine = StartCoroutine(FadeOutEffectsRoutine(fadeOutDelay));
            }
        }
    }

    private IEnumerator PatrolRoutine()
    {
        transform.position = pointA.position;

        while (true)
        {
            ToggleCameraState(true);
            yield return StartCoroutine(MoveToPoint(pointB));

            ToggleCameraState(false);
            yield return new WaitForSeconds(waitTime);

            ToggleCameraState(true);
            yield return StartCoroutine(MoveToPoint(pointA));

            ToggleCameraState(false);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator MoveToPoint(Transform target)
    {
        while (Vector2.Distance(transform.position, target.position) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target.position;
    }

    private void ToggleCameraState(bool isActive)
    {
        isCameraActive = isActive;
        if (cameraLight != null)
        {
            cameraLight.enabled = isActive;
        }

        if (!isActive)
        {
            if (isPlayerInZone)
            {
                isPlayerInZone = false;
                if (fadeOutCoroutine != null) StopCoroutine(fadeOutCoroutine);
                fadeOutCoroutine = StartCoroutine(FadeOutEffectsRoutine(fadeOutDelay));
            }
        }
    }

    private void StartWarningEffects()
    {
        if (warningTextComponent != null) warningTextComponent.gameObject.SetActive(true);
        if (postProcessingVolume != null) postProcessingVolume.gameObject.SetActive(true);
        if (audioSource != null && warningSound != null && !audioSource.isPlaying)
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
        for (int i = 0, j = polygonLength - 1; i < polygonLength; j = i++)
        {
            Vector2 vertexA = polygon[i];
            Vector2 vertexB = polygon[j];
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
                endX = -(canvasWidth / 2f) - (textWidth / 2f);
                textRect.anchoredPosition = new Vector2(startX, textRect.anchoredPosition.y);
                travelDistance = startX - endX;
                duration = travelDistance / scrollSpeed;
            }
            float newX = Mathf.Lerp(startX, endX, scrollTimer / duration);
            textRect.anchoredPosition = new Vector2(newX, textRect.anchoredPosition.y);
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.3f);
            Gizmos.DrawWireSphere(pointB.position, 0.3f);
        }

        if (detectionVertices == null || detectionVertices.Length < 2) return;
        Gizmos.color = isCameraActive ? Color.red : Color.gray;
        for (int i = 0; i < detectionVertices.Length; i++)
        {
            Vector2 currentVertex = transform.TransformPoint(detectionVertices[i]);
            Vector2 nextVertex = transform.TransformPoint(detectionVertices[(i + 1) % detectionVertices.Length]);
            Gizmos.DrawLine(currentVertex, nextVertex);
        }
    }
}
