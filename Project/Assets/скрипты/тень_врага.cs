using UnityEngine;

public class SmoothOscillation2 : MonoBehaviour
{
    // Ссылка на объект игрока, к которому будет привязан объект
    public Transform player;

    // Начальное смещение
    public Vector3 startOffset = new Vector3(-1f, -1f, 0f);
    // Конечное смещение
    public Vector3 endOffset = new Vector3(-0.7f, -0.7f, 0f);

    // Скорость движения между точками
    public float speed = 1.0f;

    void Update()
    {
        if (player != null)
        {
            // Используем Mathf.SmoothStep для плавного перемещения между начальным и конечным смещением
            float t = Mathf.PingPong(Time.time * speed, 1.0f);
            t = Mathf.SmoothStep(0f, 1f, t);
            Vector3 offset = Vector3.Lerp(startOffset, endOffset, t);

            // Объект следует за игроком с осциллирующим смещением
            transform.position = player.position + offset;
        }
    }
}
