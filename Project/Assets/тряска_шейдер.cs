using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class тряска_шейдер : MonoBehaviour
{
    public Material shakeMaterial;
    private Material defaultMaterial;
    private bool applyShake = false;

    [Range(0, 1)] public float shakeIntensity = 0.002f; // Значение из UI
    [Range(0, 50)] public float shakeSpeed = 30f; // Значение из UI

    void Start()
    {
        if (shakeMaterial == null)
        {
            Debug.LogError("Shake material is not assigned.");
        }
        defaultMaterial = new Material(Shader.Find("Sprites/Default")); // Используем стандартный шейдер для спрайтов

        // Устанавливаем параметры в материал
        if (shakeMaterial != null)
        {
            shakeMaterial.SetFloat("_ShakeIntensity", shakeIntensity);
            shakeMaterial.SetFloat("_ShakeSpeed", shakeSpeed);
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (applyShake && shakeMaterial != null)
        {
            Graphics.Blit(src, dest, shakeMaterial);
        }
        else
        {
            Graphics.Blit(src, dest, defaultMaterial);
        }
    }

    public void ApplyShakeEffect()
    {
        applyShake = true;

        // Убедимся, что параметры актуальны при применении
        if (shakeMaterial != null)
        {
            shakeMaterial.SetFloat("_ShakeIntensity", shakeIntensity);
            shakeMaterial.SetFloat("_ShakeSpeed", shakeSpeed);
        }

        Invoke("ResetShakeEffect", 0.2f); // Время применения шейдера тряски
    }

    private void ResetShakeEffect()
    {
        applyShake = false;
    }
}
