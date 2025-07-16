using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float startHealth;
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
            TakeDamage(10);
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
        
        // Transition to the Death Scene
    }
}
