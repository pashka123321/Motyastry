using UnityEngine;
using Pathfinding; // Убедитесь, что этот namespace подключен

public class BlockHealth : MonoBehaviour
{
    [System.Serializable]
    public class BlockType
    {
        public GameObject prefab; // Префаб блока
        public float initialHealth; // Начальное здоровье блока
    }

    public BlockType[] blockTypes; // Массив блоков с начальным здоровьем
    public AstarPath astarPath; // Ссылка на компонент AstarPath для обновления сетки

    private float currentHealth;

    private BuildModeController buildModeController;

    private void Start()
    {
        BlockType blockType = GetBlockType(gameObject);
        if (blockType != null)
        {
            currentHealth = blockType.initialHealth; // Устанавливаем текущее здоровье равным начальному
        }
        else
        {
            Debug.LogWarning("BlockType not found for: " + gameObject.name);
            currentHealth = 0; // Устанавливаем здоровье в 0, если тип не найден
        }

        buildModeController = GameObject.Find("стройка").GetComponent<BuildModeController>();
    }

    private BlockType GetBlockType(GameObject block)
    {
        foreach (var blockType in blockTypes)
        {
            if (blockType.prefab == block)
            {
                return blockType;
            }
        }
        return null;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage; // Уменьшаем текущее здоровье на урон
        if (currentHealth <= 0)
        {
            // Получаем коллайдер блока для обновления графа
            Collider2D blockCollider = GetComponent<Collider2D>();

            if (blockCollider != null)
            {
                // Обновляем граф перед удалением блока
                Bounds bounds = blockCollider.bounds;
                GraphUpdateObject guo = new GraphUpdateObject(bounds);
                AstarPath.active.UpdateGraphs(guo);
            }

            buildModeController.DestroyByEnemy(transform.position);
        }
    }
}
