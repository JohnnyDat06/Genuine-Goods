using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    public int currentRaid = 0;
    public int failCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void CorrectLocation()
    {
        currentRaid++;
    }

    public void WrongLocation()
    {
        failCount++;
    }
}