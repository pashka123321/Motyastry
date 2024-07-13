using UnityEngine;

public class DrillPlacement : MonoBehaviour
{
    public GameObject drillPrefab; // Префаб бура
    public GameObject copperPrefab; // Префаб медной руды
    public LayerMask groundLayer; // Слой, на котором размещаются объекты

    private GameObject previewDrill; // Предпросмотр бура
    private bool isPreviewing = false; // Флаг для отслеживания предпросмотра

    void Update()
    {
        // Обработка нажатия кнопки для размещения бура
        if (Input.GetMouseButtonDown(0)) // ЛКМ для размещения
        {
            PlaceDrill();
        }

        // Обработка предпросмотра бура
        UpdatePreview();
    }

    void PlaceDrill()
    {
        if (!isPreviewing) return; // Если нет предпросмотра, выходим

        // Создаем новый экземпляр бура на месте предпросмотра
        GameObject newDrill = Instantiate(drillPrefab, previewDrill.transform.position, Quaternion.identity);

        // Проверяем под буром наличие медной руды и удаляем ее, если она есть
        Collider2D[] colliders = Physics2D.OverlapBoxAll(newDrill.transform.position, new Vector2(1f, 1f), 0f, groundLayer);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("CopperOre"))
            {
                Destroy(collider.gameObject);
            }
        }

        // Уничтожаем предпросмотр бура
        Destroy(previewDrill);

        // Сбрасываем флаг предпросмотра
        isPreviewing = false;
    }

    void UpdatePreview()
    {
        // Получаем позицию курсора в мировых координатах
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPos = new Vector3Int(Mathf.FloorToInt(mouseWorldPos.x), Mathf.FloorToInt(mouseWorldPos.y), 0);

        // Если нет предпросмотра, создаем его
        if (!isPreviewing)
        {
            previewDrill = Instantiate(drillPrefab, gridPos, Quaternion.identity);
            isPreviewing = true;
        }
        else
        {
            // Позиционируем предпросмотр бура по блочной четке
            previewDrill.transform.position = gridPos;
        }
    }
}
