using UnityEngine;

public class ShowObjectsAfterDelay : MonoBehaviour
{
    [System.Serializable]
    public class ObjectDelay
    {
        public GameObject obj; // Ссылка на объект, который нужно показать
        public float delay; // Время задержки перед показом объекта
    }

    public ObjectDelay[] objectsWithDelay; // Массив объектов с настройками задержки
    private bool[] objectShown; // Массив для отслеживания, был ли объект уже показан

    void Start()
    {
        objectShown = new bool[objectsWithDelay.Length]; // Инициализируем массив для отслеживания видимости объектов

        // Запускаем показ объектов с задержкой
        for (int i = 0; i < objectsWithDelay.Length; i++)
        {
            Invoke("ShowDelayedObject", objectsWithDelay[i].delay);
        }
    }

    void ShowDelayedObject()
    {
        for (int i = 0; i < objectsWithDelay.Length; i++)
        {
            if (!objectShown[i]) // Проверяем, не показан ли уже объект
            {
                objectsWithDelay[i].obj.SetActive(true); // Делаем объект видимым
                objectShown[i] = true; // Помечаем объект как показанный
                return; // Выходим из метода после показа первого непоказанного объекта
            }
        }
    }
}
