using UnityEngine;
using System.Collections;
public class ParticleSpawner : MonoBehaviour
{
    public GameObject particlePrefab; // Префаб системы частиц, который будет спауниться
    public float spawnDelay = 1.6f; // Задержка перед спауном

    void Start()
    {
        // Запускаем корутину, которая через заданное время создаст частицу
        StartCoroutine(SpawnParticleAfterDelay());
    }

    private IEnumerator SpawnParticleAfterDelay()
    {
        // Ждём указанное количество времени
        yield return new WaitForSeconds(spawnDelay);

        // Создаём систему частиц на месте, где привязан этот скрипт
        Instantiate(particlePrefab, transform.position, Quaternion.identity);
    }
}
