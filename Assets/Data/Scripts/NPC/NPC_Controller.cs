using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float moveDistance = 3f;

    private Vector3 startPosition;
    private int direction = 1;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Di chuyển qua lại trong một khoảng cách nhất định
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        if (Vector3.Distance(startPosition, transform.position) > moveDistance)
        {
            direction *= -1; // Đảo chiều
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z); // Lật hình
        }
    }
}