using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 100;

    private Transform player;
    private Vector2 targetDirection;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player != null)
        {
            targetDirection = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = targetRotation;
        }
        else targetDirection = transform.right;
        rb.velocity = targetDirection * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null) playerHealth.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (!collision.CompareTag("Enemy") && !collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
