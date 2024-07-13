using UnityEngine;

public class ToggleCanvas1 : MonoBehaviour
{
    public Canvas canvas;  // Canvas, который нужно привязать/отвязать
    public Camera camera;  // Камера, к которой нужно привязывать Canvas

    private Vector3 initialPosition;  // Изначальная позиция Canvas
    private Quaternion initialRotation;  // Изначальная ротация Canvas
    private Vector3 initialScale;  // Изначальный масштаб Canvas
    private bool isAttached = false;  // Флаг, который показывает, привязан ли Canvas к камере

    void Start()
    {
        // Сохранить изначальные трансформации Canvas
        initialPosition = canvas.transform.position;
        initialRotation = canvas.transform.rotation;
        initialScale = canvas.transform.localScale;
    }

    void Update()
    {
        // Проверка нажатия клавиши Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCanvasAttachment();
        }
    }

    void ToggleCanvasAttachment()
    {
        if (isAttached)
        {
            // Отвязать Canvas от камеры и вернуть его в изначальную позицию
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.transform.position = initialPosition;
            canvas.transform.rotation = initialRotation;
            canvas.transform.localScale = initialScale;
        }
        else
        {
            // Привязать Canvas к камере в режиме ScreenSpaceOverlay
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        // Переключить флаг
        isAttached = !isAttached;
    }
}
