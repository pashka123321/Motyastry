using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Для использования List<>
using UnityEngine.EventSystems;
using Mirror;

public class PlayerMovementm : NetworkBehaviour
{
    public float maxMovementSpeed = 10f;
    public float acceleration = 15f;
    public float deceleration = 10f;
    public float directionChangeSpeed = 10f;
    public float rotationSpeed = 10f;
    public float rotationDelay = 0.5f;

    private Vector2 currentDirection = Vector2.zero;
    private float currentSpeed = 0.0f;

    public bool canControl = true;

    private bool isRotating = false;
    private bool wasRotatingInitially = false;
    private float rotationStartTime = 0f;

    public List<GameObject> ignoreUIElements = new List<GameObject>();

    [SyncVar]
    private Vector3 syncPosition;

    [SyncVar]
    private Quaternion syncRotation;

    void Update()
    {
        if (!isLocalPlayer)
        {
            // Синхронизировать позицию и ротацию для не локальных игроков
            transform.position = Vector3.Lerp(transform.position, syncPosition, Time.deltaTime * 15);
            transform.rotation = Quaternion.Lerp(transform.rotation, syncRotation, Time.deltaTime * 15);
            return;
        }

        if (!canControl) return;

        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (Input.GetKey(KeyCode.W)) verticalInput = 1f;
        if (Input.GetKey(KeyCode.S)) verticalInput = -1f;
        if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;
        if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;

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

        if (isLMBPressed && !IsPointerOverUI())
        {
            if (!wasRotatingInitially)
            {
                rotationStartTime = Time.time;
                wasRotatingInitially = true;
            }

            if (Time.time - rotationStartTime >= rotationDelay)
            {
                isRotating = true;
            }
        }
        else
        {
            isRotating = false;
            wasRotatingInitially = false;
        }

        if (isRotating)
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

        // Отправить данные на сервер
        CmdMove(transform.position, transform.rotation);
    }

    [Command]
    void CmdMove(Vector3 position, Quaternion rotation)
    {
        syncPosition = position;
        syncRotation = rotation;

        // Обновить данные на всех клиентах
        RpcMove(position, rotation);
    }

    [ClientRpc]
    void RpcMove(Vector3 position, Quaternion rotation)
    {
        if (isLocalPlayer) return;

        syncPosition = position;
        syncRotation = rotation;
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

    bool IsPointerOverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (!ignoreUIElements.Contains(result.gameObject))
            {
                return true;
            }
        }

        return false;
    }
}
