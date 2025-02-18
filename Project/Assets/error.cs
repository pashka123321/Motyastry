using UnityEngine;

public class MissingSpriteChecker : MonoBehaviour
{
    public Sprite errorSprite; // Спрайт ошибки, который будет использоваться если спрайт отсутствует

    void Start()
    {
        // Получаем все объекты с компонентом SpriteRenderer на сцене
        SpriteRenderer[] renderers = FindObjectsOfType<SpriteRenderer>();
        
        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer.sprite == null) // Проверяем, отсутствует ли спрайт
            {
                renderer.sprite = errorSprite; // Заменяем отсутствующий спрайт на errorSprite
                AdjustSpriteToCollider(renderer); // Настраиваем размер спрайта под коллизию
            }
        }
    }

    void AdjustSpriteToCollider(SpriteRenderer renderer)
    {
        Collider2D collider = renderer.GetComponent<Collider2D>();
        if (collider != null)
        {
            Bounds bounds = collider.bounds;
            Vector3 scale = renderer.transform.lossyScale;
            
            renderer.drawMode = SpriteDrawMode.Sliced;
            renderer.size = new Vector2((bounds.size.x / scale.x), (bounds.size.y / scale.y));
        }
    }
}