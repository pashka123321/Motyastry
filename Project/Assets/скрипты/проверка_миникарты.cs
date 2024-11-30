using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    public GameObject[] miniMapParts;

    void Start()
    {
        // Проверяем состояние мини-карты в настройках
        bool isMiniMapEnabled = PlayerPrefs.GetInt("MiniMapEnabled", 1) == 1;

        // Включаем или отключаем все части мини-карты в зависимости от состояния
        foreach (GameObject part in miniMapParts)
        {
            part.SetActive(isMiniMapEnabled);
        }
    }
}