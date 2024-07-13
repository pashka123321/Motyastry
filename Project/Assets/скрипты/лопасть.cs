using UnityEngine;

public class RotateAroundSelf : MonoBehaviour
{
    public float rotationSpeed = 50f; // скорость вращения

    void Update()
    {
        // Вращаем объект вокруг себя по оси Z
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}