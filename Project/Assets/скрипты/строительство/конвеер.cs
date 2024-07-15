using UnityEngine;
using UnityEngine.UI;

public class conveerBuildModeController : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject previewBlockPrefab; // Префаб блока предварительного просмотра
    public int previewBlockLayer = 10; // Слой для блока предварительного просмотра
    public Button buildModeButton;
    public AudioClip[] buildSounds; // Массив для хранения звуков строительства
    public AudioClip[] breakSounds; // Массив для хранения звуков разрушения
    private AudioSource audioSource;
    private GameObject previewBlock;
    private bool isBuildModeActive = false;
    private bool[,] grid; // Булева сетка для отслеживания занятых ячеек
    private int gridWidth = 200;
    private int gridHeight = 200;
    private float currentRotation = 0f; // Текущая ориентация блока

    public bool IsBuildModeActive => isBuildModeActive; // Свойство для проверки, активен ли режим строительства

    void Start()
    {
        grid = new bool[gridWidth, gridHeight];
        buildModeButton.onClick.AddListener(ToggleBuildMode);

        // Получаем компонент AudioSource с текущего объекта
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // Если компонент AudioSource не найден, выводим сообщение об ошибке
            Debug.LogError("AudioSource component not found on this GameObject.");
        }
    }

    void ToggleBuildMode()
    {
        isBuildModeActive = !isBuildModeActive;
        if (previewBlock != null)
        {
            Destroy(previewBlock);
        }
        currentRotation = 0f; // Сброс вращения при выходе из режима строительства
    }

    void Update()
    {
        if (isBuildModeActive)
        {
            if (previewBlock == null)
            {
                previewBlock = Instantiate(previewBlockPrefab);
                SetBlockTransparency(previewBlock, 0.5f); // Сделать блок предварительного просмотра прозрачным на 50%
                SetBlockLayer(previewBlock, previewBlockLayer); // Установить слой для блока предварительного просмотра
            }
            UpdatePreviewBlockPosition();

            if (Input.GetMouseButtonDown(0)) // Левая кнопка мыши
            {
                PlaceBlock();
            }

            if (Input.GetKeyDown(KeyCode.R)) // Клавиша R
            {
                RotateBlock();
            }
        }
        else
        {
            if (previewBlock != null)
            {
                Destroy(previewBlock);
            }
        }

        if (Input.GetMouseButtonDown(1)) // Правая кнопка мыши
        {
            if (isBuildModeActive)
            {
                isBuildModeActive = false;
                if (previewBlock != null)
                {
                    Destroy(previewBlock);
                }
                currentRotation = 0f; // Сброс вращения при выходе из режима строительства
            }
            else
            {
                RemoveBlock();
            }
        }
    }

    void UpdatePreviewBlockPosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 gridPosition = SnapToGrid(mousePosition);
        previewBlock.transform.position = new Vector3(gridPosition.x, gridPosition.y, 0f);
        previewBlock.transform.rotation = Quaternion.Euler(0f, 0f, currentRotation); // Устанавливаем вращение

        int x = Mathf.RoundToInt(gridPosition.x);
        int y = Mathf.RoundToInt(gridPosition.y);

        if (CanPlaceBlock(x, y))
        {
            SetBlockColor(previewBlock, Color.white); // Можно разместить блок, подсветка белая
        }
        else
        {
            SetBlockColor(previewBlock, Color.red); // Нельзя разместить блок, подсветка красная
        }

        SetBlockTransparency(previewBlock, 0.5f); // Обновляем прозрачность при перемещении блока
        SetBlockLayer(previewBlock, previewBlockLayer); // Обновляем слой при перемещении блока
    }

    void PlaceBlock()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 gridPosition = SnapToGrid(mousePosition);
        int x = Mathf.RoundToInt(gridPosition.x);
        int y = Mathf.RoundToInt(gridPosition.y);

        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            Debug.Log("Position is out of bounds");
            return;
        }

        if (!CanPlaceBlock(x, y))
        {
            Debug.Log("Cannot place block: Cell is already occupied by another block");
            return;
        }

        // Play build sound
        PlayBuildSound();

        GameObject newBlock = Instantiate(blockPrefab, new Vector3(x, y, 0f), Quaternion.Euler(0f, 0f, currentRotation));
        grid[x, y] = true;
    }

    void RemoveBlock()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 gridPosition = SnapToGrid(mousePosition);
        int x = Mathf.RoundToInt(gridPosition.x);
        int y = Mathf.RoundToInt(gridPosition.y);

        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            Debug.Log("Position is out of bounds");
            return;
        }

        if (IsCellOccupied(x, y))
        {
            // Play break sound
            PlayBreakSound();

            Collider2D[] colliders = Physics2D.OverlapPointAll(new Vector2(gridPosition.x, gridPosition.y));
            foreach (Collider2D col in colliders)
            {
                if (col.gameObject.CompareTag("Block"))
                {
                    Destroy(col.gameObject);
                    grid[x, y] = false;
                    break;
                }
                else if (col.gameObject.CompareTag("Conveer"))
                {
                    Destroy(col.gameObject);
                    grid[x, y] = false;
                    break;
                }
            }
        }
    }

    bool CanPlaceBlock(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            return false; // Вне границ сетки
        }

        Collider2D[] colliders = Physics2D.OverlapPointAll(new Vector2(x, y));
        foreach (Collider2D col in colliders)
        {
            if (col.gameObject != previewBlock && col.gameObject.CompareTag("Block"))
            {
                return false; // В этой клетке уже есть другой блок
            }
            else if (col.gameObject != previewBlock && col.gameObject.CompareTag("Conveer"))
            {
                return false; // В этой клетке уже есть другой блок
            }
        }

        return true; // Можно разместить блок
    }

    void SetBlockTransparency(GameObject block, float alpha)
    {
        SpriteRenderer renderer = block.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
    }

    void SetBlockColor(GameObject block, Color color)
    {
        SpriteRenderer renderer = block.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = color;
        }
    }

    void SetBlockLayer(GameObject block, int layer)
    {
        SpriteRenderer renderer = block.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = layer;
        }
    }

    Vector3 SnapToGrid(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        return new Vector3(x, y, 0f);
    }

    bool IsCellOccupied(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
        {
            return true;
        }
        return grid[x, y];
    }

    void PlayBuildSound()
    {
        if (buildSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, buildSounds.Length);
            audioSource.PlayOneShot(buildSounds[randomIndex]);
        }
    }

    void PlayBreakSound()
    {
        if (breakSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, breakSounds.Length);
            audioSource.PlayOneShot(breakSounds[randomIndex]);
        }
    }

    void RotateBlock()
    {
        currentRotation -= 90f; // Вращаем на -90 градусов
        if (currentRotation <= -360f)
        {
            currentRotation += 360f; // Возвращаем в диапазон 0-360 градусов
        }
        if (previewBlock != null)
        {
            previewBlock.transform.rotation = Quaternion.Euler(0f, 0f, currentRotation); // Обновляем вращение блока предварительного просмотра
        }
    }
}
