using UnityEngine;

public class ResourcesMovementLogic : MonoBehaviour
{
    private void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MovementController movementController = collision.GetComponent<MovementController>();

        if (movementController != null)
        {
            Debug.LogError($"{gameObject.transform.position.y} <> {collision.transform.position.y} <--> {movementController.currentDirection}", gameObject);
            if (movementController.currentDirection == MovementController.Direction.Up && gameObject.transform.position.y < collision.transform.position.y)
            {
                gameObject.GetComponent<MovementController>().enabled = false;
            }
            else if (movementController.currentDirection == MovementController.Direction.Left && gameObject.transform.position.x < collision.gameObject.transform.position.x)
            {
                gameObject.GetComponent<MovementController>().enabled = false;
            }
            else if (movementController.currentDirection == MovementController.Direction.Down && gameObject.transform.position.y > collision.gameObject.transform.position.y)
            {
                gameObject.GetComponent<MovementController>().enabled = false;
            }
            else if (movementController.currentDirection == MovementController.Direction.Right && gameObject.transform.position.x < collision.gameObject.transform.position.x)
            {
                gameObject.GetComponent<MovementController>().enabled = false;
            }
        }
        else
        {
            if (collision.CompareTag("ZavodEnter"))
            {
                gameObject.GetComponent<MovementController>().enabled = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        MovementController movementController = collision.GetComponent<MovementController>();

        if (movementController != null)
        {
            if (movementController.currentDirection == MovementController.Direction.Up && gameObject.transform.position.y < collision.transform.position.y)
            {
                gameObject.GetComponent<MovementController>().enabled = true;
            }
            else if (movementController.currentDirection == MovementController.Direction.Left && gameObject.transform.position.x < collision.gameObject.transform.position.x)
            {
                gameObject.GetComponent<MovementController>().enabled = true;
            }
            else if (movementController.currentDirection == MovementController.Direction.Down && gameObject.transform.position.y > collision.gameObject.transform.position.y)
            {
                gameObject.GetComponent<MovementController>().enabled = true;
            }
            else if (movementController.currentDirection == MovementController.Direction.Right && gameObject.transform.position.x < collision.gameObject.transform.position.x)
            {
                gameObject.GetComponent<MovementController>().enabled = true;
            }
        }
        else
        {
            if (collision.CompareTag("ZavodEnter"))
            {
                Debug.LogError("CanMove");
                gameObject.GetComponent<MovementController>().enabled = true;
            }
        }
    }
}
