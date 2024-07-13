using UnityEngine;
using UnityEngine.UI;

public class BuildModeController3 : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject previewBlockPrefab; // New public property for preview block prefab
    public AudioClip[] buildSounds; // Array to hold build sound clips
    public AudioClip[] breakSounds; // Array to hold break sound clips
    private AudioSource audioSource;
    public Button buildModeButton;
    private GameObject previewBlock;
    private bool isBuildModeActive = false;
    private bool[,] grid; // Boolean grid to track occupied cells
    private int gridWidth = 200;
    private int gridHeight = 200;

    void Start()
    {
        grid = new bool[gridWidth, gridHeight];
        buildModeButton.onClick.AddListener(ToggleBuildMode);

        // Получаем компонент AudioSource с текущего объекта
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // Если компонент AudioSource не найден, выведем сообщение об ошибке
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
    }

    void Update()
    {
        if (isBuildModeActive)
        {
            if (previewBlock == null)
            {
                previewBlock = Instantiate(previewBlockPrefab); // Instantiate previewBlockPrefab instead of blockPrefab
                SetBlockTransparency(previewBlock, 0.5f);
            }
            UpdatePreviewBlockPosition();

            if (Input.GetMouseButtonDown(0)) // Left mouse button
            {
                PlaceBlock();
            }
        }
        else
        {
            if (previewBlock != null)
            {
                Destroy(previewBlock);
            }
        }

        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            if (isBuildModeActive)
            {
                isBuildModeActive = false;
                if (previewBlock != null)
                {
                    Destroy(previewBlock);
                }
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

        GameObject newBlock = Instantiate(blockPrefab, new Vector3(x, y, 0f), Quaternion.identity);
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
}