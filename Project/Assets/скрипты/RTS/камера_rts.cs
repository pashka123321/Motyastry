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

    // Проверяем объект под курсором в любом случае
    HighlightHoveredTarget();

    // Смена таргета камеры при нажатии ЛКМ
    if (Input.GetKey(KeyCode.V) && Input.GetMouseButtonDown(0) && hoveredTarget != null && hoveredTarget.GetComponent<TargetableObject>() != null)
    {
        if (corePrefab != null && playerTransform != null)
        {
            playerTransform.position = corePrefab.transform.position;
        }

        SetTarget(hoveredTarget);
        isTargetChanged = true;
        playerTransform.gameObject.SetActive(false);

        controlledUnit = hoveredTarget.GetComponent<UnitLogic>();
        if (controlledUnit != null)
        {
            controlledUnit.isManualMode = true;
        }
    }

    // Возврат к игроку при нажатии V
    if (Input.GetKeyDown(KeyCode.V) && isTargetChanged)
    {
        ResetToPlayer();
        isTargetChanged = false;
        playerTransform.gameObject.SetActive(true);
        if (controlledUnit != null)
        {
            controlledUnit.isManualMode = false;
            controlledUnit = null;
        }
    }

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

        // Проверяем, изменился ли объект под курсором
        if (hitTransform != hoveredTarget)
        {
            ResetHoveredTarget(); 
            hoveredTarget = hitTransform;
        }

        // Если V зажата, включаем индикатор
        if (Input.GetKey(KeyCode.V) && hoveredTarget.GetComponent<TargetableObject>() != null)
        {
            if (hoverIndicator == null && hoverIndicatorPrefab != null)
            {
                hoverIndicator = Instantiate(hoverIndicatorPrefab, hoveredTarget.position, Quaternion.identity);
                hoverIndicator.transform.SetParent(hoveredTarget);
                hoverIndicator.transform.rotation = Quaternion.Euler(0, 0, 45);
            }
        }
        else
        {
            ResetHoveredTarget();
        }
    }
    else
    {
        ResetHoveredTarget();
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
