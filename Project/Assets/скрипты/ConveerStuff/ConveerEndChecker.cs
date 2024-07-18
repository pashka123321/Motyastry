using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class ConveerEndChecker : MonoBehaviour
{
    [SerializeField] private BoxCollider2D conveerEnd;

    public bool hasEnd;

    public int enters;

    private void Start()
    {
        hasEnd = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.LogError(collision.tag);

        if (collision.CompareTag("ConveerEnter"))
        {
            ConveerEndChecker tempCEC = collision.GetComponent<ExitReference>().exit.GetComponent<ConveerEndChecker>();

            tempCEC.enters++;

            if (tempCEC.enters > 1 && tempCEC.GetComponent<CrossroadsAlgotithm>() == null)
            {
                tempCEC.gameObject.AddComponent<CrossroadsAlgotithm>();
            }

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
            ConveerEndChecker tempCEC = collision.GetComponent<ExitReference>().exit.GetComponent<ConveerEndChecker>();

            tempCEC.enters--;

            if (tempCEC.enters < 2 && tempCEC.GetComponent<CrossroadsAlgotithm>() != null)
            {
                Destroy(tempCEC.gameObject.GetComponent<CrossroadsAlgotithm>());
            }
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
