using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class PixelateEffect : MonoBehaviour
{
    public Material pixelateMaterial;
    [Range(0.001f, 0.1f)]
    public float pixelSize = 0.01f;
    [Range(0.1f, 1.0f)]
    public float resolutionScale = 0.5f; // коэффициент уменьшения разрешения

    public List<GameObject> objectsToExclude; // Список объектов, которые камера не будет видеть

    private RenderTexture lastFrame;
    private Camera cam;
    private float updateInterval = 1f / 30f; // интервал обновления в секундах (1/30 сек)

    void Start()
    {
        cam = GetComponent<Camera>();
        StartCoroutine(UpdateFrame());
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (cam.enabled)
        {
            int width = (int)(src.width * resolutionScale);
            int height = (int)(src.height * resolutionScale);

            if (lastFrame == null || lastFrame.width != width || lastFrame.height != height)
            {
                if (lastFrame != null)
                {
                    lastFrame.Release();
                }
                lastFrame = new RenderTexture(width, height, src.depth);
            }

            if (pixelateMaterial != null)
            {
                pixelateMaterial.SetFloat("_PixelSize", pixelSize);
                Graphics.Blit(src, lastFrame, pixelateMaterial);
            }
            else
            {
                Graphics.Blit(src, lastFrame);
            }

            Graphics.Blit(lastFrame, dest);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }

    IEnumerator UpdateFrame()
    {
        while (true)
        {
            // Включить камеру
            cam.enabled = true;
            yield return new WaitForEndOfFrame(); // Подождать конца кадра, чтобы камера успела нарисовать

            // Выключить камеру
            cam.enabled = false;
            yield return new WaitForSeconds(updateInterval); // Ждать интервал в 1/30 сек для следующего обновления
        }
    }

    void LateUpdate()
    {
        // Фиксация вращения вокруг оси Z
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.z = 0;
        transform.eulerAngles = eulerAngles;
    }

    void OnDestroy()
    {
        if (lastFrame != null)
        {
            lastFrame.Release();
        }
    }

    void OnPreCull()
    {
        // Исключение объектов из вида камеры
        foreach (var obj in objectsToExclude)
        {
            if (obj != null)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
                }
            }
        }
    }

    void OnPostRender()
    {
        // Восстановление видимости объектов после отрисовки кадра
        foreach (var obj in objectsToExclude)
        {
            if (obj != null)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = true;
                }
            }
        }
    }
}
