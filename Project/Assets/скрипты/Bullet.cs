using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    private Vector3 startPosition;
    public float maxDistance = 100f;
    public int damage = 20;
    public int damageType;
    public AudioClip hitSound; // Звук попадания
    private AudioSource audioSource;

    private SpriteRenderer spriteRenderer;
    private float fadeDuration = 2f; // Длительность плавного исчезновения

    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeOutAndDestroy());
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            audioSource = player.GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, damageType);
            if (hitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hitSound, 0.5f); // Воспроизведение звука на 50% громкости
            }
            Destroy(gameObject);
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float startAlpha = spriteRenderer.color.a;
        float rate = 1.0f / fadeDuration;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            Color tmpColor = spriteRenderer.color;
            spriteRenderer.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, Mathf.Lerp(startAlpha, 0, progress));
            progress += rate * Time.deltaTime;

            yield return null;
        }

        Destroy(gameObject);
    }
}
