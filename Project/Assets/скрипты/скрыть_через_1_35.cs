using UnityEngine;

public class HideObjectAfterDelay : MonoBehaviour
{
    void Start()
    {
        // Вызываем метод HideObject через 1.35 секунды
        Invoke("HideObject", 1.7f);
    }

    void HideObject()
    {
        // Скрываем объект
        gameObject.SetActive(false);
    }
}
