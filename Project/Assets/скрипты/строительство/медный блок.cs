using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;

public class BuildModeController : MonoBehaviour
{
    [System.Serializable]
    public class BlockPrefabData
    {
        public GameObject prefab;
        public GameObject previewPrefab;
        public Button button;
        public bool canRotate;
        public ResourceRequirement[] requiredResources; // Изменено на массив ResourceRequirement
    }

    public BlockPrefabData[] blockPrefabsData;
    public AudioClip[] buildSounds;
    public AudioClip[] breakSounds;
    private AudioSource audioSource;
    private GameObject previewBlock;
    private bool isBuildModeActive = false;
    private bool[,] grid;
    private int gridWidth = 200;
    private int gridHeight = 200;
    private int currentBlockPrefabIndex = 0;
    public Text buildModeText;
    private List<GameObject> uiObjectsToIgnore = new List<GameObject>();
    public GameObject[] uiObjectsToIgnoreArray;

    public bool IsBuildModeActive => isBuildModeActive;
    private CoreResourcesScript coreResources; // Ссылка на CoreResourcesScript

    void Start()
    {
        grid = new bool[gridWidth, gridHeight];
        coreResources = FindObjectOfType<CoreResourcesScript>(); // Ищем ссылку на CoreResourcesScript

        foreach (var blockPrefabData in blockPrefabsData)
        {
            blockPrefabData.button.transition = Selectable.Transition.None;
            blockPrefabData.button.onClick.AddListener(() => SetCurrentBlockPrefab(blockPrefabData));
            AddButtonAnimation(blockPrefabData.button);
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on this GameObject.");
        }

        SetPreviewBlock(blockPrefabsData[currentBlockPrefabIndex].previewPrefab);

        buildModeText.gameObject.SetActive(false);

        InitializeUIObjectsToIgnoreArray();
    }

    void AddButtonAnimation(Button button)
    {
        ColorBlock colors = button.colors;
        Color normalColor = colors.normalColor;
        Color pressedColor = new Color(normalColor.r * 0.8f, normalColor.g * 0.8f, normalColor.b * 0.8f, normalColor.a);

        button.onClick.AddListener(() =>
        {
            StartCoroutine(AnimateButtonPress(button, normalColor, pressedColor));
        });
    }

    IEnumerator AnimateButtonPress(Button button, Color normalColor, Color pressedColor)
    {
        button.image.color = pressedColor;
        yield return new WaitForSeconds(0.1f);
        button.image.color = normalColor;
    }

    void SetCurrentBlockPrefab(BlockPrefabData prefabData)
    {
        foreach (var data in blockPrefabsData)
        {
            data.button.interactable = true;
        }

        Destroy(previewBlock);

        for (int i = 0; i < blockPrefabsData.Length; i++)
        {
            if (blockPrefabsData[i] == prefabData)
            {
                currentBlockPrefabIndex = i;
                isBuildModeActive = false;
                ToggleBuildMode();
                SetPreviewBlock(prefabData.previewPrefab);
                prefabData.button.interactable = false;
                break;
            }
        }
    }

    void ToggleBuildMode()
    {
        isBuildModeActive = !isBuildModeActive;

        if (isBuildModeActive)
        {
            buildModeText.text = "Повернуть на R";
            buildModeText.gameObject.SetActive(true);
        }
        else
        {
            buildModeText.gameObject.SetActive(false);
            Destroy(previewBlock);

            foreach (var data in blockPrefabsData)
            {
                data.button.interactable = true;
            }
        }
    }

    void Update()
    {
        bool isPointerOverUI = IsPointerOverIgnoredUI();

        if (isPointerOverUI && !isBuildModeActive)
        {
            if (previewBlock != null)
            {
                previewBlock.SetActive(false);
            }
            return;
        }
        else
        {
            if (previewBlock != null)
            {
                previewBlock.SetActive(true);
            }
        }

        if (isBuildModeActive)
        {
            if (previewBlock == null)
            {
                previewBlock = Instantiate(blockPrefabsData[currentBlockPrefabIndex].previewPrefab);
                SetBlockTransparency(previewBlock, 0.5f);
                SetBlockLayer(previewBlock, 10);
            }
            UpdatePreviewBlockPosition();

            if (Input.GetMouseButton(0) && !isPointerOverUI)
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

        if (Input.GetMouseButton(1))
        {
            if (isBuildModeActive)
            {
                ToggleBuildMode();
            }
            else
            {
                RemoveBlock();
            }
        }

        if (isBuildModeActive && previewBlock != null && blockPrefabsData[currentBlockPrefabIndex].canRotate)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                previewBlock.transform.Rotate(0, 0, -90f);
            }
        }
    }

    bool IsPointerOverIgnoredUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject != null && (uiObjectsToIgnore.Contains(result.gameObject) || System.Array.Exists(uiObjectsToIgnoreArray, element => element == result.gameObject)))
            {
                return true;
            }
        }

        return false;
    }

    public void AddUIObjectToIgnore(GameObject uiObject)
    {
        if (!uiObjectsToIgnore.Contains(uiObject))
        {
            uiObjectsToIgnore.Add(uiObject);
        }
    }

    void InitializeUIObjectsToIgnoreArray()
    {
        foreach (GameObject uiObject in uiObjectsToIgnoreArray)
        {
            if (uiObject != null)
            {
                AddUIObjectToIgnore(uiObject);
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
            SetBlockColor(previewBlock, Color.white);
        }
        else
        {
            SetBlockColor(previewBlock, Color.red);
        }

        SetBlockTransparency(previewBlock, 0.5f);
        SetBlockLayer(previewBlock, 10);
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

        var requiredResources = blockPrefabsData[currentBlockPrefabIndex].requiredResources;
        var resourceDictionary = requiredResources.ToDictionary(r => r.resourceName, r => r.amount);

        if (!coreResources.ConsumeResources(resourceDictionary))
        {
            Debug.Log("Not enough resources to build this block");
            return;
        }

        PlayBuildSound();

        Quaternion rotation = previewBlock.transform.rotation;
        GameObject newBlock = Instantiate(blockPrefabsData[currentBlockPrefabIndex].prefab, new Vector3(x, y, 0f), rotation);
        grid[x, y] = true;

        UpdateGraph(newBlock);
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
            PlayBreakSound();

            Collider2D[] colliders = Physics2D.OverlapPointAll(new Vector2(gridPosition.x, gridPosition.y));
            foreach (Collider2D col in colliders)
            {
                if (col.gameObject.CompareTag("Block") || col.gameObject.CompareTag("Conveer"))
                {
                    UpdateGraphBeforeRemoval(col.gameObject);

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
            return false;
        }

        Collider2D[] colliders = Physics2D.OverlapPointAll(new Vector2(x, y));
        foreach (Collider2D col in colliders)
        {
            if (col.gameObject != previewBlock && (col.gameObject.CompareTag("Block") || col.gameObject.CompareTag("Conveer")))
            {
                return false;
            }
        }

        return true;
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

    void SetPreviewBlock(GameObject previewPrefab)
    {
        Destroy(previewBlock);
        previewBlock = Instantiate(previewPrefab);
        SetBlockTransparency(previewBlock, 0.5f);
        SetBlockLayer(previewBlock, 10);
    }

    void UpdateGraph(GameObject newBlock)
    {
        Bounds bounds = newBlock.GetComponent<Collider2D>().bounds;
        GraphUpdateObject guo = new GraphUpdateObject(bounds);
        AstarPath.active.UpdateGraphs(guo);
    }

    void UpdateGraphBeforeRemoval(GameObject block)
    {
        Bounds bounds = block.GetComponent<Collider2D>().bounds;
        GraphUpdateObject guo = new GraphUpdateObject(bounds);
        AstarPath.active.UpdateGraphs(guo);
    }
}
