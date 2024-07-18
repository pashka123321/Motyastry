using UnityEngine;

public class ConveerTrigger : MonoBehaviour
{
    public bool containsOre;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<ResourcesMovementLogic>() != null)
        {
            containsOre = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<ResourcesMovementLogic>() != null)
        {
            containsOre = false;
        }
    }
}
