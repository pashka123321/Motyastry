using UnityEngine;
using UnityEngine.UI;

public class PlayerSpeedDisplay : MonoBehaviour
{
    public Rigidbody2D playerRigidbody;
    public Text speedText;

    private Vector2 lastPosition;
    private float speed;
    private Vector2 direction;

    void Start()
    {
        if (playerRigidbody != null)
        {
            lastPosition = playerRigidbody.position;
        }
    }

    void Update()
    {
        if (playerRigidbody != null && speedText != null)
        {
            Vector2 currentPosition = playerRigidbody.position;
            Vector2 movement = currentPosition - lastPosition;
            speed = movement.magnitude / Time.deltaTime;
            direction = movement.normalized;
            lastPosition = currentPosition;

            speedText.text = "Speed: " + speed.ToString("F2") + " Direction: " + direction.ToString();
        }
    }

    public Vector2 GetPlayerVelocity()
    {
        return direction * speed;
    }
}
