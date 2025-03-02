using UnityEngine;

public class PreventPause : MonoBehaviour
{
    void Awake()
    {
        Application.runInBackground = true; // Позволяет игре работать в фоне
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Debug.Log("Игра свернута, но не останавливается.");
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Debug.Log("Окно потеряло фокус, но игра продолжает работать.");
        }
    }
}