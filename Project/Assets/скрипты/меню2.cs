using UnityEngine;

public class ToggleCanvas : MonoBehaviour
{
    public Canvas canvas;
    public Camera camera;
    public AudioSource[] normalAudioSources;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    private bool isAttached = false;

    void Start()
    {
        // Save initial transformations of the canvas
        initialPosition = canvas.transform.position;
        initialRotation = canvas.transform.rotation;
        initialScale = canvas.transform.localScale;

        // Initially set canvas to WorldSpace mode
        SetWorldSpaceMode();
    }

    void Update()
    {
        // Check for Esc key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCanvasAttachment();
        }
    }

    // Function to toggle canvas attachment
    void ToggleCanvasAttachment()
    {
        if (isAttached)
        {
            // Detach canvas from camera and return to initial position
            SetWorldSpaceMode();

            // Reset volume of all AudioSources
            foreach (var audioSource in FindObjectsOfType<AudioSource>())
            {
                audioSource.volume = 1f;
            }
        }
        else
        {
            // Attach canvas to camera in ScreenSpaceOverlay mode
            SetScreenSpaceOverlayMode();

            // Set volume of all AudioSources to 10% except those in normalAudioSources
            foreach (var audioSource in FindObjectsOfType<AudioSource>())
            {
                if (System.Array.IndexOf(normalAudioSources, audioSource) == -1)
                {
                    audioSource.volume = 0; // 10% volume
                }
            }
        }

        // Toggle the attachment state
        isAttached = !isAttached;
    }

    // Method to handle back button press on the canvas
    public void OnBackButtonPressed()
    {
        ToggleCanvasAttachment(); // Call the toggle function when the back button is pressed
    }

    // Helper function to set canvas to WorldSpace mode
    void SetWorldSpaceMode()
    {
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.transform.position = initialPosition;
        canvas.transform.rotation = initialRotation;
        canvas.transform.localScale = initialScale;
    }

    // Helper function to set canvas to ScreenSpaceOverlay mode
    void SetScreenSpaceOverlayMode()
    {
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
    }
}
