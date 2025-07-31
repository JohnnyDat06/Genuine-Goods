using UnityEngine;

public class NPC_Controller : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float moveDistance = 3f;

    private Vector3 pointA;
    private Vector3 pointB;
    private int direction = 1;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        pointA = transform.position;
        pointB = new Vector3(transform.position.x + moveDistance, transform.position.y, transform.position.z);
    }

    private void OnDisable()
    {
        if (animator != null) animator.SetBool("isWalking", false);
    }

    void OnEnable()
    {
        if (animator != null) animator.SetBool("isWalking", true);
    }

    void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
        if ((direction == 1 && transform.position.x >= pointB.x) || (direction == -1 && transform.position.x <= pointA.x))
        {
            Flip();
        }
    }

    private void Flip()
    {
        direction *= -1;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
