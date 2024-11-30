using UnityEngine;

public class RandomEnable : MonoBehaviour
{
    [SerializeField] private GameObject targetPrefab;

    private void Start()
    {
        if (targetPrefab != null)
        {
            // Проверяем шанс 50%
            bool activate = Random.value > 0.5f;
            targetPrefab.SetActive(activate);
        }
        else
        {
            Debug.LogWarning("Не назначен префаб для активации/деактивации.");
        }
    }
}
