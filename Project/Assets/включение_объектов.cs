using UnityEngine;

public class TimedObjectActivator : MonoBehaviour
{
    [System.Serializable]
    public class TimedObject
    {
        public GameObject obj;
        public float activationDelay;
    }

    public TimedObject[] objectsToActivate;
    public TimedObject[] objectsToEnableCollision;

    void Start()
    {
        // Запускаем активацию объектов
        foreach (var item in objectsToActivate)
        {
            StartCoroutine(ActivateObjectAfterDelay(item.obj, item.activationDelay));
        }

        // Запускаем включение коллизии объектов
        foreach (var item in objectsToEnableCollision)
        {
            StartCoroutine(EnableCollisionAfterDelay(item.obj, item.activationDelay));
        }
    }

    private System.Collections.IEnumerator ActivateObjectAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            obj.SetActive(true);
        }
    }

    private System.Collections.IEnumerator EnableCollisionAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null)
        {
            Collider2D collider = obj.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
    }
}
