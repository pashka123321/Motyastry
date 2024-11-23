using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    // Префаб или объект, за которым нужно следить
    public Transform target;

    // Смещение относительно цели (можно задать в инспекторе)
    public Vector3 offset = Vector3.zero;

    void Update()
    {
        if (target != null)
        {
            // Телепортируем объект к цели с учетом смещения
            transform.position = target.position + offset;
        }
    }

    // Метод для установки цели через код
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
