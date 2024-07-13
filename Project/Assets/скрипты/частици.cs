using UnityEngine;
using System.Collections;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem particleSystem; // Переменная для системы частиц
    public int particleCount = 1000; // Количество частиц, которое нужно выпустить
    public float delayBeforeStart = 1.6f; // Задержка перед запуском частиц
    public float particleDuration = 1f; // Продолжительность выпуска частиц

    private void Start()
    {
        // Убедимся, что система частиц изначально не запущена
        particleSystem.Stop();

        // Запуск корутины для управления частицами
        StartCoroutine(ManageParticles());
    }

    private IEnumerator ManageParticles()
    {
        // Ожидание заданного времени перед запуском частиц
        yield return new WaitForSeconds(delayBeforeStart);

        // Выпуск заданного количества частиц
        particleSystem.Emit(particleCount);

        // Запуск системы частиц (чтобы частицы отображались)
        particleSystem.Play();

        // Ожидание заданного времени перед остановкой частиц
        yield return new WaitForSeconds(particleDuration);

        // Остановка системы частиц
        particleSystem.Stop();
    }
}
