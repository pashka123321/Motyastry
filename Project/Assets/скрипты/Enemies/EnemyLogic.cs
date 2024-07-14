using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    [SerializeField] private Transform gunTransform;
    public Transform playerTransform;
    [SerializeField] private bool playerNearby;
    private Transform core;

    [SerializeField] private float rotationSpeed = 5f; // скорость поворота

    private void Start()
    {
        core = GameObject.Find("ядро").GetComponent<Transform>(); // замените "Core" на имя объекта с ядром
    }

    private void Update()
    {
        if (playerNearby && IsPlayerCloserThanCore())
        {
            RotateTowardsTarget(playerTransform.position);
        }
        else
        {
            RotateTowardsTarget(core.position);
        }
    }

    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - gunTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        gunTransform.rotation = Quaternion.RotateTowards(gunTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private bool IsPlayerCloserThanCore()
    {
        float distanceToPlayer = Vector3.Distance(gunTransform.position, playerTransform.position);
        float distanceToCore = Vector3.Distance(gunTransform.position, core.position);
        return distanceToPlayer < distanceToCore;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}
