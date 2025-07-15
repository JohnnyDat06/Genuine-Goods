using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float startingHealth = 3f;

    public float currentHealth { get; private set; }
    private Animator anim;
    private Transform player;

    private bool isDead;
    private bool preventRespawn = false;
    private bool isPlayerNearby = false;

    protected virtual void Awake()
    {
        currentHealth = startingHealth;
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void Update()
    {
        if (isDead && isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            preventRespawn = true;
        }
        
        CheckPlayer();
    }
    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, startingHealth);
        Debug.Log(currentHealth);
        if (currentHealth > 0)
        {
            if (anim != null) anim.SetTrigger("IsHit");
        }
        else
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Enemy chết rồi!");

        if (anim != null) anim.SetBool("IsDeath", isDead);

        StartCoroutine(WaitForKillOrRespawn(5f));
    }

    IEnumerator WaitForKillOrRespawn(float delay)
    {
        float timer = 0f;

        // Trong vòng delay giây, chờ người chơi nhấn E
        while (timer < delay)
        {
            if (preventRespawn)
            {
                Debug.Log("Đã bị khóa còng");
                Destroy(gameObject, 10f); // Xóa quái vật
                yield break; // Thoát coroutine, không hồi sinh nữa
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Nếu hết thời gian mà không bị kết liễu thì hồi sinh
        currentHealth = Mathf.RoundToInt(startingHealth * 0.3f);
        isDead = false;
        Debug.Log("Enemy hồi sinh với máu: " + currentHealth);

        if (anim != null) anim.SetBool("IsDeath", isDead);
    }


    void CheckPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer < 1f && isDead)
        {
            isPlayerNearby = true;
        }
        else
        {
            isPlayerNearby = false;
        }
    }
}
