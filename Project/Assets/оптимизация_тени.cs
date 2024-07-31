using UnityEngine;

public class ShadowOptimization : MonoBehaviour
{
    public Collider2D topCollider;
    public Collider2D bottomCollider;
    public Collider2D leftCollider;
    public Collider2D rightCollider;

    private bool isTopColliding;
    private bool isBottomColliding;
    private bool isLeftColliding;
    private bool isRightColliding;

    void Start()
    {
        isTopColliding = false;
        isBottomColliding = false;
        isLeftColliding = false;
        isRightColliding = false;
    }

    void Update()
    {
        if (isTopColliding && isBottomColliding && isLeftColliding && isRightColliding)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ten"))
        {
            if (other == topCollider) isTopColliding = true;
            if (other == bottomCollider) isBottomColliding = true;
            if (other == leftCollider) isLeftColliding = true;
            if (other == rightCollider) isRightColliding = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ten"))
        {
            if (other == topCollider) isTopColliding = false;
            if (other == bottomCollider) isBottomColliding = false;
            if (other == leftCollider) isLeftColliding = false;
            if (other == rightCollider) isRightColliding = false;
        }
    }
}
