using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionMap : MonoBehaviour
{
    public static bool isOpen = false;
    [SerializeField] private GameObject mapUI;
    
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (isOpen)
            {
                mapUI.SetActive(false);
                isOpen = false;
            }
            else
            {
                mapUI.SetActive(true);
                isOpen = true;
            }
        }
    }

    public void MapError()
    {
        SceneManager.LoadScene("MapError");
    }

    public void Map2()
    {
        SceneManager.LoadScene("Map2");
    }

    public void Map1()
    {
        SceneManager.LoadScene("Map1");
    }
}
