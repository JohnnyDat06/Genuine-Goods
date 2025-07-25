using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public float startHealth;
    [SerializeField] private float numberOfHearts;
    [SerializeField] private GameManagers gameManager;
    public float currentHeath {get; private set;}

    [SerializeField] private PlayerController playerMovement;
    
    [SerializeField] private float deathAnimationDuration = 1.5f;
    private bool isDead = false;

    private void Awake()
    {
        currentHeath = startHealth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isDead)
        {
            TakeHealth(100);
        }
    }

    public void TakeDamage(float _damage)
    {
        if(isDead) return;
        currentHeath = Mathf.Clamp(currentHeath - _damage, 0, startHealth);
        if (currentHeath <= 0 && !isDead)
        {
            StartCoroutine(HandleDeathSequence());
        }
        Debug.Log(currentHeath);
    }
    
    private void TakeHealth(float _health)
    {
        if(isDead) return;
        if (numberOfHearts > 0)
        {
            currentHeath = Mathf.Clamp(currentHeath + _health, 0, startHealth);
            numberOfHearts--;
        }
        Debug.Log(currentHeath);
    }

    private IEnumerator HandleDeathSequence()
    {
        isDead = true;
        if (playerMovement != null) playerMovement.TriggerDeathAnimation();
        foreach (var component in GetComponents<MonoBehaviour>())
        {
            if (component != this)
            {
                component.enabled = false;
            }
        }
        yield return new WaitForSeconds(deathAnimationDuration);
        
        gameManager.GameOver();
    }
}
