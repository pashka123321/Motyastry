using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class поиск_путей : MonoBehaviour
{
    public LayerMask blockLayer;
    public float nodeSpacing = 0.5f;
    public int maxIterations = 100;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0;
            List<Vector3> path = FindPath(transform.position, targetPosition);
            DrawPath(path);
        }
    }

    List<Vector3> FindPath(Vector3 start, Vector3 target)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 currentPosition = start;
        path.Add(currentPosition);

        for (int i = 0; i < maxIterations; i++)
        {
            Vector3 direction = (target - currentPosition).normalized;
            Vector3 nextPosition = currentPosition + direction * nodeSpacing;

            if (Physics2D.OverlapCircle(nextPosition, nodeSpacing / 2, blockLayer))
            {
                nextPosition = FindAlternativePosition(currentPosition, direction);
            }

            path.Add(nextPosition);
            currentPosition = nextPosition;

            if (Vector3.Distance(currentPosition, target) < nodeSpacing)
            {
                path.Add(target);
                break;
            }
        }

        return path;
    }

    Vector3 FindAlternativePosition(Vector3 currentPosition, Vector3 direction)
    {
        Vector3[] alternatives = {
            new Vector3(-direction.y, direction.x),
            new Vector3(direction.y, -direction.x),
            new Vector3(-direction.y, -direction.x),
            new Vector3(direction.y, direction.x)
        };

        foreach (var alt in alternatives)
        {
            Vector3 alternativePosition = currentPosition + alt * nodeSpacing;
            if (!Physics2D.OverlapCircle(alternativePosition, nodeSpacing / 2, blockLayer))
            {
                return alternativePosition;
            }
        }

        return currentPosition;
    }

    void DrawPath(List<Vector3> path)
    {
        lineRenderer.positionCount = path.Count;
        lineRenderer.SetPositions(path.ToArray());
    }
}
