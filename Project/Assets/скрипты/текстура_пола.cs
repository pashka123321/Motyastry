using UnityEngine;

public class GridTextureController : MonoBehaviour
{
    public int gridSizeX = 100; // Ширина сетки в клетках
    public int gridSizeY = 100; // Высота сетки в клетках
    public float cellSize = 1f; // Размер одной клетки (в юнитах)

    public Texture2D gridTexture; // Текстура для заполнения сетки

    void Start()
    {
        if (gridTexture == null)
        {
            Debug.LogError("Grid texture is not assigned!");
            return;
        }

        // Создаем новый SpriteRenderer для отображения текстуры
        GameObject gridObject = new GameObject("GridTexture");
        SpriteRenderer spriteRenderer = gridObject.AddComponent<SpriteRenderer>();

        // Устанавливаем текстуру
        spriteRenderer.sprite = Sprite.Create(gridTexture, new Rect(0, 0, gridTexture.width, gridTexture.height), Vector2.zero, cellSize);

        // Устанавливаем размеры спрайта (размеры текстуры в юнитах)
        float spriteWidth = gridSizeX * cellSize;
        float spriteHeight = gridSizeY * cellSize;
        gridObject.transform.localScale = new Vector3(spriteWidth, spriteHeight, 1f);

        // Устанавливаем слой
        spriteRenderer.sortingLayerName = "LayerName"; // Замените "LayerName" на ваш слой
        spriteRenderer.sortingOrder = 2; // Устанавливаем порядок слоя
    }
}
