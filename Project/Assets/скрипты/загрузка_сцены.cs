using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Update()
    {
        // Проверяем нажатие клавиши F1
        if (Input.GetKeyDown(KeyCode.F1))
        {
            // Загружаем сцену с названием "deep"
            SceneManager.LoadScene("deep");
        }
    }
}
