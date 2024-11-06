using UnityEngine;

public class CameraIgnorePrefabs : MonoBehaviour
{
    // Массив префабов, которые камера должна игнорировать
    public GameObject[] ignoredPrefabs;

    private void Start()
    {
        // Проходим по каждому объекту из массива игнорируемых
        foreach (GameObject prefab in ignoredPrefabs)
        {
            // Проверяем, существует ли объект в сцене
            GameObject[] objectsInScene = GameObject.FindGameObjectsWithTag(prefab.tag);

            // Устанавливаем для каждого объекта его рендер в неактивное состояние
            foreach (GameObject obj in objectsInScene)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
                }
            }
        }
    }
}
