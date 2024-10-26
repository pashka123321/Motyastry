using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaCoiltesla : MonoBehaviour
{
    public LayerMask targetLayers; // Слои, на которых будут находиться цели
    public Sprite connectedSprite;
    public Sprite disconnectedSprite;
    public float boltDuration = 0.1f;
    public float strikeInterval = 0.1f;
    public int boltSegments = 10;
    public float boltOffset = 0.5f;
    public float activationRadius = 10f;
    public float energyTransferRate = 10f;
    public Material boltMaterial;

    private List<LineRenderer> lineRenderers = new List<LineRenderer>();
    private SpriteRenderer spriteRenderer;
    private bool isActive;
    private List<Transform> currentTargets = new List<Transform>();
    private float currentEnergy = 100f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
        StartCoroutine(StrikeRoutine());
    }

    IEnumerator StrikeRoutine()
    {
        while (true)
        {
            if (isActive && currentTargets.Count > 0)
            {
                for (int i = 0; i < currentTargets.Count; i++)
                {
                    if (currentTargets[i] != null)
                    {
                        // Убедиться, что есть LineRenderer для текущей цели
                        if (i >= lineRenderers.Count)
                        {
                            CreateLineRenderer(i);
                        }

                        StartCoroutine(GenerateBolt(currentTargets[i].position, lineRenderers[i]));

                        // Передача энергии
                        TransferEnergy(currentTargets[i]);
                    }
                }
            }
            yield return new WaitForSeconds(strikeInterval);
        }
    }

    void CreateLineRenderer(int index)
    {
        GameObject boltObject = new GameObject("Bolt");
        boltObject.transform.parent = transform;
        LineRenderer newLine = boltObject.AddComponent<LineRenderer>();
        newLine.positionCount = boltSegments;
        newLine.material = boltMaterial;
        newLine.startWidth = 0.1f;
        newLine.endWidth = 0.1f;
        newLine.startColor = Color.white;
        newLine.endColor = Color.white;
        newLine.useWorldSpace = true;
        lineRenderers.Add(newLine);
    }

    void TransferEnergy(Transform target)
    {
        Battery battery = target.GetComponent<Battery>();
        CoalGenerator generator = target.GetComponent<CoalGenerator>();
        WindTurbine windTurbine = target.GetComponent<WindTurbine>();

        if (battery != null)
        {
            battery.StoreEnergy(energyTransferRate * strikeInterval);
        }
        else if (generator != null)
        {
            generator.AddCoal(Mathf.FloorToInt(energyTransferRate * strikeInterval));
        }
        else if (windTurbine != null)
        {
            windTurbine.ReceiveEnergy(energyTransferRate * strikeInterval);
        }
    }

    IEnumerator GenerateBolt(Vector3 targetPosition, LineRenderer lineRenderer)
    {
        Vector3 startPosition = transform.position;

        for (int i = 0; i < boltSegments; i++)
        {
            float t = i / (float)(boltSegments - 1);
            Vector3 position = Vector3.Lerp(startPosition, targetPosition, t);
            position.x += Random.Range(-boltOffset, boltOffset);
            position.y += Random.Range(-boltOffset, boltOffset);
            lineRenderer.SetPosition(i, position);
        }

        lineRenderer.enabled = true;
        yield return new WaitForSeconds(boltDuration);
        lineRenderer.enabled = false;
    }

    void Update()
    {
        UpdateTargets(); // Обновление целей каждый кадр
        isActive = currentTargets.Count > 0;
        UpdateSprite();
    }

    void UpdateTargets()
    {
        currentTargets.RemoveAll(target => target == null || Vector3.Distance(transform.position, target.position) > activationRadius);

        Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(transform.position, activationRadius, targetLayers);

        foreach (var target in targetsInRange)
        {
            if (target != null && !currentTargets.Contains(target.transform))
            {
                if (IsTargetValid(target))
                {
                    currentTargets.Add(target.transform);
                }
            }
        }
    }

    bool IsTargetValid(Collider2D target)
    {
        Battery battery = target.GetComponent<Battery>();
        CoalGenerator generator = target.GetComponent<CoalGenerator>();
        TeslaCoiltesla coil = target.GetComponent<TeslaCoiltesla>();
        WindTurbine windTurbine = target.GetComponent<WindTurbine>();

        return (battery != null || generator != null || (coil != null && coil != this) || windTurbine != null);
    }

    public void ReceiveEnergy(float amount)
    {
        currentEnergy += amount;
    }

    void UpdateSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = isActive ? connectedSprite : disconnectedSprite;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}
