using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManagers : MonoBehaviour
{
    public static GameManagers instance;
    
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private PlayerController playerMovement;
    
    
    private bool isGameOver = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    void Start()
    {
        if (deathPanel != null) deathPanel.SetActive(false);
        StartGame();
    }
    
    void Update()
    {
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RestartGame();
            }
        }    
    }

    void StartGame()
    {
        isGameOver = false;
        if (deathPanel != null) deathPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;
        if (deathPanel != null) deathPanel.SetActive(true);
        if (playerMovement != null) playerMovement.enabled = false;
        Time.timeScale = 0f;
    }

    void RestartGame()
    {
        isGameOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (deathPanel != null) deathPanel.SetActive(true);
    }
}
