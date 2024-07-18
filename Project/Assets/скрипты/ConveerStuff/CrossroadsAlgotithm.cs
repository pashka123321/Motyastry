using UnityEngine;

public class CrossroadsAlgotithm : MonoBehaviour
{
    private ConveerEndChecker conveerEndChecker;

    private MovementController movementController;

    private void Start()
    {
        conveerEndChecker = GetComponent<ConveerEndChecker>();

        movementController = GetComponent<MovementController>();


    }
}
