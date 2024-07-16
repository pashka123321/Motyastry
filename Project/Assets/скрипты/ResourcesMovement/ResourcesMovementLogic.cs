using UnityEngine;

public class ResourcesMovementLogic : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<MovementController>() != null)
        {
            if (collision.GetComponent<MovementController>().canMove == false)
            {
                gameObject.GetComponent<MovementController>().canMove = false;
            }
        }
        else if (collision.CompareTag("ZavodEnter"))
        {
            gameObject.GetComponent<MovementController>().canMove = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<MovementController>() != null)
        {
            gameObject.GetComponent<MovementController>().canMove = true;
        }
    }
}
