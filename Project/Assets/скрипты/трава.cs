using UnityEngine;

public class GrassShaderController2D : MonoBehaviour
{
    public Shader grassShader;  // Ссылка на ваш 2D шейдер
    public Texture2D grassTexture;  // Ссылка на текстуру травы

    // Параметры для управления колебанием травы
    public float waveSpeed = 1.0f;
    public float waveAmplitude = 0.05f;
    public float frequency = 3.0f;

    private Material tempMaterial;

    void Start()
    {
        // Проверка, есть ли шейдер
        if (grassShader == null)
        {
            Debug.LogError("Shader не назначен!");
            return;
        }

        // Создаем временный материал с использованием нашего 2D шейдера
        tempMaterial = new Material(grassShader);

        // Назначаем текстуру нашему временному материалу
        if (grassTexture != null)
        {
            tempMaterial.SetTexture("_MainTex", grassTexture);
        }

        // Назначаем параметры шейдера
        tempMaterial.SetFloat("_WaveSpeed", waveSpeed);
        tempMaterial.SetFloat("_WaveAmplitude", waveAmplitude);
        tempMaterial.SetFloat("_Frequency", frequency);

        // Применяем материал к компоненту SpriteRenderer
        GetComponent<SpriteRenderer>().material = tempMaterial;
    }

    void Update()
    {
        // Динамическое обновление параметров шейдера, если это необходимо
        if (tempMaterial != null)
        {
            tempMaterial.SetFloat("_WaveSpeed", waveSpeed);
            tempMaterial.SetFloat("_WaveAmplitude", waveAmplitude);
            tempMaterial.SetFloat("_Frequency", frequency);
        }
    }

    void OnDestroy()
    {
        // Уничтожаем временный материал, чтобы избежать утечек памяти
        if (tempMaterial != null)
        {
            Destroy(tempMaterial);
        }
    }
}
