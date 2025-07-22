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

    public void SelectLocation(string locationName)
    {
        switch (MissionManager.Instance.currentRaid)
        {
            case 0:
                if (locationName == "Map1")
                {
                    MissionManager.Instance.CorrectLocation();
                    SceneManager.LoadScene("Map1");
                }
                else ShowError();
                break;

            case 1:
                if (locationName == "Map2")
                {
                    MissionManager.Instance.CorrectLocation();
                    SceneManager.LoadScene("Map2");
                }
                else ShowError();
                break;

            case 2:
                if (locationName == "Map3")
                {
                    MissionManager.Instance.CorrectLocation();
                    SceneManager.LoadScene("Map3");
                }
                else ShowError();
                break;
        }
    }

    public void ShowError()
    {
        MissionManager.Instance.WrongLocation();
        SceneManager.LoadScene("MapError");
    }

}
