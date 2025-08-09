using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaChuBanHang : MonoBehaviour
{
    [SerializeField] private GameObject panelSell;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private KeyCode exitKey = KeyCode.Escape;
    [SerializeField] private float interactionDistance = 2f;

    private Transform player;
    private Transform playerTransform => player ??= GameObject.FindGameObjectWithTag("Player").transform;
    private bool isPlayerInRange;
    private bool isChatting;

    void Start()
    {
        if (panelSell == null)
        {
            Debug.LogError("Panel Sell is not assigned in the inspector.");
        }
        else
        {
            panelSell.SetActive(false);
        }
    }

    void Update()
    {
        if (playerTransform == null) return;
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        isPlayerInRange = (distance <= interactionDistance);

        // Toggle chat panel with E key
        if (isPlayerInRange && Input.GetKeyDown(interactionKey))
        {
            if (!isChatting)
            {
                StartChat();
            }
            else
            {
                EndChat();
            }
        }
        else if (isChatting && Input.GetKeyDown(exitKey))
        {
            EndChat();
        }
    }

    private void StartChat()
    {
        isChatting = true;
        panelSell.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    private void EndChat()
    {
        isChatting = false;
        panelSell.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }
}
