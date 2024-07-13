using System.Collections.Generic;
using UnityEngine;

public class ItemStorage : MonoBehaviour
{
    public Transform exitPoint; // Точка выхода

    private List<GameObject> storedItems = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, является ли объект "хранимым предметом" (например, имеет ли он тег "Item")
        if (other.CompareTag("Item"))
        {
            // Добавляем предмет в хранилище
            StoreItem(other.gameObject);
        }
    }

    private void StoreItem(GameObject item)
    {
        storedItems.Add(item);
        item.SetActive(false); // Делаем предмет невидимым или неактивным
        Debug.Log("Item stored!");
    }

    private void Update()
    {
        // Проверяем, нажата ли кнопка E
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Извлекаем предметы из хранилища
            RetrieveItems();
        }
    }

    private void RetrieveItems()
    {
        foreach (GameObject item in storedItems)
        {
            // Устанавливаем предметы в определенные точки выхода (например, в позицию хранилища)
            item.transform.position = exitPoint.position; // Или задайте конкретную позицию выхода
            item.SetActive(true); // Делаем предмет видимым или активным
        }
        storedItems.Clear();
        Debug.Log("Items retrieved!");
    }
}
