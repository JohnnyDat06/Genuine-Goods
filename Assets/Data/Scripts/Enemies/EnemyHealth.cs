using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float startingHealth = 3f;
    [SerializeField] private NinjaController ninjaController;
    [SerializeField] private GameObject enemyObj;
    public float currentHealth { get; private set; }
    private Animator anim;
    private Transform player;

    public bool isDead;
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
        ninjaController.BeAttack(0.6f, 0.1f);
    }
    
    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Enemy chết rồi!");

        if (anim != null) anim.SetTrigger("IsDeath");
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        
        StartCoroutine(WaitForKillOrRespawn(5f));
    }

    IEnumerator WaitForKillOrRespawn(float delay)
    {
        float timer = 0f;
        
        while (timer < delay)
        {
            if (preventRespawn)
            {
                Debug.Log("Đã bị khóa còng");
                Destroy(enemyObj, 10f);
                yield break; 
            }

            timer += Time.deltaTime;
            yield return null;
        }
        
        currentHealth = Mathf.RoundToInt(startingHealth * 0.3f);
        isDead = false;
        Debug.Log("Enemy hồi sinh với máu: " + currentHealth);
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        rb.isKinematic = false;
        
        if (anim != null) anim.SetTrigger("IsRevive");
        ninjaController.Revive();
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
