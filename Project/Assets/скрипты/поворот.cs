using UnityEngine;

public class RotateTowardsMouse : MonoBehaviour
{
    public GameObject objectToRotate; // Префаб объекта для поворота
    public float rotationSpeed = 5f;  // Скорость поворота

    private void Update()
    {
        RotateObject();
    }

    void RotateObject()
    {
        if (objectToRotate != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector2 direction = (Vector2)mousePosition - (Vector2)objectToRotate.transform.position;
            direction.Normalize();

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

            objectToRotate.transform.rotation = Quaternion.Slerp(objectToRotate.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
