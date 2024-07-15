using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour
{
    public int mapWidth = 200;
    public int mapHeight = 200;
    public RawImage miniMapImage;

    private Texture2D miniMapTexture;

    // Сопоставление слоев и цветов
    private Dictionary<int, Color> layerColors = new Dictionary<int, Color>();

    void Start()
    {
        // Инициализация цветов для слоев
        InitializeLayerColors();

        // Создаем текстуру для мини-карты
        miniMapTexture = new Texture2D(mapWidth, mapHeight);
        miniMapImage.texture = miniMapTexture;

        // Обновляем текстуру мини-карты при старте
        UpdateMiniMap();

        // Вызываем UpdateMiniMap каждые 5 секунд, начиная сразу после старта
        InvokeRepeating("UpdateMiniMap", 0f, 5f);
    }

    void InitializeLayerColors()
    {
        // Добавьте свои собственные слои и цвета здесь
        layerColors.Add(LayerMask.NameToLayer("Block"), Color.gray);
        layerColors.Add(LayerMask.NameToLayer("Ore"), new Color(1.0f, 0.5f, 0.0f)); // Оранжевый
    }

    void UpdateMiniMap()
    {
        // Заполняем текстуру цветами на основе объектов на карте
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Color pixelColor = GetPixelColorAtPosition(x, y);
                miniMapTexture.SetPixel(x, y, pixelColor);
            }
        }

        // Применяем изменения к текстуре
        miniMapTexture.Apply();
    }

    Color GetPixelColorAtPosition(int x, int y)
    {
        // Определяем позицию в мировых координатах
        Vector2 worldPosition = new Vector2(x, y);

        // Получаем все коллайдеры в этой точке
        Collider2D[] colliders = Physics2D.OverlapPointAll(worldPosition);

        // Если есть коллайдеры в этой точке
        if (colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                int layer = collider.gameObject.layer;
                if (layerColors.ContainsKey(layer))
                {
                    return layerColors[layer];
                }
            }
        }

        // Если нет объектов в этой точке, возвращаем зеленый цвет
        return new Color(0, 0.5f, 0);
    }
}
