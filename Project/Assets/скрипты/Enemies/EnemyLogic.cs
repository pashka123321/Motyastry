using Pathfinding;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    [SerializeField] private Transform gunTransform;

    public Transform playerTransform;

    [SerializeField] private bool playerNearby;

    private Transform transendenceTransform;

    private Transform core;

    private void Start()
    {
        transendenceTransform = gunTransform;

        core = GameObject.Find("ядро").GetComponent<Transform>();
    }

    private void LateUpdate()
    {
        if (gunTransform.eulerAngles.z == 90)
        {
            Debug.Log(gunTransform.eulerAngles.z);
        }

        if (playerNearby)
        {
            Debug.Log("Ok");
            Vector3 look = gunTransform.InverseTransformPoint(playerTransform.position);
            float angle = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg * Time.deltaTime * 0.45f;

            gunTransform.Rotate(0, 0, angle);
        }
        else
        {
            Vector3 look = gunTransform.InverseTransformPoint(core.position);
            float angle = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg * Time.deltaTime * 0.45f;

            gunTransform.Rotate(0, 0, angle);
        }
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
