using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private PlayerController playerMovement;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        GameIsPaused = false;
        if(pauseMenuUI != null) pauseMenuUI.SetActive(false);
        if (playerMovement != null) playerMovement.enabled = true;
        Time.timeScale = 1f;
    }
    public void Pause()
    {
        GameIsPaused = true;
        if(pauseMenuUI != null) pauseMenuUI.SetActive(true);
        if (playerMovement != null) playerMovement.enabled = false;
        Time.timeScale = 0f;
    }
    public void LoadMenu()
    {
        Debug.Log("Loading menu...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Start");
    }
    public void Play()
    {
        SceneManager.LoadScene("TraDa");
        Time.timeScale = 1f;
    }
    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
    
    public void ReturnMap()
    {
        Invoke("Play", 5f);
    }
}
