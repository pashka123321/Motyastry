using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public float maxMovementSpeed = 10f; // Максимальная скорость
    public float acceleration = 15f; // Коэффициент ускорения
    public float deceleration = 10f; // Коэффициент замедления
    public float directionChangeSpeed = 10f; // Скорость изменения направления
    public float rotationSpeed = 10f;

    private Vector2 currentDirection = Vector2.zero; // Текущее направление
    private float currentSpeed = 0.0f; // Текущая скорость

    public bool canControl = true; // Флаг для контроля управления

    private bool isRotating = false; // Флаг для отслеживания, идет ли поворот
    private bool wasRotatingInitially = false; // Флаг для отслеживания, начался ли поворот до наведения на UI

    void Update()
    {
        if (!canControl) return; // Если управление отключено, выходим из метода

        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.W))
        {
            verticalInput = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            verticalInput = -1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1f;
        }

        Vector2 inputDirection = new Vector2(horizontalInput, verticalInput).normalized;

        if (inputDirection.magnitude > 0)
        {
            currentDirection = Vector2.Lerp(currentDirection, inputDirection, directionChangeSpeed * Time.deltaTime);
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, maxMovementSpeed);
        }
        else
        {
            currentSpeed -= deceleration * Time.deltaTime;
            currentSpeed = Mathf.Max(currentSpeed, 0);
        }

        Vector2 movement = currentDirection * currentSpeed;
        transform.Translate(movement * Time.deltaTime, Space.World);

        bool isLMBPressed = Input.GetMouseButton(0);

        if (isLMBPressed && !EventSystem.current.IsPointerOverGameObject())
        {
            wasRotatingInitially = true;
            isRotating = true;
        }

        if (!isLMBPressed)
        {
            isRotating = false;
            wasRotatingInitially = false;
        }

        if ((isRotating && wasRotatingInitially) || (!EventSystem.current.IsPointerOverGameObject() && isLMBPressed))
        {
            RotateTowardsMouse();
        }
        else if (movement.magnitude > 0)
        {
            RotateTowardsMovement(movement);
        }
        else
        {
            RotateTowardsMouse();
        }
    }

    void RotateTowardsMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void RotateTowardsMovement(Vector2 movement)
    {
        float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
