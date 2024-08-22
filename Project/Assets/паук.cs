using UnityEngine;
using System.Collections;

public class SpiderLegs2D : MonoBehaviour
{
    public Transform[] legs; // Ссылки на ноги (столбы)
    public Transform[] joints; // Ссылки на суставы между ногами и телом
    public Transform target; // Цель, к которой движется паук
    public ParticleSystem dustParticlePrefab; // Префаб для пыли
    public AudioClip stepSound; // Звук шага
    public AudioSource audioSource; // Аудиоисточник для воспроизведения звуков

    public float stepDistance = 2f; // Дистанция, на которую ноги могут переставляться
    public float stepSpeed = 3f; // Скорость перемещения ноги
    public float legMoveThreshold = 1.5f; // Максимальное расстояние от тела, на котором нога должна сделать шаг
    public float rotationSpeed = 5f; // Скорость поворота главного объекта к цели
    public float maxLegDistance = 3f; // Максимальная дистанция между ногой и телом
    public float bodyAdjustmentSpeed = 2f; // Скорость корректировки положения тела

    public Vector2[] legOffsets;
    public Vector2[] jointOffsets;

    private bool[] legIsMoving;

    void Start()
    {
        if (legOffsets == null || legOffsets.Length != legs.Length)
        {
            Debug.LogError("Необходимо задать смещения для каждой ноги.");
            return;
        }

        if (joints == null || joints.Length != legs.Length)
        {
            Debug.LogError("Необходимо задать суставы для каждой ноги.");
            return;
        }

        if (jointOffsets == null || jointOffsets.Length != joints.Length)
        {
            Debug.LogError("Необходимо задать смещения для каждого сустава.");
            return;
        }

        legIsMoving = new bool[legs.Length];

        // Запуск корутин для каждой ноги в строгом порядке
        StartCoroutine(MoveLegsInOrder());
    }

    IEnumerator MoveLegsInOrder()
    {
        int[] legOrder = new int[] { 0, 3, 1, 2 }; // Порядок перемещения ног

        while (true)
        {
            foreach (int i in legOrder)
            {
                Vector2 rotatedOffset = RotateVector(legOffsets[i], transform.rotation.eulerAngles.z);
                Vector3 bodyToLeg = legs[i].position - (transform.position + (Vector3)rotatedOffset);

                if (bodyToLeg.magnitude > legMoveThreshold && !legIsMoving[i])
                {
                    legIsMoving[i] = true;

                    Vector3 directionToTarget = (target.position - transform.position).normalized;
                    Vector3 legTargetPosition = (Vector2)transform.position + (Vector2)directionToTarget * stepDistance + rotatedOffset;

                    Vector3 initialLegPosition = legs[i].position;

                    float moveDuration = Vector2.Distance(initialLegPosition, legTargetPosition) / stepSpeed;
                    float elapsedTime = 0f;

                    while (elapsedTime < moveDuration)
                    {
                        legs[i].position = Vector3.Lerp(initialLegPosition, legTargetPosition, elapsedTime / moveDuration);
                        UpdateJointPosition(i);
                        RotateLegTowardsTarget(i, legTargetPosition);
                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }

                    legs[i].position = legTargetPosition;
                    UpdateJointPosition(i);

                    // Спавн пыли под ногой
                    SpawnDustEffect(legs[i].position);

                    // Воспроизведение звука шага
                    PlayStepSound();

                    legIsMoving[i] = false;

                    // Задержка перед следующим шагом
                    yield return new WaitForSeconds(0.2f); // Уменьшена задержка для плавности
                }
            }

            yield return new WaitForSeconds(0.1f); // Небольшая задержка между проходами по порядку ног
        }
    }

    void Update()
    {
        Vector2 directionToTarget = (target.position - transform.position).normalized;
        transform.position += (Vector3)(directionToTarget * stepSpeed * Time.deltaTime);
        RotateTowardsTarget2D();

        Vector3 centerPosition = Vector3.zero;
        bool adjustBodyPosition = false;

        foreach (Transform leg in legs)
        {
            centerPosition += leg.position;

            // Проверка на максимальное расстояние между телом и ногами
            if (Vector2.Distance(transform.position, leg.position) > maxLegDistance)
            {
                adjustBodyPosition = true;
            }
        }

        centerPosition /= legs.Length;

        // Если тело слишком далеко от ног, скорректируем его позицию
        if (adjustBodyPosition)
        {
            transform.position = Vector2.Lerp(transform.position, centerPosition, Time.deltaTime * bodyAdjustmentSpeed);
        }
        else
        {
            transform.position = Vector2.Lerp(transform.position, centerPosition, Time.deltaTime * stepSpeed * 0.5f); // Плавное смещение тела
        }

        for (int i = 0; i < legs.Length; i++)
        {
            UpdateJointPosition(i);
        }
    }

    private void RotateTowardsTarget2D()
    {
        Vector2 directionToTarget = target.position - transform.position;
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        float currentAngle = Mathf.LerpAngle(transform.rotation.eulerAngles.z, angle, Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
    }

    private Vector2 RotateVector(Vector2 v, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
    }

    private void UpdateJointPosition(int i)
    {
        Vector3 legPosition = legs[i].position;
        Vector3 bodyPosition = transform.position;

        Vector2 rotatedJointOffset = RotateVector(jointOffsets[i], transform.rotation.eulerAngles.z);
        joints[i].position = (legPosition + bodyPosition) / 2f + (Vector3)rotatedJointOffset;

        Vector2 direction = legPosition - joints[i].position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        joints[i].rotation = Quaternion.Euler(0, 0, angle);
    }

    private void RotateLegTowardsTarget(int legIndex, Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - legs[legIndex].position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        legs[legIndex].rotation = Quaternion.Euler(0, 0, angle);
    }

    // Метод для спавна эффекта пыли
    private void SpawnDustEffect(Vector3 position)
    {
        if (dustParticlePrefab != null)
        {
            ParticleSystem dust = Instantiate(dustParticlePrefab, position, Quaternion.identity);
            dust.Play();
            Destroy(dust.gameObject, 2f); // Уничтожаем объект после того, как эффект отыграет
        }
    }

    // Метод для воспроизведения звука шага
    private void PlayStepSound()
    {
        if (audioSource != null && stepSound != null)
        {
            audioSource.PlayOneShot(stepSound);
        }
    }
}
