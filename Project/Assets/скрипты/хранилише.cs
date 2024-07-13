using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    // Список для хранения объектов
    private List<GameObject> storedObjects = new List<GameObject>();

    // Тег объектов, которые должны быть собраны
    public string collectibleTag = "Collectible";

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверка, имеет ли объект определенный тег
        if (other.CompareTag(collectibleTag))
        {
            // Добавление объекта в список
            storedObjects.Add(other.gameObject);

            // Отключение объекта из сцены (можно удалить, если нужно)
            other.gameObject.SetActive(false);
        }
    }

    // Метод для получения списка объектов
    public List<GameObject> GetStoredObjects()
    {
        return storedObjects;
    }
}
