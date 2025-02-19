using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class генерация_звезд : MonoBehaviour
{
    public int numberOfStars = 100;
    public float radius = 10f;
    public GameObject starPrefab;
    public float moveSpeed = 0.1f;
    public float blinkSpeed = 1f;
    private List<GameObject> stars = new List<GameObject>();
    private List<float> originalScales = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateStars();
    }

    // Update is called once per frame
    void Update()
    {
        MoveStars();
    }

    void GenerateStars()
    {
        for (int i = 0; i < numberOfStars; i++)
        {
            Vector2 position = new Vector2(Random.Range(-radius, radius), Random.Range(-radius, radius));
            GameObject star = Instantiate(starPrefab, position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            float scale = Random.Range(0.05f, 0.2f);
            star.transform.localScale = new Vector3(scale, scale, 1);
            stars.Add(star);
            originalScales.Add(scale);
        }
    }

    void MoveStars()
    {
        for (int i = 0; i < stars.Count; i++)
        {
            GameObject star = stars[i];
            Vector2 newPosition = star.transform.position + (Vector3)(Random.insideUnitCircle * moveSpeed * Time.deltaTime);
            if (newPosition.magnitude > radius)
            {
                newPosition = newPosition.normalized * radius;
            }
            star.transform.position = newPosition;

            // Blink animation
            float scale = originalScales[i] * (1 + Mathf.Sin(Time.time * blinkSpeed) * 0.1f);
            star.transform.localScale = new Vector3(scale, scale, 1);
        }
    }
}
