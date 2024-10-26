using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour
{
    public float stepDuration = 0.2f; // Время одного шага ноги
    public Vector3 targetPositionOffset; // Смещение, к которому нога должна двигаться
    
    [HideInInspector] public bool isMoving = false;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    public void Move()
    {
        if (!isMoving)
        {
            StartCoroutine(Step());
        }
    }

    IEnumerator Step()
    {
        isMoving = true;

        Vector3 targetPosition = initialPosition + targetPositionOffset;
        float elapsedTime = 0f;

        // Движение ноги к целевой позиции
        while (elapsedTime < stepDuration)
        {
            transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / stepDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition;

        // Возвращаем ногу в исходное положение
        elapsedTime = 0f;
        while (elapsedTime < stepDuration)
        {
            transform.localPosition = Vector3.Lerp(targetPosition, initialPosition, elapsedTime / stepDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = initialPosition;

        isMoving = false;
    }
}
