using UnityEngine;

public class CameraFollowRTS : MonoBehaviour
{
    public Transform target; // Текущий объект, за которым следит камера
    public float smoothing = 5f; // Скорость сглаживания движения камеры
    public float zoomSpeed = 2f; // Скорость изменения масштаба
    public float minZoom = 2f; // Минимальный масштаб
    public float maxZoom = 10f; // Максимальный масштаб
    private float targetZoom; // Целевой масштаб камеры

    public Transform playerTransform; // Объект игрока (стандартный таргет)
    public GameObject corePrefab; // Префаб ядра, к которому телепортируется игрок
    public GameObject hoverIndicatorPrefab; // Префаб индикатора для подсветки объектов

    private bool isFollowingPlayer = true; // Флаг, указывающий, следует ли камера за игроком
    private bool isCameraStopped = false; // Флаг для остановки камеры

    private Transform hoveredTarget; // Объект, на который наведен курсор
    private GameObject hoverIndicator; // Текущий индикатор наведения
    private bool isTargetChanged = false; // Флаг, указывающий, был ли сменен таргет

    private UnitLogic controlledUnit; // Ссылка на управляемый юнит

    void Start()
    {
        targetZoom = 8f;
        Camera.main.orthographicSize = targetZoom;
        if (playerTransform != null)
        {
            SetTarget(playerTransform);
        }
    }

void LateUpdate()
{
    if (isCameraStopped) return;

    // Следование за текущим таргетом
    if (isFollowingPlayer && target != null)
    {
        Vector3 targetCamPos = target.position;
        targetCamPos.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }

    // Обработка изменения масштаба
    float scrollData = Input.GetAxis("Mouse ScrollWheel");
    targetZoom -= scrollData * zoomSpeed;
    targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, smoothing * Time.deltaTime);

    // Логика смены таргета при зажатом V
    if (Input.GetKey(KeyCode.V))
    {
        HighlightHoveredTarget();
    }
    else
    {
        ResetHoveredTarget();
    }

    // Смена таргета камеры при нажатии ЛКМ
    if (Input.GetKey(KeyCode.V) && Input.GetMouseButtonDown(0) && hoveredTarget != null && hoveredTarget.GetComponent<TargetableObject>() != null)
    {
        // Телепортируем игрока перед его отключением
        if (corePrefab != null && playerTransform != null)
        {
            playerTransform.position = corePrefab.transform.position;
        }

        SetTarget(hoveredTarget);
        isTargetChanged = true; // Устанавливаем флаг смены таргета
        playerTransform.gameObject.SetActive(false); // Отключаем игрока

        // Проверяем, является ли новый таргет юнитом
        controlledUnit = hoveredTarget.GetComponent<UnitLogic>();
        if (controlledUnit != null)
        {
            controlledUnit.isManualMode = true; // Активируем ручной режим
        }
    }

    // Возврат к стандартному таргету при нажатии V, если мы вселились в другой объект
    if (Input.GetKeyDown(KeyCode.V) && isTargetChanged)
    {
        ResetToPlayer();
        isTargetChanged = false; // Сбрасываем флаг после возврата
        playerTransform.gameObject.SetActive(true); // Включаем игрока
        if (controlledUnit != null)
        {
            controlledUnit.isManualMode = false; // Деактивируем ручной режим
            controlledUnit = null; // Сбрасываем управляемый юнит
        }
    }

    // Логика ручного управления юнитом
    if (controlledUnit != null && controlledUnit.isManualMode)
    {
        controlledUnit.ManualControl();
    }
}


    // Функция для установки нового объекта для отслеживания
public void SetTarget(Transform newTarget)
{
    target = newTarget;
    isFollowingPlayer = true;

    // Если цель турель, активируем ручной режим
    TurretController turretController = newTarget.GetComponent<TurretController>();
    if (turretController != null)
    {
        turretController.isManualMode = true;
    }
}
// Сброс ручного режима при возврате к игроку
public void ResetToPlayer()
{
    if (target != null)
    {
        TurretController turretController = target.GetComponent<TurretController>();
        if (turretController != null)
        {
            turretController.isManualMode = false;
        }
    }

    SetTarget(playerTransform);
}
// Подсветка объекта, на который наведен курсор
private void HighlightHoveredTarget()
{
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

    if (hit.collider != null)
    {
        Transform hitTransform = hit.collider.transform;

        // Проверяем, есть ли на объекте скрипт TargetableObject
        if (hitTransform != hoveredTarget && hitTransform.GetComponent<TargetableObject>() != null)
        {
            ResetHoveredTarget(); // Сбрасываем предыдущий индикатор
            hoveredTarget = hitTransform;

            if (hoverIndicatorPrefab != null)
            {
                hoverIndicator = Instantiate(hoverIndicatorPrefab, hoveredTarget.position, Quaternion.identity);
                hoverIndicator.transform.SetParent(hoveredTarget); // Устанавливаем индикатор как дочерний объект

                // Поворот индикатора на 90 градусов
                hoverIndicator.transform.rotation = Quaternion.Euler(0, 0, 45);
            }
        }
    }
    else
    {
        ResetHoveredTarget(); // Сбрасываем индикатор, если ничего не выделено
    }
}


    // Сброс индикатора с предыдущего объекта
    private void ResetHoveredTarget()
    {
        if (hoverIndicator != null)
        {
            Destroy(hoverIndicator); // Удаляем индикатор
        }
        hoveredTarget = null;
    }

    
}
