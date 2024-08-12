using UnityEngine;

public class RotateAroundSelf : MonoBehaviour
{
    public float rotationSpeed = 50f; // скорость вращения
    public float initialRotation = 0f; // начальный угол вращения

    void Start()
    {
        // Устанавливаем начальный угол
        transform.rotation = Quaternion.Euler(0, 0, initialRotation);
    }

    void Update()
    {
        // Вращаем объект вокруг себя по оси Z
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
