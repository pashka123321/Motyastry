using Pathfinding;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    [SerializeField] private Transform gunTransform;

    public Transform playerTransform;

    private bool playerNearby;

    private void Start()
    {

    }

    private void Update()
    {
        if (playerNearby)
        {
            Vector3 look = gunTransform.InverseTransformPoint(playerTransform.position);
            float angle = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg * Time.deltaTime * 0.45f;

            gunTransform.Rotate(0, 0, angle);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerNearby = false;
        }
    }
}
