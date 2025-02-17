using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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
        public int rtsCost; // Цена в режиме RTS
        public float buildTime; // Добавлено время постройки
        public bool disableScriptsDuringBuild = true; // Флаг для отключения скриптов во время постройки для каждого объекта
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
    public LineRenderer lineRenderer; // Добавьте это поле
    public GameObject playerPrefab; // Префаб игрока
    public Transform lineStartPoint; // Точка начала линии
    public float rotationSpeed = 1f; // Скорость поворота игрока к мышке

    public bool IsBuildModeActive => isBuildModeActive;
    private CoreResourcesScript coreResources; // Ссылка на CoreResourcesScript
    private MoneyManager moneyManager; // Ссылка на MoneyManager
    private List<MonoBehaviour> disabledScripts = new List<MonoBehaviour>();
    private Dictionary<GameObject, List<MonoBehaviour>> disabledScriptsByObject = new Dictionary<GameObject, List<MonoBehaviour>>();
    public bool disableScriptsDuringBuild = true; // Флаг для отключения скриптов во время постройки

    void Start()
    {
        grid = new bool[gridWidth, gridHeight];
        coreResources = FindObjectOfType<CoreResourcesScript>(); // Ищем ссылку на CoreResourcesScript
        moneyManager = FindObjectOfType<MoneyManager>(); // Ищем ссылку на MoneyManager

        if (moneyManager == null)
        {
            Debug.LogError("MoneyManager not found on this scene.");
        }

        if (coreResources == null && !moneyManager.IsRTSMode())
        {
            Debug.LogError("CoreResourcesScript not found on this scene.");
        }

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
        Color pressedColor = new Color(normalColor.r * 0.8f, normalColor.g * 8f, normalColor.b * 0.8f, normalColor.a);

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

            if (lineRenderer != null)
            {
                lineRenderer.enabled = true; // Включаем линию снова
            }
        }
        else
        {
            buildModeText.gameObject.SetActive(false);
            Destroy(previewBlock);

            if (lineRenderer != null)
            {
                lineRenderer.enabled = false; // Скрываем линию при выходе
            }

            foreach (var data in blockPrefabsData)
            {
                data.button.interactable = true;
            }
        }
    }


    void Update()
    {
        bool isPointerOverUI = IsPointerOverIgnoredUI();

        // Если курсор находится над UI, блокируем возможность строительства
        if (isPointerOverUI)
        {
            // Деактивируем предпросмотр блока
            if (previewBlock != null)
            {
                previewBlock.SetActive(false);
            }

            if (lineRenderer != null)
            {
                lineRenderer.enabled = false; // Отключаем линию, если курсор над UI
            }

            return;
        }
        else
        {
            // Активируем предпросмотр блока, если он существует
            if (previewBlock != null)
            {
                previewBlock.SetActive(true);
            }
        }

        // Логика строительства и удаления блоков
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

            RotatePlayerTowardsMouse(); // Поворачиваем игрока за мышкой

            if (lineRenderer != null)
            {
                if (previewBlock != null)
                {
                    lineRenderer.enabled = true; // Линия включена, если есть объект для постройки
                    UpdateBuildLine(previewBlock); // Обновляем линию во время строительства
                }
                else
                {
                    lineRenderer.enabled = false; // Линия отключена, если нет объекта для постройки
                }
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
        }

        if (isBuildModeActive && previewBlock != null && blockPrefabsData[currentBlockPrefabIndex].canRotate)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                previewBlock.transform.Rotate(0, 0, -90f);
            }
        }

        if (isBuildModeActive && previewBlock != null)
        {
            UpdateBuildLine(previewBlock); // Обновляем линию во время строительства
        }
    }

    bool IsPointerOverIgnoredUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject != null)
                {
                    // Проверяем сам объект и его родителей
                    Transform currentTransform = result.gameObject.transform;
                    while (currentTransform != null)
                    {
                        if (uiObjectsToIgnore.Contains(currentTransform.gameObject) ||
                            System.Array.Exists(uiObjectsToIgnoreArray, element => element == currentTransform.gameObject))
                        {
                            return true;
                        }
                        currentTransform = currentTransform.parent;
                    }
                }
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
        if (moneyManager == null)
        {
            Debug.LogError("MoneyManager not found on this scene.");
            return;
        }

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

        if (moneyManager.IsRTSMode())
        {
            int totalCost = blockPrefabsData[currentBlockPrefabIndex].rtsCost;
            if (moneyManager.money < totalCost)
            {
                Debug.Log("Not enough money to build this block");
                return;
            }
            moneyManager.SubtractMoney(totalCost);
        }
        else
        {
            if (coreResources == null)
            {
                Debug.LogError("CoreResourcesScript not found on this scene.");
                return;
            }

            var requiredResources = blockPrefabsData[currentBlockPrefabIndex].requiredResources;
            var resourceDictionary = requiredResources.ToDictionary(r => r.resourceName, r => r.amount);

            if (!coreResources.ConsumeResources(resourceDictionary))
            {
                Debug.Log("Not enough resources to build this block");
                return;
            }
        }

        PlayBuildSound();

        Quaternion rotation = previewBlock.transform.rotation;
        Vector3 position = new Vector3(x, y, 0f);
        StartCoroutine(BuildBlock(blockPrefabsData[currentBlockPrefabIndex].prefab, position, rotation, blockPrefabsData[currentBlockPrefabIndex].buildTime));
    }

    private IEnumerator BuildBlock(GameObject prefab, Vector3 position, Quaternion rotation, float buildTime)
    {
        GameObject newBlock = Instantiate(prefab, position, rotation);
        
        if (blockPrefabsData[currentBlockPrefabIndex].disableScriptsDuringBuild)
        {
            DisableScriptsOnObject(newBlock); // Отключаем скрипты на новом объекте
        }

        SpriteRenderer[] renderers = newBlock.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer renderer in renderers)
        {
            Color color = renderer.color;
            color.a = 0;
            renderer.color = color;
        }

        float elapsedTime = 0f;

        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, previewBlock.transform.position);
            lineRenderer.SetPosition(1, newBlock.transform.position);
        }

        while (elapsedTime < buildTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / buildTime);

            foreach (SpriteRenderer renderer in renderers)
            {
                Color color = renderer.color;
                color.a = alpha;
                renderer.color = color;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }

        foreach (SpriteRenderer renderer in renderers)
        {
            Color color = renderer.color;
            color.a = 1f;
            renderer.color = color;
        }

        grid[Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y)] = true;
        UpdateGraph(newBlock);

        EnableScriptsOnObject(newBlock); // Включаем скрипты после завершения постройки
    }

    public void DestroyByEnemy(Vector3 destoyedBlock)
    {
        Vector3 gridPosition = SnapToGrid(destoyedBlock);

        int x = Mathf.RoundToInt(gridPosition.x);
        int y = Mathf.RoundToInt(gridPosition.y);

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

    void DisableScriptsOnObject(GameObject obj)
    {
        if (!blockPrefabsData[currentBlockPrefabIndex].disableScriptsDuringBuild) return; // Проверяем флаг

        if (!disabledScriptsByObject.ContainsKey(obj))
        {
            disabledScriptsByObject[obj] = new List<MonoBehaviour>();
        }

        MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script.enabled && script.GetType() != typeof(BlockHealth)) // Исключаем BlockHealth
            {
                script.enabled = false;
                disabledScriptsByObject[obj].Add(script);
            }
        }
    }

    void EnableScriptsOnObject(GameObject obj)
    {
        if (!blockPrefabsData[currentBlockPrefabIndex].disableScriptsDuringBuild) return; // Проверяем флаг

        if (obj == null || !disabledScriptsByObject.ContainsKey(obj))
        {
            return;
        }

        foreach (var script in disabledScriptsByObject[obj])
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }

        disabledScriptsByObject.Remove(obj);
    }

    void UpdateBuildLine(GameObject target)
    {
        if (lineRenderer != null && target != null)
        {
            if (lineStartPoint != null)
            {
                lineRenderer.SetPosition(0, lineStartPoint.position);
            }
            else if (playerPrefab != null)
            {
                lineRenderer.SetPosition(0, playerPrefab.transform.position);
            }
            lineRenderer.SetPosition(1, target.transform.position);
        }
    }

    void RotatePlayerTowardsMouse()
    {
        if (playerPrefab == null) return;

        PlayerMovement playerMovement = playerPrefab.GetComponent<PlayerMovement>();
        if (playerMovement == null) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 direction = mousePosition - playerPrefab.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        playerPrefab.transform.rotation = Quaternion.RotateTowards(playerPrefab.transform.rotation, targetRotation, playerMovement.buildModeRotationSpeed * Time.deltaTime);
    }
}