using UnityEngine;

public class BlockRotator : MonoBehaviour
{
    public GameObject blockPrefab; // Префаб блока
    public Transform parentTransform; // Родительский объект, который будет вращаться вместе с дочерними

    public Color rotationColor = Color.green; // Цвет, который будет применяться при вращении
    public Color defaultColor = Color.white; // Исходный цвет

    private bool isRotating = false;
    private float rotationDuration = 0.2f;
    private float rotationTime = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && IsMouseOver() && blockPrefab != null)
        {
            RotateBlockHierarchy();
        }

        if (isRotating)
        {
            rotationTime += Time.deltaTime;
            if (rotationTime >= rotationDuration)
            {
                isRotating = false;
                SetHierarchyColor(parentTransform, defaultColor);
            }
        }
    }

    void RotateBlockHierarchy()
    {
        if (parentTransform != null)
        {
            parentTransform.Rotate(0, 0, -90f);
            SetHierarchyColor(parentTransform, rotationColor);
        }
        else
        {
            transform.Rotate(0, 0, -90f);
            SetHierarchyColor(transform, rotationColor);
        }

        isRotating = true;
        rotationTime = 0f;
    }

    void SetHierarchyColor(Transform parent, Color color)
    {
        SpriteRenderer spriteRenderer = parent.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }

        foreach (Transform child in parent)
        {
            SetHierarchyColor(child, color);
        }
    }

    public void SetBlockPrefab(GameObject prefab)
    {
        blockPrefab = prefab;
    }

    bool IsMouseOver()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D collider = GetComponent<Collider2D>();
        return collider.bounds.Contains(new Vector3(mousePos.x, mousePos.y, transform.position.z));
    }
}
