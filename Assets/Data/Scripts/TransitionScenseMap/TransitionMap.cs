using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
