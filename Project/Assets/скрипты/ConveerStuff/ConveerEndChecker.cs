using UnityEngine;

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
        if (collision.tag != null)
        {
            conveerEnd.enabled = false;
            hasEnd = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != null)
        {
            conveerEnd.enabled = true;
            hasEnd = true;
        }
    }
}
