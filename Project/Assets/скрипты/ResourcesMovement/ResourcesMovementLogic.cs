using UnityEngine;

public class ResourcesMovementLogic : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        try
        {
            MovementController movementController = collision.GetComponent<MovementController>();

            Debug.LogError(movementController);

            Debug.LogError("Pon");

            if (!movementController.enabled)
            {
                Debug.LogError("Pon");

                gameObject.GetComponent<MovementController>().enabled = false;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);

            if (collision.CompareTag("ZavodEnter")) 
            {
                Debug.LogError("Damn");
                gameObject.GetComponent<MovementController>().enabled = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<MovementController>() != null)
        {
            gameObject.GetComponent<MovementController>().enabled = true;
        }
    }
}
