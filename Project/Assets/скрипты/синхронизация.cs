using Mirror;
using UnityEngine;

public class PlayerMovementSync : NetworkBehaviour
{
    public float speed = 5f;
    
    // Переменная для хранения последней позиции игрока
    [SyncVar] private Vector2 syncedPosition;

    private bool isPositionInitialized = false;  // Флаг для отслеживания начальной синхронизации

    void Update()
    {
        // Управление объектом только для локального игрока
        if (isLocalPlayer)
        {
            HandleMovement();
        }
        else
        {
            // Интерполяция для плавного обновления позиции у других клиентов
            if (isPositionInitialized)  // Проверяем, была ли инициализирована начальная позиция
            {
                transform.position = Vector3.Lerp(transform.position, syncedPosition, Time.deltaTime * 10);
            }
        }
    }

    // Управление движением игрока
    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        Vector2 movement = new Vector2(moveX, moveY);
        transform.Translate(movement);

        // Отправляем команду на сервер для обновления позиции
        CmdSendPositionToServer(transform.position);
    }

    // Команда для отправки позиции на сервер
    [Command]
    void CmdSendPositionToServer(Vector2 position)
    {
        // Обновляем позицию на сервере
        syncedPosition = position;

        // Вызываем синхронизацию для всех клиентов
        RpcSyncPosition(position);
    }

    // Синхронизация позиции для всех клиентов
    [ClientRpc]
    void RpcSyncPosition(Vector2 position)
    {
        // Обновляем позицию на клиенте
        syncedPosition = position;

        // Устанавливаем флаг после получения начальной позиции
        isPositionInitialized = true;
    }
}
