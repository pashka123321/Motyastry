using UnityEngine;
using UnityEngine.Rendering;

public class ConveerEndChecker : MonoBehaviour
{
    [SerializeField] private BoxCollider2D conveerEnd;

    public bool hasEnd;

    private void Start()
    {
        hasEnd = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.LogError(collision.tag);

        if (collision.CompareTag("ConveerEnter"))
        {
            conveerEnd.gameObject.SetActive(false);
            hasEnd = false;
        }
        else if (collision.GetComponent<OvenInteractPointChecker>() != null)
        {
            conveerEnd.gameObject.SetActive(false);
            hasEnd = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ConveerEnter"))
        {
            conveerEnd.gameObject.SetActive(true);
            hasEnd = true;
        }
        else if (collision.GetComponent<OvenInteractPointChecker>() != null)
        {
            conveerEnd.gameObject.SetActive(true);
            hasEnd = true;
        }
    }
}
