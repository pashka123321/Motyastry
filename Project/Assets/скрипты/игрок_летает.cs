using UnityEngine;

public class SmoothOscillation1 : MonoBehaviour
{
    // Ссылка на объект игрока
    public Transform player;

    // Начальный и конечный масштабы игрока
    public Vector3 startScale = new Vector3(1f, 1f, 1f);
    public Vector3 endScale = new Vector3(1.1f, 1.1f, 1.1f);

    // Скорость осцилляции
    public float speed = 1.0f;

    void Update()
    {
        if (player != null)
        {
            // Используем Mathf.PingPong и Mathf.SmoothStep для плавного изменения масштаба игрока
            float t = Mathf.PingPong(Time.time * speed, 1.0f);
            t = Mathf.SmoothStep(0f, 1f, t);

            // Плавное изменение масштаба игрока
            Vector3 scale = Vector3.Lerp(startScale, endScale, t);
            player.localScale = scale;
        }
    }
}
