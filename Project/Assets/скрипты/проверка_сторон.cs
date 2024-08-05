using UnityEngine;

public class BlockColliderChecker : MonoBehaviour
{
    public GameObject shadowPrefab;
    public Color surroundedColor;

    private bool[] sidesOccupied = new bool[4]; // Up, Right, Down, Left
    private GameObject shadowInstance;

    void Start()
    {
        shadowInstance = Instantiate(shadowPrefab, transform.position, Quaternion.identity, transform);
        shadowInstance.SetActive(false);
    }

    public void InitializeColliders()
    {
        CreateSideCollider(Vector2.up, 0);
        CreateSideCollider(Vector2.right, 1);
        CreateSideCollider(Vector2.down, 2);
        CreateSideCollider(Vector2.left, 3);
    }

    void CreateSideCollider(Vector2 direction, int index)
    {
        GameObject sideColliderObj = new GameObject("SideCollider");
        sideColliderObj.transform.parent = transform;
        sideColliderObj.transform.localPosition = direction * 0.5f;
        BoxCollider2D sideCollider = sideColliderObj.AddComponent<BoxCollider2D>();
        sideCollider.isTrigger = true;
        sideCollider.size = new Vector2(0.1f, 0.1f);
        SideCollider sideColliderScript = sideColliderObj.AddComponent<SideCollider>();
        sideColliderScript.Initialize(this, index);
    }

    public void UpdateSideStatus(int index, bool status)
    {
        sidesOccupied[index] = status;
        CheckSurrounded();
    }

    void CheckSurrounded()
    {
        foreach (bool side in sidesOccupied)
        {
            if (!side)
            {
                shadowInstance.SetActive(true);
                return;
            }
        }

        // All sides are occupied
        GetComponent<SpriteRenderer>().color = surroundedColor;
        shadowInstance.SetActive(false);
    }
}

public class SideCollider : MonoBehaviour
{
    private BlockColliderChecker parentChecker;
    private int sideIndex;

    public void Initialize(BlockColliderChecker parent, int index)
    {
        parentChecker = parent;
        sideIndex = index;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            parentChecker.UpdateSideStatus(sideIndex, true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            parentChecker.UpdateSideStatus(sideIndex, false);
        }
    }
}
