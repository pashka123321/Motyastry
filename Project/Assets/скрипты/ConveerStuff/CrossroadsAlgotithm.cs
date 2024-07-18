using UnityEngine;

public class CrossroadsAlgotithm : MonoBehaviour
{
    private ConveerEndChecker conveerEndChecker;

    private MovementController movementController;

    public GameObject[] boxColliders = new GameObject[3];

    public ConveerTrigger conveerTrigger;

    private int k = 0;

    private void Start()
    {
        conveerEndChecker = GetComponent<ConveerEndChecker>();

        movementController = GetComponent<MovementController>();

        conveerTrigger = GetComponentInParent<ConveerTrigger>();

        boxColliders[0] = gameObject.transform.parent.transform.GetChild(3).gameObject;
        boxColliders[1] = gameObject.transform.parent.transform.GetChild(4).gameObject;
        boxColliders[2] = gameObject.transform.parent.transform.GetChild(5).gameObject;

        for (int i = 0; i < 3; i++)
        {
            boxColliders[i].SetActive(true);
        }
    }

    private void Update()
    {
        if (!conveerTrigger.containsOre)
        {
            if (k == 2)
            {
                k = 0;
            }

            for (int i = 0; i < 3; i++)
            {
                boxColliders[i].SetActive(true);
            }

            boxColliders[k].SetActive(false);
            k++;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                boxColliders[i].SetActive(true);
            }
        }
    }
}
