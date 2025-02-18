using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteOnRightClick : MonoBehaviour
{
    public static DeleteOnRightClick Instance { get; private set; }

    public Transform player; 
    public LineRenderer lineRenderer; 
    public Transform startPoint; 

    private bool isDestroying = false; 
    private const float maxDestructionDistance = 20f; 

    [System.Serializable]
    public class TargetBlock
    {
        public GameObject prefab;       
        public float destructionDelay;  
    }

    public TargetBlock[] targetBlocks; 
    private Dictionary<string, float> destructionSpeeds = new Dictionary<string, float>(); 

    private Queue<GameObject> destructionQueue = new Queue<GameObject>(); 
    private HashSet<GameObject> objectsInQueue = new HashSet<GameObject>(); 
    private GameObject currentTarget = null; 

    private Queue<BuildTask> buildQueue = new Queue<BuildTask>();
    private bool isBuilding = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        foreach (TargetBlock targetBlock in targetBlocks)
        {
            if (targetBlock.prefab != null)
            {
                destructionSpeeds[targetBlock.prefab.name] = targetBlock.destructionDelay;
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GameObject targetObject = GetObjectUnderMouse();

            if (targetObject != null && destructionSpeeds.ContainsKey(targetObject.name.Replace("(Clone)", "")) &&
                Vector3.Distance(player.position, targetObject.transform.position) <= maxDestructionDistance &&
                !objectsInQueue.Contains(targetObject))
            {
                destructionQueue.Enqueue(targetObject);
                objectsInQueue.Add(targetObject);
                if (currentTarget == null) ProcessNextObject();
            }
        }

        if (currentTarget != null)
        {
            UpdateLine();
            RotatePlayerTowardsObject(currentTarget);
        }
    }

    public void EnqueueBuildTask(GameObject blockPrefab, Vector3 position, Quaternion rotation, float buildTime)
    {
        BuildTask task = new BuildTask(blockPrefab, position, rotation, buildTime);
        buildQueue.Enqueue(task);

        if (!isBuilding) StartCoroutine(ProcessBuildQueue());
    }

    private IEnumerator ProcessBuildQueue()
    {
        isBuilding = true;

        while (buildQueue.Count > 0)
        {
            BuildTask task = buildQueue.Dequeue();
            yield return StartCoroutine(BuildBlock(task.blockPrefab, task.position, task.rotation, task.buildTime));
        }

        isBuilding = false;
    }
private IEnumerator FadeOutAndDestroy(GameObject target, float fadeDuration)
{
    if (target == null) yield break;

    SpriteRenderer[] renderers = target.GetComponentsInChildren<SpriteRenderer>();
    float elapsedTime = 0f;

    // Сохраняем исходные цвета
    Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();
    foreach (SpriteRenderer renderer in renderers)
    {
        originalColors[renderer] = renderer.color;
    }

    // Постепенно уменьшаем прозрачность
    while (elapsedTime < fadeDuration)
    {
        float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer != null)
            {
                Color color = originalColors[renderer];
                color.a = alpha;
                renderer.color = color;
            }
        }
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    // Убедимся, что объект полностью невидим перед удалением
    foreach (SpriteRenderer renderer in renderers)
    {
        if (renderer != null)
        {
            Color color = renderer.color;
            color.a = 0f;
            renderer.color = color;
        }
    }

    // Отключаем lineRenderer
    if (lineRenderer != null)
    {
        lineRenderer.enabled = false;
    }

    // Удаляем объект
    Destroy(target);

    // Обновляем текущую цель и продолжаем разрушение следующих объектов
    currentTarget = null;
    isDestroying = false;
    ProcessNextObject();
}


    private IEnumerator BuildBlock(GameObject prefab, Vector3 position, Quaternion rotation, float buildTime)
    {
        GameObject newBlock = Instantiate(prefab, position, rotation);
        SpriteRenderer[] renderers = newBlock.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer renderer in renderers)
        {
            Color color = renderer.color;
            color.a = 0;
            renderer.color = color;
        }

        float elapsedTime = 0f;

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

        foreach (SpriteRenderer renderer in renderers)
        {
            Color color = renderer.color;
            color.a = 1f;
            renderer.color = color;
        }
    }

GameObject GetObjectUnderMouse()
{
    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Collider2D collider = Physics2D.OverlapPoint(mousePosition);

    if (collider != null)
    {
        // Находим корневой объект, чтобы разрушить весь объект
        Transform rootObject = collider.transform;
        while (rootObject.parent != null)
        {
            rootObject = rootObject.parent;
        }
        return rootObject.gameObject;
    }

    return null;
}


    void ProcessNextObject()
    {
        while (destructionQueue.Count > 0 && destructionQueue.Peek() == null)
        {
            destructionQueue.Dequeue();
        }

        if (destructionQueue.Count > 0)
        {
            currentTarget = destructionQueue.Dequeue();
            objectsInQueue.Remove(currentTarget);
            if (currentTarget != null)
            {
                StartDestruction(currentTarget);
            }
        }
    }

    void RotatePlayerTowardsObject(GameObject target)
    {
        if (target == null) return;

        Vector2 directionToObject = (target.transform.position - player.position).normalized;
        float angle = Mathf.Atan2(directionToObject.y, directionToObject.x) * Mathf.Rad2Deg;
        player.rotation = Quaternion.Euler(0, 0, angle);
    }

void StartDestruction(GameObject target)
{
    if (target == null) return;

    isDestroying = true;

    if (lineRenderer != null)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, startPoint != null ? startPoint.position : player.position);
        lineRenderer.SetPosition(1, target.transform.position);
    }

    string prefabName = target.name.Replace("(Clone)", "");
    float delay = destructionSpeeds[prefabName];

    // Запускаем корутину для плавного исчезновения объекта
    StartCoroutine(FadeOutAndDestroy(target, delay));
}

    void UpdateLine()
    {
        if (lineRenderer != null && currentTarget != null)
        {
            lineRenderer.SetPosition(0, startPoint != null ? startPoint.position : player.position);
            lineRenderer.SetPosition(1, currentTarget.transform.position);
        }
    }

    void DestroyObject()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }

        if (currentTarget != null)
        {
            Destroy(currentTarget);
            currentTarget = null;
            isDestroying = false;

            ProcessNextObject();
        }
    }

    public void EnqueueObjectForDeletion(GameObject targetObject)
    {
        if (targetObject != null && destructionSpeeds.ContainsKey(targetObject.name.Replace("(Clone)", "")) &&
            Vector3.Distance(player.position, targetObject.transform.position) <= maxDestructionDistance &&
            !objectsInQueue.Contains(targetObject))
        {
            destructionQueue.Enqueue(targetObject);
            objectsInQueue.Add(targetObject);
            if (currentTarget == null) ProcessNextObject();
        }
    }

    private class BuildTask
    {
        public GameObject blockPrefab;
        public Vector3 position;
        public Quaternion rotation;
        public float buildTime;

        public BuildTask(GameObject prefab, Vector3 pos, Quaternion rot, float time)
        {
            blockPrefab = prefab;
            position = pos;
            rotation = rot;
            buildTime = time;
        }
    }
}