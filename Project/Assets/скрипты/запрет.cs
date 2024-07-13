using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxX = 300f;
    public float maxY = 300f;
    public float minX = -300f;
    public float minY = -300f;

    private void Update()
    {
        // Ограничиваем позицию игрока в пределах заданных координат
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(transform.position.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(transform.position.y, minY, maxY);
        transform.position = clampedPosition;
    }
}