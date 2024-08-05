using System.Collections;
using UnityEngine;

public class ChangeSortingOrderOverTime : MonoBehaviour
{
    public float changeDuration = 2f;  // Длительность изменения порядка сортировки
    public int startSortingOrder = 40; // Начальный порядок сортировки
    public int targetSortingOrder = 3; // Конечный порядок сортировки

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        // Получаем компонент SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on the object.");
            return;
        }

        // Устанавливаем начальный порядок сортировки
        spriteRenderer.sortingOrder = startSortingOrder;
        
        // Начинаем изменение порядка сортировки
        StartCoroutine(ChangeSortingOrder());
    }

    IEnumerator ChangeSortingOrder()
    {
        float elapsedTime = 0f;

        while (elapsedTime < changeDuration)
        {
            elapsedTime += Time.deltaTime;
            int currentSortingOrder = Mathf.RoundToInt(Mathf.Lerp(startSortingOrder, targetSortingOrder, elapsedTime / changeDuration));
            spriteRenderer.sortingOrder = currentSortingOrder;
            yield return null;
        }

        // Убедимся, что объект на конечном порядке сортировки
        spriteRenderer.sortingOrder = targetSortingOrder;
    }
}
