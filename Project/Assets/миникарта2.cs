using UnityEngine;
using UnityEngine.UI;

public class CameraDisplay : MonoBehaviour
{
    public Camera cameraToDisplay;
    public RawImage displayImage;

    private void Start()
    {
        if (cameraToDisplay != null && displayImage != null)
        {
            // Создаем RenderTexture и назначаем его камере
            RenderTexture renderTexture = new RenderTexture(200, 200, 24);
            cameraToDisplay.targetTexture = renderTexture;

            // Назначаем RenderTexture RawImage
            displayImage.texture = renderTexture;
        }
    }
}