using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneSwitcher : MonoBehaviour
{
    public Button yourButton; // Присвойте сюда вашу кнопку через инспектор

    void Start()
    {
        if (yourButton != null)
        {
            yourButton.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        StartCoroutine(SwitchSceneWithDelay(1f)); // Задержка в 1 секунду
    }

    IEnumerator SwitchSceneWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("multiplayer");
    }
}
