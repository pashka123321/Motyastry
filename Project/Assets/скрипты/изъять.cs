using System.Collections.Generic;
using UnityEngine;

namespace MyNamespace
{
    public class Storage : MonoBehaviour
    {
        public Transform exitPoint; // Точка выхода

        private List<GameObject> storedItems = new List<GameObject>();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Item"))
            {
                StoreItem(other.gameObject);
            }
        }

        private void StoreItem(GameObject item)
        {
            storedItems.Add(item);
            item.SetActive(false);
            Debug.Log("Item stored!");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                RetrieveItems();
            }
        }

        private void RetrieveItems()
        {
            foreach (GameObject item in storedItems)
            {
                item.transform.position = exitPoint.position; // Устанавливаем позицию точки выхода
                item.SetActive(true);
            }
            storedItems.Clear();
            Debug.Log("Items retrieved!");
        }
    }
}
