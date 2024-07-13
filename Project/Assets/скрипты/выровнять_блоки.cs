using UnityEngine;

public class GridAligner : MonoBehaviour
{
    public float gridSize = 1.0f;  // Размер клетки сетки
    public GameObject[] blocksToAlign;  // Массив блоков для выравнивания

    // Update вызывается один раз в кадр
    void Update()
    {
        foreach (GameObject block in blocksToAlign)
        {
            if (block != null)
            {
                AlignToGrid(block);
            }
        }
    }

    void AlignToGrid(GameObject block)
    {
        // Получаем текущее положение блока
        Vector3 blockPosition = block.transform.position;

        // Вычисляем ближайшую позицию на сетке
        float x = Mathf.Round(blockPosition.x / gridSize) * gridSize;
        float y = Mathf.Round(blockPosition.y / gridSize) * gridSize;
        float z = Mathf.Round(blockPosition.z / gridSize) * gridSize;

        Vector3 newPosition = new Vector3(x, y, z);

        // Если блок не на сетке, перемещаем его
        if (block.transform.position != newPosition)
        {
            block.transform.position = newPosition;
        }
    }
}
