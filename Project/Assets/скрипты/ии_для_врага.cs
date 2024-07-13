 using UnityEngine;

public class EnemyAI2D : MonoBehaviour
{
    public Transform player; // Reference to the player object
    public float speed = 2.0f; // Enemy movement speed
    public float raycastDistance = 1.0f; // Raycast distance for obstacle detection
    public LayerMask obstacleLayer; // Layers considered as obstacles

    public bool isClone = false; // Flag to determine if this object is a clone

    void Update()
    {
        // If the object is not a clone, it should not move
        if (!isClone)
            return;

        // Check if player reference is set
        if (player != null)
        {
            // Direction towards the player
            Vector3 direction = player.position - transform.position;
            direction.Normalize();

            // Check for obstacles ahead
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastDistance, obstacleLayer);
            if (hit.collider != null)
            {
                // If there is an obstacle, choose a new direction
                Vector2[] alternateDirections = { Quaternion.Euler(0, 0, 45) * direction, Quaternion.Euler(0, 0, -45) * direction };
                foreach (Vector2 alternateDir in alternateDirections)
                {
                    RaycastHit2D hitAlternate = Physics2D.Raycast(transform.position, alternateDir, raycastDistance, obstacleLayer);
                    if (hitAlternate.collider == null)
                    {
                        direction = alternateDir;
                        break;
                    }
                }
            }

            // Move the enemy towards the player
            transform.position += (Vector3)direction * speed * Time.deltaTime;

            // Rotate the enemy towards the player
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}