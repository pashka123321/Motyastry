using UnityEngine;
using UnityEngine.EventSystems;

public class CoreToggleScript : MonoBehaviour
{
    public Canvas resourceCanvas;

    // Start is called before the first frame update
    void Start()
    {
        if (resourceCanvas != null)
        {
            resourceCanvas.gameObject.SetActive(false); // Ensure the canvas is initially hidden
        }
    }

    // Respond to mouse up event (recommended for UI interaction)
    void OnMouseUpAsButton()
    {
        if (resourceCanvas != null)
        {
            resourceCanvas.gameObject.SetActive(!resourceCanvas.gameObject.activeSelf); // Toggle canvas visibility
        }
    }

    // Optional: Check mouse down event for debugging
    void OnMouseDown()
    {
        Debug.Log("Mouse down event received.");
    }

    // Update is called once per frame
    void Update()
    {
        // Check for right mouse button click
        if (Input.GetMouseButtonDown(1)) // 1 is the right mouse button
        {
            // Check if the cursor is not over any UI element
            if (!IsPointerOverUIElement())
            {
                // Hide the canvas if it is currently visible
                if (resourceCanvas != null && resourceCanvas.gameObject.activeSelf)
                {
                    resourceCanvas.gameObject.SetActive(false);
                }
            }
        }
    }

    // Check if the pointer is over a UI element
    private bool IsPointerOverUIElement()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
