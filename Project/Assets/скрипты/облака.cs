using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutObjects : MonoBehaviour
{
    public GameObject[] objectsToFade;          // Массив объектов, которые будут постепенно исчезать
    public GameObject[] fixedObjectPrefabs;     // Массив префабов объектов, которые будут появляться на фиксированных координатах
    public Vector2[] fixedPositions;            // Массив фиксированных координат для объектов
    public float fadeDuration = 2f;             // Длительность затухания для случайных объектов
    public float fixedObjectFadeDuration = 1f; // Длительность затухания для фиксированных объектов
    public LayerMask oblakoLayer;               // Слой для объектов с тегом "oblako"
    public float minDistance = 1f;              // Минимальное расстояние между объектами

    void Start()
    {
        // Создаем объекты на фиксированных координатах
        CreateFixedObjects();

        // Генерируем случайные позиции для оставшихся объектов
        GenerateEvenlyDistributedPositions();

        // Начинаем затухание для всех объектов в массиве
        foreach (GameObject obj in objectsToFade)
        {
            // Определяем длительность затухания в зависимости от типа объекта
            float duration = obj.GetComponent<FixedObject>() ? fixedObjectFadeDuration : fadeDuration;
            StartCoroutine(FadeOut(obj, duration));
        }
    }

    void CreateFixedObjects()
    {
        if (fixedObjectPrefabs.Length != fixedPositions.Length)
        {
            Debug.LogError("Количество префабов и фиксированных позиций должно совпадать.");
            return;
        }

        for (int i = 0; i < fixedObjectPrefabs.Length; i++)
        {
            GameObject fixedObject = Instantiate(fixedObjectPrefabs[i], fixedPositions[i], Quaternion.identity);
            // Установим специальный компонент для фиксированных объектов (опционально)
            fixedObject.AddComponent<FixedObject>();
            // Добавьте в массив объектов для затухания, если это необходимо
            List<GameObject> objectsList = new List<GameObject>(objectsToFade);
            objectsList.Add(fixedObject);
            objectsToFade = objectsList.ToArray();
        }
    }

    void GenerateEvenlyDistributedPositions()
    {
        foreach (GameObject obj in objectsToFade)
        {
            if (obj.tag != "oblako") continue;

            bool validPosition = false;
            Collider2D objCollider = obj.GetComponent<Collider2D>();
            Bounds objBounds = objCollider.bounds;
            Vector2 newPosition = Vector2.zero;

            // Пытаемся найти корректную позицию для текущего объекта
            while (!validPosition)
            {
                // Генерируем новую случайную позицию
                newPosition = new Vector2(Random.Range(0f, 200f), Random.Range(0f, 200f));

                // Проверка коллизий с объектами на слое "oblako"
                Collider2D[] colliders = Physics2D.OverlapBoxAll(newPosition, objBounds.size, 0f, oblakoLayer);
                validPosition = colliders.Length == 0;

                // Дополнительная проверка расстояний между объектами
                if (validPosition)
                {
                    foreach (GameObject otherObj in objectsToFade)
                    {
                        if (otherObj != obj && otherObj.tag == "oblako")
                        {
                            Bounds otherBounds = otherObj.GetComponent<Collider2D>().bounds;
                            float distance = Vector2.Distance(newPosition, otherObj.transform.position);
                            if (distance < minDistance)
                            {
                                validPosition = false;
                                break;
                            }
                        }
                    }
                }
            }

            obj.transform.position = newPosition;
        }
    }

IEnumerator FadeOut(GameObject obj, float duration)
{
    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
    if (sr == null)
    {
        yield break;
    }

    Color originalColor = sr.color;
    float fadeSpeed = 1f / duration;
    float progress = 0f;

    while (progress < 1f)
    {
        Color color = sr.color;
        color.a = Mathf.Lerp(originalColor.a, 0f, progress);
        sr.color = color;

        progress += fadeSpeed * Time.deltaTime;
        yield return null;
    }

    // Убедимся, что объект полностью исчез
    Color finalColor = sr.color;
    finalColor.a = 0f;
    sr.color = finalColor;

    // Удаляем объект из сцены после затухания
    Destroy(obj);
}

}

// Дополнительный компонент для фиксированных объектов
public class FixedObject : MonoBehaviour
{
    // Можно добавить специфические данные для фиксированных объектов, если необходимо
}
