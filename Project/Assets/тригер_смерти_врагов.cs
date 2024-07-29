using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    void OnDestroy()
    {
        // Проверяем, чтобы убедиться, что EnemySpawner существует
        if (EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.OnEnemyDestroyed();
        }
    }
}
