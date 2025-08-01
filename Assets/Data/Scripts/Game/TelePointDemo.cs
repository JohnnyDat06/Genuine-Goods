using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TelePointDemo : MonoBehaviour
{
    [SerializeField] private GameObject player;
    
    [SerializeField] private List<GameObject> points = new List<GameObject>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) player.transform.position = points[0].transform.position;
        if (Input.GetKeyDown(KeyCode.F2)) player.transform.position = points[1].transform.position;
        if (Input.GetKeyDown(KeyCode.F3)) player.transform.position = points[2].transform.position;
        if (Input.GetKeyDown(KeyCode.F4)) player.transform.position = points[3].transform.position;
        if (Input.GetKeyDown(KeyCode.F5)) player.transform.position = points[4].transform.position;
        if (Input.GetKeyDown(KeyCode.F6)) player.transform.position = points[5].transform.position;
        if (Input.GetKeyDown(KeyCode.F7)) player.transform.position = points[6].transform.position;
        if (Input.GetKeyDown(KeyCode.F8)) player.transform.position = points[7].transform.position;
        if (Input.GetKeyDown(KeyCode.F9)) SceneManager.LoadScene("FinalBoss");
        
    }
}
