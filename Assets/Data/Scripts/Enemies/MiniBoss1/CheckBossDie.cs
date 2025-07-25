using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBossDie : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth;
    private bool isMiniBossDead = false;
    void Update()
    {
        if (!isMiniBossDead && enemyHealth.currentHealth <= 0 && enemyHealth.preventRespawn)
        {
            isMiniBossDead = true;
            StartCoroutine(NextScene());
        }
    }
    
    IEnumerator NextScene()
    {
        MissionManager.Instance.CorrectLocation();
        yield return null;
    }
}
