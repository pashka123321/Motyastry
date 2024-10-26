using UnityEngine;

public class RotateAroundSelf : MonoBehaviour
{
    public float rotationSpeed = 50f; // скорость вращения

    void Start()
    {
        // Устанавливаем случайный начальный угол вращения
        float initialRotation = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0, 0, initialRotation);
    }

    void Update()
    {
        // Вращаем объект вокруг себя по оси Z
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
