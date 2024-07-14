using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    [SerializeField] private Transform gunTransform;
    public Transform playerTransform;
    [SerializeField] private bool playerNearby;
    private Transform core;

    private void Start()
    {
        core = GameObject.Find("CoreTransform").GetComponent<Transform>(); // Замените "CoreTransform" на имя вашего объекта
    }

    private void LateUpdate()
    {
        if (playerNearby)
        {
            RotateTowards(playerTransform.position);
        }
        else
        {
            RotateTowards(core.position);
        }
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - gunTransform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float currentAngle = gunTransform.eulerAngles.z;
        
        // Интерполяция углов для плавного поворота
        float angle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * 5f); // Увеличьте значение для увеличения скорости поворота

        gunTransform.eulerAngles = new Vector3(0, 0, angle);
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
