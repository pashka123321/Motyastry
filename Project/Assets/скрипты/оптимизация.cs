using System.Collections.Generic;
using UnityEngine;

public class VisibilityController : MonoBehaviour
{
    public Camera mainCamera;
    public List<GameObject> ignoreList = new List<GameObject>();
    public float checkInterval = 0.5f; // Интервал проверки в секундах

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Запускаем корутину для периодической проверки видимости объектов
        StartCoroutine(CheckVisibility());
    }

    private IEnumerator<WaitForSeconds> CheckVisibility()
    {
        while (true)
        {
            foreach (GameObject obj in ignoreList)
            {
                var renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = true;
                }
            }

            // Получаем все объекты на сцене
            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (ignoreList.Contains(obj))
                {
                    continue; // Игнорируем объекты из списка исключений
                }

                var renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    if (IsVisibleFrom(renderer, mainCamera))
                    {
                        renderer.enabled = true;
                    }
                    else
                    {
                        renderer.enabled = false;
                    }
                }
            }

            // Ждем перед следующей проверкой
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
